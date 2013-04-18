using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Validatie
{
    /// <summary>
    /// Validator voor afdelingsjaren
    /// </summary>
    public class AfdelingsJaarValidator: Validator<AfdelingsJaar>
    {
        /// <summary>
        /// Controleert de geldigheid van het afdelingsjaar <paramref name="teValideren"/>
        /// </summary>
        /// <param name="teValideren">te valideren afdelingsjaar</param>
        /// <returns><c>null</c> als het afdelingsjaar OK is, anders een foutnummer</returns>
        public override FoutNummer? FoutNummer(AfdelingsJaar teValideren)
        {
            FoutNummer? resultaat = new PeriodeValidator().FoutNummer(teValideren);

            if (resultaat != null)
            {
                return resultaat;
            }

            if (teValideren.GroepsWerkJaar.Groep.ID != teValideren.Afdeling.ChiroGroep.ID)
            {
                return Domain.FoutNummer.AfdelingNietVanGroep;
            }

            if (teValideren.GroepsWerkJaar.WerkJaar - teValideren.GeboorteJaarTot < Properties.Settings.Default.MinimumLeeftijd)
            {
                return Domain.FoutNummer.OngeldigeGeboorteJarenVoorAfdeling;
            }

            if (teValideren.Geslacht == GeslachtsType.Onbekend)
            {
                return Domain.FoutNummer.OnbekendGeslachtFout;
            }

            return null;
        }
    }
}
