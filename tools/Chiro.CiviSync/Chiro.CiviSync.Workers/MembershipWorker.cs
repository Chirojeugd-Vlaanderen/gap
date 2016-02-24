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
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;
using System.Diagnostics;

namespace Chiro.CiviSync.Workers
{
    /// <summary>
    /// Aardigheidjes voor memberships.
    /// </summary>
    public class MembershipWorker: BaseWorker
    {
        private readonly MembershipLogic _membershipLogic;

        public MembershipWorker(ServiceHelper serviceHelper, IMiniLog log, MembershipLogic membershipLogic, ICiviCache cache)
            : base(serviceHelper, log, cache)
        {
            _membershipLogic = membershipLogic;
        }

        /// <summary>
        /// Werkt een bestaand membership bij.
        /// </summary>
        /// <param name="bestaandMembership">Bestaand membership.</param>
        /// <param name="gedoe">Membershipdetails</param>
        public void BestaandeBijwerken(Membership bestaandMembership, MembershipGedoe gedoe)
        {
            // Om te weten of een membership 'geupgradet' moet worden van gratis naar
            // betalend (#4520), hebben we informatie nodig over de betalingen. We asserten
            // dus dat die betalingsinfo mee in het bestaandMembership zit.
            Debug.Assert(bestaandMembership.MembershipPaymentResult != null);
            int werkJaar = _membershipLogic.WerkjaarGet(bestaandMembership);
            // We halen de persoon opnieuw op. Wat een beetje overkill is, aangezien
            // dat enkel dient om te kunnen loggen. Maar het bijwerken van een bestaand
            // membership is hopelijk niet zodanig vaak nodig dat dit vertragend zal worden.

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc =>
                        svc.ContactGet(ApiKey, SiteKey,
                            new ContactRequest
                            {
                                Id = bestaandMembership.ContactId,
                                ReturnFields = "external_identifier,first_name,last_name,custom_1"
                            }));
            result.AssertValid();
            if (result.Count == 0)
            {
                Log.Loggen(Niveau.Error,
                    String.Format(
                        "Onbestaand contact {0} voor te verlengen menbership (id {1}, werkjaar {2}). Genegeerd. WTF.",
                        bestaandMembership.ContactId, bestaandMembership.Id, werkJaar),
                    gedoe.StamNummer, null, null);
                return;                
            }
            var contact = result.Values.First();
            int adNummer = int.Parse(contact.ExternalIdentifier);

            int? civiGroepId = ContactIdGet(gedoe.StamNummer);
            if (civiGroepId == null)
            {
                Log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet bewaard.", gedoe.StamNummer),
                    gedoe.StamNummer, adNummer, null);
                return;
            }

