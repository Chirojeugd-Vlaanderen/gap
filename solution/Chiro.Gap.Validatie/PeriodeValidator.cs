using Chiro.Gap.Domain;

namespace Chiro.Gap.Validatie
{
    /// <summary>
    /// Klasse die een periode valideert. Ihb: is er een begin- en einddatum, en 
    /// ligt de begindatum voor de einddatum :)
    /// </summary>
    public class PeriodeValidator: Validator<IPeriode>
    {
        /// <summary>
        /// Valideert de periode <paramref name="teValideren"/>
        /// </summary>
        /// <param name="teValideren">te valideren periode</param>
        /// <returns><c>true</c> als de periode aan de regels voldoet, anders <c>false</c></returns>
        public override bool Valideer(IPeriode teValideren)
        {
            return teValideren.DatumVan != null && teValideren.DatumTot != null &&
                   teValideren.DatumVan <= teValideren.DatumTot;
        }
    }
}
