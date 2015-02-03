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
using System.Linq;
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.EntityRequests;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer van het opgegeven lid
        /// </param>
        /// <param name="gedoe">
        /// De nodige info voor het lid.
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LidBewaren(int adNummer, LidGedoe gedoe)
        {
            int? civiGroepId = _contactHelper.ContactIdGet(gedoe.StamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet bewaard.", gedoe.StamNummer),
                    gedoe.StamNummer, adNummer, null);
                return;
            }

            // Haal de persoon op met gegeven AD-nummer en zijn recentste lidrelatie in de gevraagde groep.
            var contactRequest = new ContactRequest
            {
                ExternalIdentifier = adNummer.ToString(),
                RelationshipGetRequest = new RelationshipRequest
                {
                    RelationshipTypeId = (int)RelatieType.LidVan,
                    ContactIdAValueExpression = "$value.id",
                    ContactIdB = civiGroepId,
                    ApiOptions = new ApiOptions{Sort = "start_date DESC", Limit = 1}
                }
            };

            var contact =
                ServiceHelper.CallService<ICiviCrmApi, Contact>(
                    svc => svc.ContactGetSingle(_apiKey, _siteKey, contactRequest));

            if (contact == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} - lid niet bewaard.", adNummer),
                    gedoe.StamNummer, adNummer, null);
                return;
            }

            // Request voor lidrelatie: eerst een standaardrequest voor dit werkjaar, we passen dat zo nodig
            // aan.
            var relationshipRequest = _relationshipHelper.VanWerkjaar(RelatieType.LidVan, contact.Id, civiGroepId.Value,
                gedoe.WerkJaar);

            if (contact.RelationshipResult.Count == 1)
            {
                var bestaandeRelatie = contact.RelationshipResult.Values.First();
                if (_relationshipHelper.WerkjaarGet(bestaandeRelatie) == gedoe.WerkJaar)
                {
                    // Neem van bestaande relatie het ID en de begindatum over.
                    relationshipRequest.Id = bestaandeRelatie.Id;
                    relationshipRequest.EndDate = bestaandeRelatie.EndDate;
                    if (_relationshipHelper.IsActief(bestaandeRelatie))
                    {
                        _log.Loggen(Niveau.Warning,
                            String.Format(
                                "{0} {1} (AD {3}) was al lid voor groep {2} in werkjaar {4}. Bestaand lidobject wordt geupdatet.",
                                contact.FirstName, contact.LastName, gedoe.StamNummer, contact.ExternalIdentifier, gedoe.WerkJaar),
                            gedoe.StamNummer, adNummer, contact.GapId);
                    }
                    else
                    {
                        _log.Loggen(Niveau.Info,
                            String.Format(
                                "Inactieve lidrelatie van {0} {1} (AD {2}) voor groep {3} in werkjaar {4} wordt opnieuw geactiveerd.",
                                contact.FirstName, contact.LastName, adNummer, gedoe.StamNummer, gedoe.WerkJaar),
                            gedoe.StamNummer, adNummer, contact.GapId);
                    }
                }
            }

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc => svc.RelationshipSave(_apiKey, _siteKey, relationshipRequest));

            _log.Loggen(Niveau.Info,
                String.Format(
                    "Nieuwe lidrelatie van {0} {1} (AD {2}) voor groep {3} in werkjaar {4} werd bewaard. RelationshipId: {5}",
                    contact.FirstName, contact.LastName, adNummer, gedoe.StamNummer, gedoe.WerkJaar, result.Id),
                gedoe.StamNummer, adNummer, contact.GapId);
        }
    }
}