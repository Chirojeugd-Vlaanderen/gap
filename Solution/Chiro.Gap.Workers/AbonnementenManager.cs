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
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Businesslogica wat abonnementen betreft
    /// </summary>
    public class AbonnementenManager
    {
        private readonly IAutorisatieManager _auMgr;
        private readonly IDubbelpuntSync _dubbelpuntSync;

        public AbonnementenManager(
            IAutorisatieManager auMgr,
            IDubbelpuntSync dpSync)
        {
            _auMgr = auMgr;

            // Momenteel hebben we enkel nog dubbelpunt.  Als er ooit meerdere publicaties komen waarop je je kunt
            // abonneren (bijv. de nieuwsbrieven?), moet dit generieker.
            _dubbelpuntSync = dpSync;
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
            // TODO (#767) nakijken of het nog wel de moeite loont om een abonnement te noemen voor het gegeven werkJaar.
            // (in augustus is dat bijvoorbeeld minder interessant)
            if (!publicatie.Actief)
            {
                throw new FoutNummerException(FoutNummer.PublicatieInactief,
                                              string.Format(
                                                  Resources.PublicatieInactief,
                                                  publicatie.Naam));
            }

            if (!_auMgr.IsGav(gp) || !_auMgr.IsGav(groepsWerkJaar))
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

    }
}