// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Diagnostics;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Service voor operaties op groepsniveau
    /// </summary>
    public class AdressenService : IAdressenService, IDisposable
    {
        /// <summary>
        /// _context is verantwoordelijk voor het tracken van de wijzigingen aan de
        /// entiteiten. Via _context.SaveChanges() kunnen wijzigingen gepersisteerd
        /// worden.
        /// 
        /// Context is IDisposable. De context wordt aangemaakt door de IOC-container,
        /// en gedisposed op het moment dat de service gedisposed wordt. Dit gebeurt
        /// na iedere call.
        /// </summary>
        private readonly IContext _context;

        // Repositories, verantwoordelijk voor data access.

        private readonly IRepository<Adres> _adressenRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IVeelGebruikt _veelGebruikt;

        /// <summary>
        /// Nieuwe Adressenservice
        /// </summary>
        /// <param name="veelGebruikt">Cache voor veelgebruikte zaken</param>
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        public AdressenService(IVeelGebruikt veelGebruikt,
                               IRepositoryProvider repositoryProvider)
        {
            _context = repositoryProvider.ContextGet();
            _adressenRepo = repositoryProvider.RepositoryGet<Adres>();
            _veelGebruikt = veelGebruikt;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Haalt adres op, op basis van de adresgegevens
        /// </summary>
        /// <param name="adresInfo">De adresgegevens</param>
        /// <returns>Gevraagd adresobject</returns>
        public Adres Ophalen(AdresInfo adresInfo)
        {
            Adres resultaat;

            using (var db = new ChiroGroepEntities())
            {
                db.Adres.AsNoTracking();

                if (String.IsNullOrEmpty(adresInfo.LandNaam) ||
                    String.Compare(adresInfo.LandNaam, Properties.Resources.Belgie, true) == 0)
                {
                    var adressentabel = db.Adres.OfType<BelgischAdres>();

                    resultaat = (
                                    from a in adressentabel
                                    where (a.StraatNaam.Naam == adresInfo.StraatNaamNaam && a.StraatNaam.PostNummer == adresInfo.PostNr
                                           && a.WoonPlaats.Naam == adresInfo.WoonPlaatsNaam && a.WoonPlaats.PostNummer == adresInfo.PostNr
                                           && (a.HuisNr == null && adresInfo.HuisNr == null || a.HuisNr == adresInfo.HuisNr)
                                           && (a.Bus == null && adresInfo.Bus == null || a.Bus == adresInfo.Bus))
                                    select a).FirstOrDefault();

                    // Gekke constructie voor huisnummer, bus en postcode, omdat null anders niet goed
                    // opgevangen wordt.  (je krijgt bijv. where PostCode == null in de SQL query, wat niet werkt)
                }
                else
                {
                    var adressentabel = db.Adres.OfType<BuitenLandsAdres>();

                    resultaat = (
                            from a in adressentabel
                            where (a.Straat == adresInfo.StraatNaamNaam
                                   && a.WoonPlaats == adresInfo.WoonPlaatsNaam
                                   && a.PostNummer == adresInfo.PostNr
                                   && a.PostCode == adresInfo.PostCode
                                   && (a.HuisNr == null && adresInfo.HuisNr == null || a.HuisNr == adresInfo.HuisNr)
                                   && (a.Bus == null && adresInfo.Bus == null || a.Bus == adresInfo.Bus)
                                   && (a.Land.Naam == adresInfo.LandNaam))
                            select a).FirstOrDefault();

                    // Gekke constructie voor huisnummer, bus en postcode, omdat null anders niet goed
                    // opgevangen wordt.  (je krijgt bijv. where PostCode == null in de SQL query, wat niet werkt)
                }
            }

            return resultaat;
        }
    }
}
