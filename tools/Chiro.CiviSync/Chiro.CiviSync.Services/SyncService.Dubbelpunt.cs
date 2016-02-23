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
using Chiro.CiviCrm.Api.DataContracts.Requests;
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
                request.JoinDate = bestaandMembership.JoinDate < request.JoinDate
                    ? bestaandMembership.JoinDate
                    : request.JoinDate;

                if (bestaandMembership.EndDate == null ||
                    bestaandMembership.EndDate.Value.AddDays(Properties.Settings.Default.DagenOverlapDubbelpunt) >=
                    request.StartDate)
                {
                    // Als er een actief membership is, of de overlap is 'redelijk', dan
                    // hergebruiken we het recentste membership.

                    // Misschien beter mappen? Geen idee.
                    request.Id = bestaandMembership.Id;
                    request.StartDate = bestaandMembership.StartDate < request.StartDate
                        ? bestaandMembership.StartDate
                        : request.StartDate;
                    request.EndDate = bestaandMembership.EndDate > request.EndDate
                        ? bestaandMembership.EndDate
                        : request.EndDate;
                    request.Status = MembershipStatus.Current;
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
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void AbonnementStopzetten(int adNummer)
        {
            var contact = _contactWorker.PersoonMetRecentsteMembership(adNummer, MembershipType.DubbelpuntAbonnement);

            if (contact == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te bewaren Dubbelpuntabonnement - als dusdanig terug naar GAP.", adNummer),
                    null, adNummer, null);

                await _gapUpdateClient.OngeldigAdNaarGap(adNummer);
                return;
            }

            if (contact.MembershipResult.Count != 1)
            {
                _log.Loggen(Niveau.Error,
                    String.Format(
                        "Geen recentste Dubbelpuntabonnement gevonden voor {0} {1} (AD {2}, GapID {3}). Abonnement niet verwijderd.",
                        contact.FirstName, contact.LastName, adNummer, contact.GapId),
                    null, adNummer, contact.GapId);
                return;
            }
            var bestaandMembership = contact.MembershipResult.Values.First();

            if (_membershipLogic.IsVervallen(bestaandMembership))
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "Dubbelpuntabonnement voor {0} {1} (AD {2}, GapID {3}) was al beeindigd, einddatum {4}. Abonnement niet verwijderd.",
                        contact.FirstName, contact.LastName, adNummer, contact.GapId, bestaandMembership.EndDate),
                    null, adNummer, contact.GapId);
                return;
            }
            _membershipLogic.Beeindigen(bestaandMembership);

            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                svc => svc.MembershipSave(_apiKey, _siteKey, Mapper.Map<MembershipRequest>(bestaandMembership)));
            result.AssertValid();

            _log.Loggen(Niveau.Debug,
                String.Format("DubbelpuntAbonnement voor {0} {1} ({2}, {3}) stopgezet. Type {4}.",
                    contact.FirstName, contact.LastName, contact.Email, contact.StreetAddress,
                    bestaandMembership.AbonnementType), null, adNummer, contact.GapId);
        }

        /// <summary>
        /// Verwijdert abonnement van persoon met gegeven <paramref name="details"/>.
        /// </summary>
        /// <param name="details">Details van persoon die geen abonnement meer wil.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AbonnementStopzettenNieuwePersoon(PersoonDetails details)
        {
            // Update of maak de persoon, en vind zijn AD-nummer
            int adNr = PersoonUpdatenOfMaken(details);
            AbonnementStopzetten(adNr);
        }
    }
}
