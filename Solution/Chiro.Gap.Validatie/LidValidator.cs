using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Validatie
{
    public class LidValidator: Validator<Lid>
    {
        /// <summary>
        /// Controleert de geldigheid van het lid <paramref name="teValideren"/>
        /// </summary>
        /// <param name="teValideren">te valideren lid</param>
        /// <returns><c>null</c> als het lid OK is, anders een foutnummer</returns>
        public override FoutNummer? FoutNummer(Lid teValideren)
        {
            if (teValideren.GelieerdePersoon.Persoon.GeboorteDatum == null)
            {
                return Domain.FoutNummer.GeboorteDatumOntbreekt;
            }

            Debug.Assert(teValideren.GelieerdePersoon.GebDatumMetChiroLeefTijd != null);

            if (!Equals(teValideren.GelieerdePersoon.Groep, teValideren.GroepsWerkJaar.Groep))
            {
                return Domain.FoutNummer.GroepsWerkJaarNietVanGroep;
            }

            if (teValideren.GroepsWerkJaar.WerkJaar - teValideren.GelieerdePersoon.GebDatumMetChiroLeefTijd.Value.Year <
                Properties.Settings.Default.MinimumLeeftijd)
            {
                return Domain.FoutNummer.LidTeJong;
            }

            if (teValideren.GelieerdePersoon.Persoon.SterfDatum != null &&
                teValideren.GroepsWerkJaar.WerkJaar > teValideren.GelieerdePersoon.Persoon.SterfDatum.Value.Year)
            {
                return Domain.FoutNummer.PersoonOverleden;
            }

            return null;
        }

        /// <summary>
        /// Property die de minimumleeftijd van een lid oproept.
        /// Een lid moet gedurende het eerste kalenderjaar van het werkjaar de minimumleeftijd hebben.
        /// </summary>
        public int MinimumLeeftijd
        {
            get { return Properties.Settings.Default.MinimumLeeftijd; }
        }
    }
}
