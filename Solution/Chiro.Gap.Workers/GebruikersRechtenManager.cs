using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Chiro.Ad.ServiceContracts;
using Chiro.Adf.ServiceModel;
using Chiro.Cdf.Data;
using Chiro.Cdf.Mailer;
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
        private readonly IGavDao _gavDao;
        private readonly IGelieerdePersonenDao _gelieerdePersonenDao;
        private readonly IAutorisatieManager _autorisatieManager;
        private readonly IMailer _mailer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autorisatieDao">DAO die gebruikt zal worden om gegevens ivm gebruikersrechten op te zoeken</param>
        /// <param name="gelieerdePersonenDao">data access voor gelieerdepersonen</param>
        /// <param name="gavDao">voorziet data access voor GAV's</param>
        /// <param name="autorisatieManager">Data access ivm gebruikersrechten</param>
        /// <param name="mailer">Klasse die mails zal sturen</param>
        public GebruikersRechtenManager(
            IAutorisatieDao autorisatieDao, 
            IGelieerdePersonenDao gelieerdePersonenDao, 
            IGavDao gavDao,
            IAutorisatieManager autorisatieManager,
            IMailer mailer)
        {
            _autorisatieDao = autorisatieDao;
            _gelieerdePersonenDao = gelieerdePersonenDao;
            _gavDao = gavDao;
            _autorisatieManager = autorisatieManager;
            _mailer = mailer;
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
        /// geeft <c>true</c> als de 'gebruikersrechtvervaldatum' <paramref name="datum"/>  in de
        /// nabije toekomst is, anders <c>false</c>.
        /// </summary>
        /// <param name="datum">Vervaldatum van een gebruikersrecht</param>
        /// <returns><c>true</c> als de 'gebruikersrechtvervaldatum' <paramref name="datum"/>  in de
        /// nabije toekomst is, anders <c>false</c>.</returns>
        public static bool VervalDatumNabij(DateTime? datum)
        {
            return datum == null
           ? false
           : ((DateTime)datum) < DateTime.Now.AddMonths(
               Properties.Settings.Default.MaandenGebruikersRechtVerlengbaar);
        }

        /// <summary>
        /// Geeft <c>true</c> als het gegeven <paramref name="gebruikersrecht"/> verlengd
        /// kan worden, anders <c>false</c>
        /// </summary>
        /// <param name="gebruikersrecht">gebruikersrecht waarvan de verlengbaarheid te controleren is</param>
        /// <returns><c>true</c> als het gegeven <paramref name="gebruikersrecht"/> verlengd
        /// kan worden, anders <c>false</c>.</returns>
        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            if (!(_autorisatieManager.IsSuperGav() || _autorisatieManager.IsGavGebruikersRecht(gebruikersrecht.ID)))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            return VervalDatumNabij(gebruikersrecht.VervalDatum);
        }

        /// <summary>
        /// Verlengt het gegeven <paramref name="gebruikersRecht"/> (indien mogelijk) tot het standaard aantal maanden
        /// na vandaag.
        /// </summary>
        /// <param name="gebruikersRecht">Te verlengen gebruikersrecht</param>
        public void Verlengen(GebruikersRecht gebruikersRecht)
        {
            // TODO: Nakijken of deze method niet in onderstaande gebruikt kan worden.

            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersRecht.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            else if (!IsVerlengbaar(gebruikersRecht))
            {
                // Als er gebruikersrecht is, maar dat is niet verlengbaar, dan gooien
                // we er een exception tegenaan.

                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar, Properties.Resources.GebruikersRechtNietVerlengbaar);
            }

            // Vervaldatum aanpassen

            gebruikersRecht.VervalDatum = DateTime.Now.AddMonths(Properties.Settings.Default.MaandenGebruikersRechtStandaard);
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

           var gebruikersRecht = ToekennenOfVerlengen(gav,
                                 groep,
                                 DateTime.Now.AddMonths(Properties.Settings.Default.MaandenGebruikersRechtStandaard));

            // transactie voor AD-account aanmaken en persisteren.

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (gav.ID == 0)
                {
                    // Als het om een nieuwe GAV gaat, vraag dan een AD-account aan, en bewaar
                    // de login

                    string username = ServiceHelper.CallService<IAdService, string>(
                        svc => svc.GapLoginAanvragen(adNr, persoon.VoorNaam, persoon.Naam, eMail));

                    gav.Login = String.Format(@"CHIROPUBLIC\{0}", username);
                }

                resultaat = _autorisatieDao.Bewaren(gebruikersRecht,
                                                              gr => gr.Gav.Persoon.First().WithoutUpdate(),
                                                              gr => gr.Groep.WithoutUpdate());
