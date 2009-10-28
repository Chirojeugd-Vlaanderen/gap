using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Chiro.Cdf.EfWrapper
{
    /// <summary>
    /// Basisentiteit.  De bedoeling is dat al onze entity's
    /// deze interface implementeren.
    /// </summary>
    public interface IBasisEntiteit: IEntityWithKey
    {
        /// <summary>
        /// Iedere entity wordt geidentificeerd door een
        /// integer.
        /// </summary>
        int ID { get; set; }
        /// <summary>
        /// Timestamp die het object in de database heeft.
        /// De bedoeling is niet dat de gebruiker hier iets
        /// mee doet, de timestamp wordt enkel gebruikt om
        /// aan concurrency control te doen.
        /// </summary>
        byte[] Versie { get; set; }
        /// <summary>
        /// Geeft stringrepresentatie van Versie weer (hex).
        /// Nodig om versie te bewaren in MVC view.
        /// </summary>
        string VersieString { get; set; }
        /// <summary>
        /// Deze property geeft aan of de entity verwijderd
        /// moet worden bij terugsturen naar de service.
        /// </summary>
        bool TeVerwijderen { get; set; }
    }
}