            // We moeten bijwerken in deze gevallen:
            // (1) er was al een gratis aansluiting, maar de nieuwe aansluiting is betalend. (#4510)
            // (2) er was nog geen verzekering loonverlies, nu is die er wel. Alnaargelang de aansluiting gebeurde door
            //     een groep of niet, moet de factuurstatus aangepast worden. (#4514)
            if (!gedoe.Gratis && _membershipLogic.IsGratis(bestaandMembership))
            {
                var membershipRequest = new MembershipRequest
                {
                    Id = bestaandMembership.Id,
                    VerzekeringLoonverlies = gedoe.MetLoonVerlies,
                    AangemaaktDoorPloegId = civiGroepId,
                    FactuurStatus = FactuurStatus.VolledigTeFactureren,
                };

                var updateResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                    svc => svc.MembershipSave(ApiKey, SiteKey, membershipRequest));
                updateResult.AssertValid();
                Log.Loggen(Niveau.Info,
                    String.Format(
                        "Membership van {0} {1} (AD {2}, ID {3}) voor werkjaar {4}, met ID {5} moet nu betaald worden door {6}.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, contact.GapId,
                        werkJaar, updateResult.Id, gedoe.StamNummer), gedoe.StamNummer, adNummer, contact.GapId);
            }
            else if (gedoe.MetLoonVerlies && !bestaandMembership.VerzekeringLoonverlies)
            {
                var membershipRequest = new MembershipRequest
                {
                    Id = bestaandMembership.Id,
                    VerzekeringLoonverlies = true,
                    AangemaaktDoorPloegId = civiGroepId,
                    // TODO: Als er al een bestaand betalend membership is, maar het gewest verzekert bij
                    // voor loonverlies, dan ziten we hier mogelijk in de problemen.
                    FactuurStatus = gedoe.Gratis
                        ? FactuurStatus.FactuurOk
                        : bestaandMembership.FactuurStatus == FactuurStatus.FactuurOk
                            ? FactuurStatus.ExtraVerzekeringTeFactureren
                            : bestaandMembership.FactuurStatus
                };

                var updateResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                    svc => svc.MembershipSave(ApiKey, SiteKey, membershipRequest));
                updateResult.AssertValid();
                Log.Loggen(Niveau.Info,
                    String.Format(
                        "Membership met ID {5} door {6} bijgewerkt voor {0} {1} (AD {2}, ID {3}) met verzekering loonverlies voor werkjaar {4}.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, contact.GapId,
                        werkJaar, updateResult.Id, gedoe.StamNummer), gedoe.StamNummer, adNummer, contact.GapId);
            }
            else
            {
                // Membership bestond eigenlijk al. Maar we sturen het toch opnieuw naar de civi,
                // zodat een update-hook wordt gefired, en de Civi het juiste werkjaar aflevert
                // bij het GAP. (#3581)

                var membershipRequest = new MembershipRequest
                {
                    Id = bestaandMembership.Id,
                    // Ofwel had ik al een verzekerling loonverlies, ofwel is de bijwerking zonder
                    // verzekering loonverlies. Voor het bijgewerkte loonverlies kan het dus nog
                    // alle kanten op. (#4413)
                    VerzekeringLoonverlies = bestaandMembership.VerzekeringLoonverlies || gedoe.MetLoonVerlies,
                    // Verander niets aan de bestaande ploeg-ID. Anders wordt het te verwarrend.
                    // AangemaaktDoorPloegId = civiGroepId,
                    FactuurStatus = bestaandMembership.FactuurStatus
                };

                var updateResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                    svc => svc.MembershipSave(ApiKey, SiteKey, membershipRequest));
                updateResult.AssertValid();

                // We voegen een asteriskje toe achter dit logbericht. Op die manier kunnen we de berichten van de buggy
                // CiviSync (#4413) onderscheiden van die van de gepatchte.
                int logResult = Log.Loggen(Niveau.Info,
                    String.Format(
                        "{0} {1} (AD {3}, ID {2}) was al aangesloten in werkjaar {4}. Opnieuw naar Civi om sync te herstellen.*",
                        contact.FirstName, contact.LastName, contact.GapId, contact.ExternalIdentifier, werkJaar),
                    null, adNummer, contact.GapId);

                if (logResult != 0)
                {
                    // Loggen mislukt. Wellicht was GAP-ID van contact ongeldig.
                    Log.Loggen(Niveau.Info,
                    String.Format(
                        "{0} {1} (AD {2}) was al aangesloten in werkjaar {3}. Opnieuw naar Civi om sync te herstellen.*",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, werkJaar),
                    null, adNummer, null);
                    // We kunnen dat jammer genoeg niet met de API herstellen, want als we GapID=NULL meegeven, wordt dat
                    // als ontbrekende parameter beschouwd.
                    Log.Loggen(Niveau.Error,
                    String.Format(
                        "{0} {1} (AD {2}) heeft ongeldig Gap-ID {3} in Civi.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, contact.GapId),
                    null, adNummer, null);
                }
            }
        }

        /// <summary>
        /// Beeindigt (of verwijdert als het maar amper begonnen was) een abonnement.
        /// </summary>
        /// <param name="bestaandMembership">te verwijderen abonnement.</param>
        /// <param name="contact">Contact horende bij het abonnement. Wordt gebruikt om te loggen.</param>
        public void AbonnementBeeindigen(Membership bestaandMembership, Contact contact)
        {
            Debug.Assert(bestaandMembership.MembershipTypeId == (int)MembershipType.DubbelpuntAbonnement);
            int adNummer = int.Parse (contact.ExternalIdentifier);

            if (_membershipLogic.IsVervallen(bestaandMembership))
            {
                Log.Loggen(Niveau.Warning,
                    String.Format(
                        "Dubbelpuntabonnement voor {0} {1} (AD {2}, GapID {3}) was al beeindigd, einddatum {4}. Abonnement niet verwijderd.",
                        contact.FirstName, contact.LastName, adNummer, contact.GapId, bestaandMembership.EndDate),
                    null, adNummer, contact.GapId);
                return;
            }
            if (_membershipLogic.Beeindigen(bestaandMembership) != null)
            {
                var request = new MembershipRequest
                {
                    Id = bestaandMembership.Id,
                    EndDate = bestaandMembership.EndDate
                };

                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                    svc => svc.MembershipSave(ApiKey, SiteKey, request));
                result.AssertValid();

                Log.Loggen(Niveau.Debug,
                    String.Format("DubbelpuntAbonnement voor {0} {1} ({2}, {3}) stopgezet. Type {4}.",
                        contact.FirstName, contact.LastName, contact.Email, contact.StreetAddress,
                        bestaandMembership.AbonnementType), null, adNummer, contact.GapId);
            }
            else
            {
                var request = new DeleteRequest
                {
                    Id = bestaandMembership.Id
                };
                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc => svc.MembershipDelete(ApiKey, SiteKey, request));
                result.AssertValid();

                Log.Loggen(Niveau.Debug,
                    String.Format("DubbelpuntAbonnement voor {0} {1} ({2}, {3}) verwijderd. Type {4}.",
                        contact.FirstName, contact.LastName, contact.Email, contact.StreetAddress,
                        bestaandMembership.AbonnementType), null, adNummer, contact.GapId);

            }
        }
    }
}
