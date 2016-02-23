/*
 * Copyright 2015, 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Dubbelpuntabonnement als membership naar Civi.
        /// </summary>
        /// <param name="adNummer">AD-nummer van de persoon die een abonnement wil.</param>
        /// <param name="werkJaar">Werkjaar van het abonnement.</param>
        /// <param name="type">Digitaal of op papier.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void AbonnementBewaren(int adNummer, int werkJaar, AbonnementTypeEnum type)
        {
            var contact = _contactWorker.PersoonMetRecentsteMembership(adNummer, MembershipType.DubbelpuntAbonnement);

            if (contact == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te bewaren Dubbelpuntabonnement - als dusdanig terug naar GAP.", adNummer),
                    null, adNummer, null);

                await _gapUpdateClient.OngeldigAdNaarGap(adNummer);
                return;
            }

            // Nieuw request dat we alleen zullen gebruiken als er geen oud is, of als
            // het oud al te oud is.
            var request = _membershipLogic.VanWerkjaar(MembershipType.DubbelpuntAbonnement, contact.Id, null,
                werkJaar);
            if (contact.MembershipResult.Count == 1)
            {
                var bestaandMembership = contact.MembershipResult.Values.First();

                Debug.Assert(bestaandMembership.EndDate.HasValue);
                if (bestaandMembership.EndDate == null ||
                    bestaandMembership.EndDate.Value.AddDays(Properties.Settings.Default.DagenOverlapDubbelpunt) >=
                    request.StartDate)
                {
                    // Als er een actief membership is, of de overlap is 'redelijk', dan
                    // hergebruiken we het recentste membership.

                    DateTime? nieuweEndDate = request.EndDate;
                    Mapper.Map(bestaandMembership, request);
                    request.EndDate = nieuweEndDate;
                }
                else
                {
                    // In het andere geval houden we wel de join date.
                    request.JoinDate = bestaandMembership.JoinDate;
                }
            }
            request.AbonnementType = (AbonnementType)(int)type;

            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                svc => svc.MembershipSave(_apiKey, _siteKey, request));
            result.AssertValid();

            _log.Loggen(Niveau.Debug,
                String.Format("DubbelpuntAbonnement voor {0} {1} ({2}, {3}) bijgewerkt. Type {4}.",
                    contact.FirstName, contact.LastName, contact.Email, contact.StreetAddress,
                    type), null, adNummer, contact.GapId);
        }

        /// <summary>
        /// Dubbelpuntabonnement als membership naar Civi.
        /// </summary>
        /// <param name="details">Details van de persoon die een abonnement wil.</param>
        /// <param name="werkjaar">Werkjaar van het abonnement.</param>
        /// <param name="type">Digitaal of op papier.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AbonnementNieuwePersoonBewaren(PersoonDetails details, int werkjaar, AbonnementTypeEnum type)
        {
            // Update of maak de persoon, en vind zijn AD-nummer
            int adNr = PersoonUpdatenOfMaken(details);
            AbonnementBewaren(adNr, werkjaar, type);
        }

        /// <summary>
        /// Verwijdert abonnement van persoon met gegeven <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer van persoon die geen abonnement meer wil.</param>
        public void AbonnementVerwijderen(int adNummer)
        {
            _log.Loggen(Niveau.Debug,
                string.Format("DubbelpuntAbonnement verwijderd: {0}.",
                    adNummer), null, adNummer, null);
        }

        /// <summary>
        /// Verwijdert abonnement van persoon met gegeven <paramref name="details"/>.
        /// </summary>
        /// <param name="details">Details van persoon die geen abonnement meer wil.</param>
        public void AbonnementVerwijderenNieuwePersoon(PersoonDetails details)
        {
            throw new NotImplementedException();
        }
    }
}
