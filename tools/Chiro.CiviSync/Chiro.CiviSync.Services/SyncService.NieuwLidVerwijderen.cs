/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Verwijdert een lid als het ad-nummer om een of andere reden niet bekend is.
        /// </summary>
        /// <param name="details">
        /// Gegevens die hopelijk toelaten het lid te identificeren
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van het lid
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar van het lid
        /// </param>
        /// <param name="uitschrijfDatum">uitschrijfdatum zoals geregistreerd in GAP</param>
        /// <remarks>
        /// Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkJaar, DateTime uitschrijfDatum)
        {
            if (details.Persoon.AdNummer != null)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "NieuwLidVerwijderen aangeroepen voor persoon {0} {1} (gid {3}) met bestaand AD-Nummer {2}."
                        , details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.AdNummer, details.Persoon.ID),
                    stamNummer, details.Persoon.AdNummer, details.Persoon.ID);
            }

            int? adNummer = AdNummerZoeken(details);

            if (adNummer == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format(
                        "NieuwLidVerwijderen aangeroepen voor niet-gematchte persoon {0} {1} (gid {2}) in groep {3}."
                        , details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.ID, stamNummer),
                    stamNummer, details.Persoon.AdNummer, details.Persoon.ID);                
            }
            else
            {
                LidVerwijderen(adNummer.Value, stamNummer, werkJaar, uitschrijfDatum);
            }
        }
    }
}