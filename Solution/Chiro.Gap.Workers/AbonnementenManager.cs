// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

#if KIPDORP
using System.Transactions;
#endif
using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Businesslogica wat abonnementen betreft
    /// </summary>
    public class AbonnementenManager
    {
        private readonly IDao<Publicatie> _pubDao;
        private readonly IAbonnementenDao _abDao;
        private readonly IAutorisatieManager _auMgr;
        private readonly IDubbelpuntSync _dubbelpuntSync;

        /// <summary>
        /// Constructor, die zorg draagt voor dependency injection
        /// </summary>
        /// <param name="pubDao">
        /// Data Access Object voor publicaties
        /// </param>
        /// <param name="abDao">
        /// Data Access Object voor abonnement
        /// </param>
        /// <param name="auMgr">
        /// Neemt de autorisatie voor zijn rekening
        /// </param>
        /// <param name="dpSync">
        /// Sync voor Dubbelpuntabonnementen
        /// </param>
        public AbonnementenManager(
            IDao<Publicatie> pubDao,
            IAbonnementenDao abDao,
            IAutorisatieManager auMgr,
            IDubbelpuntSync dpSync)
        {
            _pubDao = pubDao;
            _abDao = abDao;
            _auMgr = auMgr;

            // Momenteel hebben we enkel nog dubbelpunt.  Als er ooit meerdere publicaties komen waarop je je kunt
            // abonneren (bijv. de nieuwsbrieven?), moet dit generieker.
            _dubbelpuntSync = dpSync;
        }

        /// <summary>
        /// Haalt een publicatie op, gegeven zijn <paramref name="publicatieID"/>
        /// </summary>
        /// <param name="publicatieID">
        /// Bepaalt op te halen publicatie
        /// </param>
        /// <returns>
        /// De gevraagde publicatie
        /// </returns>
        public Publicatie PublicatieOphalen(PublicatieID publicatieID)
        {
            return _pubDao.Ophalen((int)publicatieID);
        }

        /// <summary>
        /// Creëert een abonnement voor de gelieerde persoon <paramref name="gp"/> op publicatie
        /// <paramref name="publicatie"/> in het groepswerkjaar <paramref name="groepsWerkJaar"/>.
        /// </summary>
        /// <param name="publicatie">
        /// Publicatie waarvoor abonnement aangevraagd wordt
        /// </param>
        /// <param name="gp">
        /// Gelieerde persoon die abonnement moet krijgen
        /// </param>
        /// <param name="groepsWerkJaar">
        /// Groepswerkjaar waarvoor abonnement aangevraagd wordt
        /// </param>
        /// <returns>
        /// Het aangevraagde abonnement
        /// </returns>
        /// <exception cref="BlokkerendeObjectenException{TEntiteit}">
        /// Komt voor als de <paramref name="gp"/> voor het opgegeven <paramref name="groepsWerkJaar"/> al een
        /// abonnement heeft op die <paramref name="publicatie"/>.
        /// </exception>
        /// <exception cref="FoutNummerException">
        /// Komt voor als de publicatie niet meer uitgegeven wordt en je dus geen abonnement meer kunt aanvragen,
        /// als de bestelperiode voorbij is, of als de <paramref name="gp"/> geen adres heeft waar we de publicatie 
        /// naar kunnen opsturen.
        /// </exception>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV is voor de groep waar het <paramref name="groepsWerkJaar"/>
        /// aan gekoppeld is.
        /// </exception>
        public Abonnement Abonneren(Publicatie publicatie, GelieerdePersoon gp, GroepsWerkJaar groepsWerkJaar)
        {
            // TODO (#767) nakijken of het nog wel de moeite loont om een abonnement te noemen voor het gegeven werkjaar.
            // (in augustus is dat bijvoorbeeld minder interessant)
            if (!publicatie.Actief)
            {
                throw new FoutNummerException(FoutNummer.PublicatieInactief,
                                              string.Format(
                                                  Resources.PublicatieInactief,
                                                  publicatie.Naam));
            }

            if (!_auMgr.IsGavGelieerdePersoon(gp.ID) || !_auMgr.IsGavGroepsWerkJaar(groepsWerkJaar.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (gp.PersoonsAdres == null)
            {
                throw new FoutNummerException(FoutNummer.AdresOntbreekt,
                                              string.Format(
                                                  Resources.DubbelPuntZonderAdres,
                                                  gp.Persoon.VolledigeNaam));
            }

            var eindeBestelPeriode = new DateTime(
                groepsWerkJaar.WerkJaar + 1,
                Settings.Default.EindeDubbelPuntBestelling.Month,
                Settings.Default.EindeDubbelPuntBestelling.Day);

            if (DateTime.Now > eindeBestelPeriode)
            {
                throw new FoutNummerException(
                    FoutNummer.BestelPeriodeDubbelpuntVoorbij,
                    string.Format(
                        Resources.BestelPeriodeDubbelpuntVoorbij,
                        groepsWerkJaar.WerkJaar,
                        groepsWerkJaar.WerkJaar + 1));
            }

            // Check of abonnement nog niet bestaat
            var bestaande = (from abo in gp.Abonnement
                             where abo.GroepsWerkJaar.ID == groepsWerkJaar.ID && abo.Publicatie.ID == publicatie.ID
                             select abo).FirstOrDefault();

            if (bestaande != null)
            {
                throw new BlokkerendeObjectenException<Abonnement>(new[] { bestaande },
                                                                   1,
                                                                   string.Format(
                                                                       Resources.BestaandAbonnement,
                                                                       gp.Persoon.VolledigeNaam,
                                                                       publicatie.Naam));
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
        /// <param name="abonnement">
        /// Te persisteren abonnement
        /// </param>
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

        /// <summary>
        /// Haalt alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </summary>
        /// <param name="gwjID">
        /// ID van het gegeven groepswerkjaar
        /// </param>
        /// <returns>
        /// Alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </returns>
        public IEnumerable<Abonnement> OphalenUitGroepsWerkJaar(int gwjID)
        {
            if (_auMgr.IsSuperGav() || _auMgr.IsGavGroepsWerkJaar(gwjID))
            {
                return _abDao.OphalenUitGroepsWerkJaar(gwjID);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }
    }
}