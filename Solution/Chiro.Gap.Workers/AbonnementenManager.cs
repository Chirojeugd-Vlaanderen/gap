using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Businesslogica wat betreft abonnementen
    /// </summary>
    public class AbonnementenManager
    {
        private readonly IDao<Publicatie> _pubDao;
        private readonly IDao<Abonnement> _abDao;
        private readonly IAutorisatieManager _auMgr;
        private readonly IDubbelpuntSync _dubbelpuntSync;

        /// <summary>
        /// Constructor, die zorg draagt voor dependency injection
        /// </summary>
        /// <param name="pubDao">Data Access Object voor publicaties</param>
        /// <param name="abDao">Data Access Object voor abonnement</param>
        /// <param name="auMgr">neemt de autorisatie voor zijn rekening</param>
        /// <param name="dpSync">sync voor dubbelpuntabonnementen</param>
        public AbonnementenManager(
            IDao<Publicatie> pubDao,
            IDao<Abonnement> abDao,
            IAutorisatieManager auMgr,
            IDubbelpuntSync dpSync)
        {
            _pubDao = pubDao;
            _abDao = abDao;
            _auMgr = auMgr;

            // Momenteel hebben we enkel nog dubbelpunt.  Als er ooit meerdere publicaties komen waarop je je kunt
            // abonneren (bijv. de nieuwsbrieven?) , moet dit generieker.

            _dubbelpuntSync = dpSync;
        }

        /// <summary>
        /// Haalt een publicatie op, gegeven zijn <paramref name="publicatieID"/>
        /// </summary>
        /// <param name="publicatieID">bepaalt op te halen publicatie</param>
        /// <returns>De gevraagde publicatie</returns>
        public Publicatie PublicatieOphalen(PublicatieID publicatieID)
        {
            return _pubDao.Ophalen((int)publicatieID);
        }

        /// <summary>
        /// Creëert een abonnement voor de gelieerde persoon <paramref name="gp"/> op publicatie
        /// <paramref name="publicatie"/> in het groepswerkjaar <paramref name="groepsWerkJaar"/>.
        /// </summary>
        /// <param name="publicatie">Publicatie waarvoor abonnement te maken</param>
        /// <param name="gp">Gelieerde persoon die abonnement moet krijgen</param>
        /// <param name="groepsWerkJaar">Groepswerkjaar waarvoor abonnement</param>
        /// <returns></returns>
        public Abonnement Abonneren(Publicatie publicatie, GelieerdePersoon gp, GroepsWerkJaar groepsWerkJaar)
        {
            // TODO (#767) nakijken of het nog wel de moeite loont om een abonnement te noemen voor het gegeven werkjaar.
            // (in augustus is dat bijvoorbeeld minder interessant)

            if (!publicatie.Actief)
            {
                throw new FoutNummerException(FoutNummer.PublicatieInactief, String.Format(
                    Properties.Resources.PublicatieInactief,
                    publicatie.Naam));
            }

            if (!_auMgr.IsGavGelieerdePersoon(gp.ID) || !_auMgr.IsGavGroepsWerkJaar(groepsWerkJaar.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            if (gp.PersoonsAdres == null)
            {
                throw new FoutNummerException(FoutNummer.AdresOntbreekt, String.Format(
                    Properties.Resources.DubbelPuntZonderAdres,
                    gp.Persoon.VolledigeNaam));
            }

            DateTime eindeBestelPeriode = new DateTime(
                groepsWerkJaar.WerkJaar+1,
                Properties.Settings.Default.EindeDubbelPuntBestelling.Month,
                Properties.Settings.Default.EindeDubbelPuntBestelling.Day);

            if (DateTime.Now > eindeBestelPeriode)
            {
                throw new FoutNummerException(
                    FoutNummer.BestelPeriodeDubbelpuntVoorbij, 
                    String.Format(
                        Properties.Resources.BestelPeriodeDubbelpuntVoorbij, 
                        groepsWerkJaar.WerkJaar, 
                        groepsWerkJaar.WerkJaar+1));
            }

            // Check of abonnement nog niet bestaat

            var bestaande = (from abo in gp.Abonnement
                             where abo.GroepsWerkJaar.ID == groepsWerkJaar.ID && abo.Publicatie.ID == publicatie.ID
                             select abo).FirstOrDefault();

            if (bestaande != null)
            {
                throw new BlokkerendeObjectenException<Abonnement>(new[] { bestaande }, 1, String.Format(
                    Properties.Resources.BestaandAbonnement,
                    gp.Persoon.VolledigeNaam, publicatie.Naam));
            }

            // Alles OK, nu het echte werk:

            var abonnement = new Abonnement
                          {
                              GelieerdePersoon = gp,
                              AanvraagDatum = DateTime.Now,
                              GroepsWerkJaar = groepsWerkJaar,
                              Publicatie = publicatie
                          };

            gp.Abonnement.Add(abonnement);
            groepsWerkJaar.Abonnement.Add(abonnement);
            gp.Abonnement.Add(abonnement);

            return abonnement;
        }

        /// <summary>
        /// Persisteert een abonnement
        /// </summary>
        /// <param name="abonnement">Te persisteren abonnement</param>
        public void Bewaren(Abonnement abonnement)
        {

#if KIPDORP
					using (var tx = new TransactionScope())
					{
#endif
            _abDao.Bewaren(abonnement, ab => ab.GelieerdePersoon, ab => ab.GroepsWerkJaar, ab => ab.Publicatie);
            if (abonnement.Publicatie.ID == (int)PublicatieID.Dubbelpunt)
            {
                _dubbelpuntSync.Abonneren(abonnement);
            }
#if KIPDORP
						tx.Complete();
					}
#endif
        }
    }
}
