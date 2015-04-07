using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web;
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
        /// Gaat op zoek naar de gegeven <paramref name="persoon"/>, en zoekt daarvan de communicatie van
        /// type <c>communicatieMiddel.Type</c> en nummer <paramref name="nummerBijTeWerken"/>. Dat
        /// gevonden communicatiemiddel wordt vervangen door <paramref name="communicatieMiddel"/>.
        /// </summary>
        /// <param name="persoon">persoon met te vervangen communicatiemiddel</param>
        /// <param name="nummerBijTeWerken">huidig nummer van te vervangen communicatiemiddel</param>
        /// <param name="communicatieMiddel">nieuwe info voor te vervangen communicatiemiddel</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void CommunicatieBijwerken(Persoon persoon, string nummerBijTeWerken, CommunicatieMiddel communicatieMiddel)
        {
            if (persoon.AdNummer == null)
            {
                persoon.AdNummer = AdNummerZoeken(persoon);
            }

            if (persoon.AdNummer == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Kan {0} {1} voor {2} niet bijwerken; persoon niet gevonden.", communicatieMiddel.Type,
                        nummerBijTeWerken, persoon),
                    null, null, null);
                return;
            }

            int? contactId = _contactWorker.ContactIdGet(persoon.AdNummer.Value);

            if (contactId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor bij te werken communicatie - als dusdanig terug naar GAP.", persoon.AdNummer),
                    null, persoon.AdNummer, null);

                await _gapUpdateClient.OngeldigAdNaarGap(persoon.AdNummer.Value);
                return;
            }

            var teVervangen = new CommunicatieMiddel
            {
                GeenMailings = communicatieMiddel.GeenMailings,
                Type = communicatieMiddel.Type,
                Waarde = nummerBijTeWerken
            };

            // Zoek de te vervangen communicatie
            var oudeCommunicatieRequest = CommunicatieLogic.RequestMaken(teVervangen, contactId, true);

            // Zoek er hoogstens 1. Dan hebben we een een Id in het result.
            oudeCommunicatieRequest.ApiOptions = new ApiOptions { Limit = 1 };
            var bestaandeCommunicatie =
                ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc =>
                        svc.GenericCall(_apiKey, _siteKey, oudeCommunicatieRequest.EntityType, ApiAction.Get,
                            oudeCommunicatieRequest));
            bestaandeCommunicatie.AssertValid();

            // Maak een request voor de nieuwe
            var nieuweCommunicatieRequest = CommunicatieLogic.RequestMaken(communicatieMiddel, contactId, false);

            if (bestaandeCommunicatie.Count == 0)
            {
                _log.Loggen(Niveau.Info,
                    String.Format("Kon bestaande communicatievorm ({0}) {1} voor {2} {3} (AD {4}) niet vinden. Nieuwe {5} gewoon toegevoegd.",
                        communicatieMiddel.Type, nummerBijTeWerken, persoon.VoorNaam, persoon.Naam,
                        persoon.AdNummer, communicatieMiddel.Waarde), null, persoon.AdNummer, persoon.ID);
            }
            else
            {
                Debug.Assert(bestaandeCommunicatie.Id.HasValue);
                _log.Loggen(Niveau.Info,
                    String.Format(
                        "Bestaande communicatievorm ({0}) {1} voor {2} {3} (AD {4}) vervangen door {5}.",
                        communicatieMiddel.Type, nummerBijTeWerken, persoon.VoorNaam, persoon.Naam,
                        persoon.AdNummer, communicatieMiddel.Waarde), null, persoon.AdNummer, persoon.ID);
                CommunicatieLogic.RequestIdZetten(nieuweCommunicatieRequest, bestaandeCommunicatie.Id.Value);
            }

            // Bewaar de communicatievorm
            var createResult = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc =>
                    svc.GenericCall(_apiKey, _siteKey, nieuweCommunicatieRequest.EntityType, ApiAction.Create,
                        nieuweCommunicatieRequest));
            createResult.AssertValid();
        }
    }
}