#if KIPDORP
                tx.Complete();
            }
#endif

            return resultaat;

        }

        /// <summary>
        /// Pas de vervaldatum van het gegeven <paramref name="gebruikersRecht"/> aan, zodanig dat
        /// het niet meer geldig is.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRecht">te vervallen gebruikersrecht</param>
        public void Intrekken(GebruikersRecht gebruikersRecht)
        {
            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersRecht.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            if (gebruikersRecht.VervalDatum < DateTime.Now)
            {
                throw new FoutNummerException(FoutNummer.GebruikersRechtWasAlVervallen, Properties.Resources.GebruikersRechtWasAlVervallen);
            }

            gebruikersRecht.VervalDatum = DateTime.Now.AddDays(-1);
        }

        /// <summary>
        /// Persisteert het gegeven <paramref name="gebruikersRecht"/>, zonder enige koppelingen
        /// </summary>
        /// <param name="gebruikersRecht">Te bewaren gebruikersrecht</param>
        public void Bewaren(GebruikersRecht gebruikersRecht)
        {
            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersRecht.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            _autorisatieDao.Bewaren(gebruikersRecht);
        }

        /// <summary>
        /// Haalt alle gebruikersrechten op uit de groep met ID <paramref name="groepID"/>, inclusief
        /// persoon.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan we de gebruikersrechten willen ophalen</param>
        /// <returns>alle gebruikersrechten uit de groep met ID <paramref name="groepID"/>, inclusief
        /// persoon.</returns>
        public IEnumerable<GebruikersRecht> AllesOphalen(int groepID)
        {
            if (!_autorisatieManager.IsGavGroep(groepID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            var resultaat = _autorisatieDao.AllesOphalen(groepID);

            return resultaat;
        }

        /// <summary>
        /// Haalt een gebruikersrecht op, gegeven zijn <paramref name="gebruikersRechtID"/>,
        /// zonder koppelingen.
        /// </summary>
        /// <param name="gebruikersRechtID">ID op te halen gebruikersrecht</param>
        /// <returns>Het gevraagde gebruikersrecht, zonder koppelingen</returns>
        public GebruikersRecht Ophalen(int gebruikersRechtID)
        {
            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersRechtID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            return _autorisatieDao.Ophalen(gebruikersRechtID);
        }

        /// <summary>
        /// Gegeven een <paramref name="groepID"/>, haal van de GAV's waarvan 
        /// de personen gekend zijn, de gelieerde personen op (die dus de
        /// koppeling bepalen tussen de persoon en de groep met gegeven
        /// <paramref name="groepID"/>).
        /// Voorlopig worden ook de communicatiemiddelen mee opgeleverd.
        /// </summary>
        /// <param name="groepID">ID van een groep</param>
        /// <returns>beschikbare gelieerde personen waarvan we weten dat ze GAV zijn
        /// voor die groep</returns>
        /// <remarks>Vervallen GAV's worden niet opgeleverd</remarks>
        public IEnumerable<GelieerdePersoon> GavGelieerdePersonenOphalen(int groepID)
        {
            if (!(_autorisatieManager.IsSuperGav() || _autorisatieManager.IsGavGroep(groepID)))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            var gavGps = _gelieerdePersonenDao.OphalenOpBasisVanGavs(groepID);

            return (from gp in gavGps
                    where
                        gp.Persoon.Gav.Any(
                            gav =>
                            gav.GebruikersRecht.Any(
                                gr =>
                                gr.Groep.ID == groepID && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now)))
                    select gp).ToArray();
        }

        /// <summary>
        /// Geeft de gegeven <paramref name="gav"/> gebruikersrechten op de groep van <paramref name="notificatieOntvanger"/>,
        /// en stuurt <paramref name="notificatieOntvanger"/> een mailtje dat jij dat gebruikersrecht hebt gekregen.
        /// (Dit is uiteraard enkel zinvol wanneer je supergav-rechten hebt, en ook gewone gebruikersrechten wil krijgen).
        /// PERSISTEERT, want GAV-maken en mailtje sturen moet in 1 transactie
        /// </summary>
        /// <param name="gav">GAV die gebruikersrecht moet krijgen, met daaraan gekoppeld de groepen waarop hij/zij
        /// al gebruikersrecht heeft.</param>
        /// <param name="notificatieOntvanger">De gelieerde persoon die de groep bepaalt de <paramref name="gav"/> 
        /// rechten voor moet krijgen, en die een mailtje zal krijgen waarin staat dat hij/zij rechten hebt gekregen.  
        /// </param>
        /// <param name="vervalDatum">Vervaldatum van het nieuwe gebruikersrecht.</param>
        /// <param name="reden">Reden waarom het gebruikersrecht aangevraagd werd.  Wordt mee gemaild naar de
        /// <paramref name="notificatieOntvanger"/></param>
        /// <returns>het gecreeerde gebruikersrecht</returns>
        /// <remarks>Als de gebruiker al een gebruikersrecht heeft, wordt enkel de vervaldatum aangepast</remarks>
        public GebruikersRecht ToekennenOfVerlengen(Gav gav, GelieerdePersoon notificatieOntvanger, DateTime vervalDatum, string reden)
        {
            if (!_autorisatieManager.IsSuperGav())
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            string mailAdres = GelieerdePersonenManager.EMailKiezen(notificatieOntvanger);

            if (String.IsNullOrEmpty(mailAdres))
            {
                throw new FoutNummerException(FoutNummer.EMailVerplicht, Properties.Resources.EMailVerplicht);
            }

            var gebruikersRecht = ToekennenOfVerlengen(gav, notificatieOntvanger.Groep, vervalDatum);

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                gebruikersRecht = _autorisatieDao.Bewaren(gebruikersRecht,
                                                      gr => gr.Gav.Persoon.First().WithoutUpdate(),
                                                      gr => gr.Groep.WithoutUpdate());
                _mailer.Verzenden(mailAdres,
                              Properties.Resources.TijdelijkeRechtenMailOnderwerp,
                              String.Format(
                                Properties.Resources.TijdelijkeRechtenMailBody, 
                                notificatieOntvanger.Persoon.VolledigeNaam, 
                                gav.Login, 
                                vervalDatum, 
                                reden,
                                notificatieOntvanger.Groep.ID));

                
#if KIPDORP
                tx.Complete();
            }
#endif
            return gebruikersRecht;
        }

        /// <summary>
        /// Koppelt de <paramref name="gav"/> aan de <paramref name="groep"/> met gegeven 
        /// <paramref name="vervalDatum"/>.  PERSISTEERT NIET.
        /// </summary>
        /// <param name="gav">Te koppelen GAV</param>
        /// <param name="groep">groep waaraan te koppelen</param>
        /// <param name="vervalDatum">vervaldatum gebruikersrecht</param>
        /// <returns>Deze method is PRIVATE en moet dat ook blijven, want er wordt niet gecheckt
        /// op fouten, en er worden geen notificatiemails gestuurd.  Deze method mag enkel
        /// onrechtstreeks gebruikt worden, via de publieke methods <see name="ToekennenOfVerlengen"/>
        /// </returns>
        private GebruikersRecht ToekennenOfVerlengen(Gav gav, Groep groep, DateTime vervalDatum)
        {
            // Eerst controleren of de groep nog niet aan de gebruiker is gekoppeld

            var gebruikersrecht = (from gr in gav.GebruikersRecht
                                   where gr.Groep.ID == groep.ID
                                   select gr).FirstOrDefault();

            if (gebruikersrecht == null)
            {
                // Nog geen gebruikersrecht.  Maak aan.

                gebruikersrecht = new GebruikersRecht {ID = 0, Gav = gav, Groep = groep};
                gav.GebruikersRecht.Add(gebruikersrecht);
                groep.GebruikersRecht.Add(gebruikersrecht);
            }
            else if (!IsVerlengbaar(gebruikersrecht))
            {
                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar, Properties.Resources.GebruikersRechtNietVerlengbaar);
            }

            gebruikersrecht.VervalDatum = vervalDatum;

            return gebruikersrecht;
        }

        /// <summary>
        /// Haalt de GAV op voor de gebruiker die momenteel is aangemeld.  Als er zo nog geen bestaat, wordt
        /// die aangemaakt.
        /// </summary>
        /// <returns>GAV, met eventueel gekoppelde groepen</returns>
        public Gav MijnGavOphalen()
        {
            string login = _autorisatieManager.GebruikersNaamGet();

            var gav = _gavDao.Ophalen(login) ?? new Gav {ID = 0, Login = login};

            return gav;
        }
    }
}
