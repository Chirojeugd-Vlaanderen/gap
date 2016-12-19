/*
   Copyright 2015-2016 Chirojeugd-Vlaanderen vzw

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
using System.Linq;
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Verwijdert een persoon met gekend AD-nummer als actief lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer te verwijderen lid
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer te verwijderen lid
        /// </param>
        /// <param name="uitschrijfDatum"> uitschrijfdatum zoals geregistreerd in GAP</param>
        /// <remarks>
        /// Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LidVerwijderen(int adNummer, string stamNummer, DateTime uitschrijfDatum)
        {
            int? civiGroepId = _contactWorker.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet verwijderd.", stamNummer),
                    stamNummer, adNummer, null);
                return;
            }

            var contact = _contactWorker.PersoonMetActiefLid(adNummer, civiGroepId);

            if (contact == null || contact.ExternalIdentifier == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te verwijderen lid - als dusdanig terug naar GAP.", adNummer),
                    stamNummer, adNummer, null);
                _gapUpdateClient.OngeldigAdNaarGap(adNummer);
                return;
            }

            if (contact.RelationshipResult.Count >= 1)
            {
                if (contact.RelationshipResult.Count > 1)
                {
                    _log.Loggen(Niveau.Warning,
                        String.Format(
                            "Meer dan 1 actieve lidrelatie voor {0} {1} (AD {2}) in groep {3}.",
                            contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer),
                        stamNummer, adNummer, contact.GapId);

                }
                var relatie = contact.RelationshipResult.Values.First();

                _log.Loggen(Niveau.Info,
                    String.Format(
                        "Lid {0} {1} (AD {2}) verwijderd voor groep {3}; startdatum was {4:dd/MM/yyyy}.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer, relatie.StartDate),
                    stamNummer, adNummer, contact.GapId);
                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc =>
                        svc.RelationshipDelete(_apiKey, _siteKey,
                            new DeleteRequest(relatie.Id)));
                result.AssertValid();
            }
            else
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "Lid {0} {1} (AD {2}) niet verwijderd voor groep {3} - geen actieve relatie gevonden.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer),
                    stamNummer, adNummer, contact.GapId);
            }
        }

        /// <summary>
        /// Verwijdert een actief lid als het ad-nummer om een of andere reden niet bekend is.
        /// </summary>
        /// <param name="details">
        /// Gegevens die hopelijk toelaten het lid te identificeren
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van het lid
        /// </param>
        /// <param name="uitschrijfDatum">uitschrijfdatum zoals geregistreerd in GAP</param>
        /// <remarks>
        /// Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, DateTime uitschrijfDatum)
        {
            if (details.Persoon.AdNummer != null)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "NieuwLidVerwijderen aangeroepen voor persoon {0} {1} (gid {3}) met bestaand AD-Nummer {2}."
                        , details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.AdNummer, details.Persoon.ID),
                    stamNummer, details.Persoon.AdNummer, details.Persoon.ID);
            }

            int? adNummer = _contactWorker.AdNummerZoeken(details);

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
                LidVerwijderen(adNummer.Value, stamNummer, uitschrijfDatum);
            }
        }
    }
}