using System;
using System.Linq;
using System.Transactions;

using Chiro.Ad.ServiceContracts;
using Chiro.Adf.ServiceModel;
using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Manager die informatie ophaalt over gebruikersrechten van personen waar jij 
    /// gebruikersrecht op hebt.
    /// </summary>
    public class GebruikersRechtenManager
    {
        private readonly IAutorisatieDao _autorisatieDao;
        private readonly IDao<GebruikersRecht> _gebruikersRechtenDao;
        private readonly IAutorisatieManager _autorisatieManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autorisatieDao">DAO die gebruikt zal worden om gegevens ivm gebruikersrechten op te zoeken</param>
        /// <param name="gebruikersRechtenDao">DAO voor data access ivm gebruikersrechten</param>
        /// <param name="autorisatieManager">Autorisatiemanager die gebruikt zal worden om te controleren of de user wel
        /// rechten genoeg heeft om de gevraagde gegevens op te halen</param>
        public GebruikersRechtenManager(IAutorisatieDao autorisatieDao, IDao<GebruikersRecht> gebruikersRechtenDao, IAutorisatieManager autorisatieManager)
        {
            _autorisatieDao = autorisatieDao;
            _gebruikersRechtenDao = gebruikersRechtenDao;
            _autorisatieManager = autorisatieManager;
        }


        /// <summary>
        /// Als een gelieerde persoon een gebruikersrecht heeft/had voor zijn eigen groep, dan
        /// levert deze call dat gebruikersrecht op, inclusief GAV-object.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van een gelieerde persoon</param>
        /// <returns>Gebruikersrecht van de gelieerde persoon met ID <paramref name="gelieerdePersoonID"/>
        /// op zijn eigen groep (if any, anders null)</returns>
        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            if (!_autorisatieManager.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            return _autorisatieDao.GebruikersRechtGelieerdePersoon(gelieerdePersoonID);
        }

        /// <summary>
        /// Als een gelieerde persoon een gebruikersrecht heeft/had voor zijn eigen groep, dan
        /// levert deze call dat gebruikersrecht op.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van een gelieerde persoon</param>
        /// <returns>Gebruikersrecht van de gelieerde persoon met ID <paramref name="gelieerdePersoonID"/>
        /// op zijn eigen groep (if any, anders null)</returns>
        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersrecht.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            return gebruikersrecht.VervalDatum == null
                       ? false
                       : ((DateTime)gebruikersrecht.VervalDatum) < DateTime.Now.AddMonths(
                           Properties.Settings.Default.MaandenGebruikersRechtVerlengbaar);
        }

        /// <summary>
        /// Geeft <paramref name="persoon"/> GAV-recht op <paramref name="groep"/> voor de standaardduur.  Als er al
        /// gebruikersrechten bestonden, worden die zo mogelijk verlengd.  MOET PERSISTEREN, want er wordt een login 
        /// aangemaakt in AD, en dat moet dus allemaal in 1 transactie.
        /// </summary>
        /// <param name="persoon">Persoon die gebruikersrecht moet krijgen, met daaraan gekoppeld eventueel bestaande
        /// gebruikersrechten, en e-mailadres</param>
        /// <param name="groep">Groep waarvoor <paramref name="persoon"/> gebruikersrecht moet krijgen</param>
        /// <param name="eMail">E-mailadres dat gebruikt moet worden om accountinfo op te sturen</param>
        /// <returns>Het gecreeerde gebruikersrechtobject</returns>
        public GebruikersRecht ToekennenOfVerlengen(Persoon persoon, Groep groep, string eMail)
        {
            Gav gav;
            GebruikersRecht resultaat;
            int adNr;

            if (!_autorisatieManager.IsGavPersoon(persoon.ID) || !_autorisatieManager.IsGavGroep(groep.ID) )
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            if (persoon.AdNummer == null)
            {
                throw new FoutNummerException(FoutNummer.AdNummerVerplicht, Properties.Resources.AdNummerVerplicht);
            }
            adNr = (int)persoon.AdNummer;

            if (persoon.Gav.FirstOrDefault() != null)
            {
                // Er is al een GAV.
                gav = persoon.Gav.FirstOrDefault();
            }
            else
            {
                if (String.IsNullOrEmpty(eMail))
                {
                    throw new FoutNummerException(FoutNummer.EMailVerplicht, Properties.Resources.EMailVerplicht);
                }

                // Maak GAV

                gav = new Gav();

                gav.Persoon.Add(persoon);
                persoon.Gav.Add(gav);
            }

            // Is er al gebruikersrecht?

            var gebruikersRecht = (from gr in gav.GebruikersRecht
                                   where gr.Groep.ID == groep.ID
                                   select gr).FirstOrDefault();

            // (bovenstaande query kan normaalgezien hoogstens 1 object opleveren.

            if (gebruikersRecht == null)
            {
                // Nog geen gebruikersrecht: maak aan, en koppel groep en GAV

                gebruikersRecht = new GebruikersRecht {ID = 0, Gav = gav};

                gav.GebruikersRecht.Add(gebruikersRecht);

                gebruikersRecht.Groep = groep;
                groep.GebruikersRecht.Add(gebruikersRecht);
            }
            else if (!IsVerlengbaar(gebruikersRecht))
            {
                // Als er gebruikersrecht is, maar dat is niet verlengbaar, dan gooien
                // we er een exception tegenaan.

                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar, Properties.Resources.GebruikersRechtNietVerlengbaar);
            }

            // Vervaldatum aanpassen

            gebruikersRecht.VervalDatum = DateTime.Now.AddMonths(Properties.Settings.Default.MaandenGebruikersRechtStandaard);

            // transactie voor AD-account aanmaken en persisteren.

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (gav.ID == 0)
                {
                    // Als het om een nieuwe GAV gaat, vraag dan een AD-account aan, en bewaar
                    // de login

                    gav.Login = ServiceHelper.CallService<IAdService, string>(
                        svc => svc.GapLoginAanvragen(adNr, persoon.Naam, persoon.VoorNaam, eMail));
                }

                resultaat = _gebruikersRechtenDao.Bewaren(gebruikersRecht,
                                                              gr => gr.Gav.Persoon.First().WithoutUpdate(),
                                                              gr => gr.Groep.WithoutUpdate());
#if KIPDORP
                tx.Complete();
            }
#endif

            return resultaat;

        }
    }
}
