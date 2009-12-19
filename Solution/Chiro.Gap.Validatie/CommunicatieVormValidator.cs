using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Chiro.Gap.Orm;
using Chiro.Gap.Data.Ef;

namespace Chiro.Gap.Validatie
{
    public class CommunicatieVormValidator : Validator<CommunicatieVorm>
    {
        /// <summary>
        /// Vergelijkt de opgegeven waarde met de regex die in de databank opgegeven is
        /// voor dat communicatieType
        /// </summary>
        /// <param name="cv">De communicatievorm (bv. telefoonnummer, mailadres, ...</param>
        /// <returns>
        /// <c>true</c> als de waarde ('Nummer') voldoet aan de opgegeven Regex voor dat communicatietype,
        /// en anders <c>false</c>
        /// </returns>
        public override bool Valideer(CommunicatieVorm cv)
        {
            // FIXME: cv.CommunicatieType.Validatie moet geladen worden
            return Regex.IsMatch(cv.Nummer, cv.CommunicatieType.Validatie);
        }
    }
}
