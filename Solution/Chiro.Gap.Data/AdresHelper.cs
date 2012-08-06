// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Gap.Orm;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Aparte klasse die gebruikt wordt om straat/woonplaats/land te koppelen
    /// aan adressen die geattacht zijn aan een objectcontext.
    /// <para />
    /// Dit is altijd wat prutswerk, o.w.v. het onderscheid tussen Belgische en
    /// buitenlandse adressen.
    /// </summary>
    internal static class AdresHelper
    {
        /// <summary>
        /// Koppelt de straat/gemeente/land van de voorkeursadressen aan een (geattachte!) gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersoon">De gelieerde persoon waar het over gaat</param>
        /// <returns>Dezelfde gelieerde persoon, maar met voorkeursadressen</returns>
        /// <remarks>Het adresobject van het voorkeursadres moeten al gekoppeld zijn; 
        /// deze method instantieert enkel nog straat, gemeente en land.</remarks>
        public static GelieerdePersoon VoorkeursAdresKoppelen(GelieerdePersoon gelieerdePersoon)
        {
            if (gelieerdePersoon == null || gelieerdePersoon.PersoonsAdres == null)
            {
                return gelieerdePersoon;
            }
            else
            {
                return Enumerable.First(VoorkeursAdresKoppelen(new GelieerdePersoon[] { gelieerdePersoon }));
            }
        }

        /// <summary>
        /// Koppelt de straat/gemeente/land van de voorkeursadressen aan een lijst (geattachte!) gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersonen">Gelieerde personen</param>
        /// <returns>Dezelfde gelieerde personen, maar met voorkeursadressen</returns>
        /// <remarks>De adresobjecten moeten al gekoppeld zijn; deze method instantieert enkel nog straat, gemeente en land.</remarks>
        public static IEnumerable<GelieerdePersoon> VoorkeursAdresKoppelen(IEnumerable<GelieerdePersoon> gelieerdePersonen)
        {
            // De truuk is gewoon de informatie van de adressen te instantiëren, als Belgisch of
            // buitenlands adres.
            // Entity framework zal ervoor zorgen dat de extra info aan de reeds geladen adresobjecten
            // wordt gekoppeld.

            var alleGps = gelieerdePersonen.ToArray();

            foreach (var gp in alleGps)
            {
                gp.PersoonsAdresReference.Load();

                if (gp.PersoonsAdres != null)
                {
                    gp.PersoonsAdres.AdresReference.Load();
                    var adr = gp.PersoonsAdres.Adres;
                    if (adr is BelgischAdres)
                    {
                        (adr as BelgischAdres).StraatNaamReference.Load();
                        (adr as BelgischAdres).WoonPlaatsReference.Load();
                    }
                    else
                    {
                        Debug.Assert(adr is BuitenLandsAdres);
                        ((BuitenLandsAdres) adr).LandReference.Load();
                    }
                }
            }

            return alleGps;
        }

        /// <summary>
        /// Koppelt de straat/gemeente/land van de adressen aan een (geattachte!) gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersoon">De gelieerde persoon waar het over gaat</param>
        /// <returns>Dezelfde gelieerde persoon, maar met alle adressen gekoppeld</returns>
        /// <remarks>Het adresobject van het voorkeursadres moeten al gekoppeld zijn; 
        /// deze method instantieert enkel nog straat, gemeente en land.</remarks>
        public static GelieerdePersoon AlleAdressenKoppelen(GelieerdePersoon gelieerdePersoon)
        {
            return Enumerable.First(AlleAdressenKoppelen(new GelieerdePersoon[] { gelieerdePersoon }));
        }

        /// <summary>
        /// Koppelt straat/gemeente/land van alle adressen aan een lijst (geattachte!) gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersonen">Gelieerde personen</param>
        /// <returns>Dezelfde gelieerde personen, maar met alle adressen</returns>
        /// <remarks>De adresobjecten moeten al gekoppeld zijn; deze method instantieert enkel nog straat, gemeente en land.</remarks>
        public static IEnumerable<GelieerdePersoon> AlleAdressenKoppelen(IEnumerable<GelieerdePersoon> gelieerdePersonen)
        {
            // De truuk is gewoon de informatie van de adressen te instantiëren: eerst die van de 
            // Belgische, dan die van de buitenlandse.
            // Entity framework zal ervoor zorgen dat de extra info aan de reeds geladen adresobjecten
            // wordt gekoppeld.

            var alleAdressen = gelieerdePersonen.SelectMany(gp => gp.Persoon.PersoonsAdres).Select(pa => pa.Adres);

            foreach (var adr in alleAdressen)
            {
                if (adr is BelgischAdres)
                {
                    (adr as BelgischAdres).StraatNaamReference.Load();
                    (adr as BelgischAdres).WoonPlaatsReference.Load();
                }
                else
                {
                    Debug.Assert(adr is BuitenLandsAdres);
                    ((BuitenLandsAdres)adr).LandReference.Load();
                }
            }
            return gelieerdePersonen;
        }

        /// <summary>
        /// Koppelt straat/woonplaats/land aan een adres
        /// </summary>
        /// <param name="adres">Belgisch of buitenlands adres, gekoppeld aan een bestaande
        /// objectcontext.</param>
        public static void AdresGegevensKoppelen(Adres adres)
        {
            // Dat deze method static is, is erg tricky.  Ze kan aangeroepen worden van overal
            // in Chiro.Gap.Data, maar niet daarbuiten (zoals bijv. Chiro.Gap.Workers).  Buiten
            // deze assembly worden de DAO's namelijk met dependency injection geconstrueerd.
            //
            // Bijgevolg is het ook niet de bedoeling dat deze method in de interface terecht komt.

            if (adres is BelgischAdres)
            {
                ((BelgischAdres)adres).StraatNaamReference.Load();
                ((BelgischAdres)adres).WoonPlaatsReference.Load();
            }
            else if (adres is BuitenLandsAdres)
            {
                ((BuitenLandsAdres)adres).LandReference.Load();
            }
        }
    }
}
