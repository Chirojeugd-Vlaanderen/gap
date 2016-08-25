/*
   Copyright 2015, 2016 Chirojeugd-Vlaanderen vzw

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
        /// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer van het opgegeven lid
        /// </param>
        /// <param name="gedoe">
        /// De nodige info voor het lid.
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void LidBewaren(int adNummer, LidGedoe gedoe)
        {
            int? civiGroepId = _contactWorker.ContactIdGet(gedoe.StamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet bewaard.", gedoe.StamNummer),
                    gedoe.StamNummer, adNummer, null);
                return;
            }

            // We halen het recentste lid op, en niet het 'actieve' lid, zodat we zo nodig een inactief
            // lid terug kunnen activeren.
            var contact = _contactWorker.PersoonMetRecentsteLid(adNummer, civiGroepId);

            if (contact == null || contact.ExternalIdentifier == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te bewaren lid - als dusdanig terug naar GAP.", adNummer),
                    gedoe.StamNummer, adNummer, null);

               await _gapUpdateClient.OngeldigAdNaarGap(adNummer);
                return;
            }

            // Request voor te bewaren (nieuwe) lidrelatie: eerst een standaardrequest voor dit werkjaar.
            // Als het contact al zo'n relatie had (contact.RelationshipResult), dan nemen we van die bestaande
            // de relevante zaken over.
            var relationshipRequest = _relationshipLogic.RequestMaken(RelatieType.LidVan, contact.Id, civiGroepId.Value, gedoe.Werkjaar,
                gedoe.UitschrijfDatum);

            if (contact.RelationshipResult.Count == 1)
            {
                var bestaandeRelatie = contact.RelationshipResult.Values.First();

                // Het zou cool zijn moesten we gewoon kunnen zeggen: er bestaat nog geen actieve relatie, we
                // maken er een nieuwe. Maar op die manier zou de civi vol met superkorte relaties raken als
                // iemand met een slecht karakter een van zijn leden constant in- en uitschrijft.
                // Het kan dus zijn dat een gestopte relatie toch als huidig wordt beschouwd, namelijk als ze
                // nog niet zo lang geleden is gestart.
                if (_relationshipLogic.IsHuidig(bestaandeRelatie))
                {
                    // Neem van bestaande relatie het ID en de begindatum over.
                    relationshipRequest.Id = bestaandeRelatie.Id;
                    relationshipRequest.StartDate = bestaandeRelatie.StartDate;
                    if (bestaandeRelatie.IsActive)
                    {
                        _log.Loggen(Niveau.Warning,
                            String.Format(
                                "{0} {1} (AD {3}) was al lid voor groep {2}; startdatum {4}. Bestaand lidobject wordt geupdatet.",
                                contact.FirstName, contact.LastName, gedoe.StamNummer, contact.ExternalIdentifier, bestaandeRelatie.StartDate),
                            gedoe.StamNummer, adNummer, contact.GapId);
                    }
                    else
                    {
                        _log.Loggen(Niveau.Info,
                            String.Format(
                                "Inactieve lidrelatie van {0} {1} (AD {2}) voor groep {3} met startdatum {4} wordt bijgewerkt.",
                                contact.FirstName, contact.LastName, adNummer, gedoe.StamNummer, bestaandeRelatie.StartDate),
                            gedoe.StamNummer, adNummer, contact.GapId);
                    }
                }
            }

            // We vervangen functies en afdelingen.
            relationshipRequest.Afdeling = gedoe.LidType == LidTypeEnum.Kind
                ? Mapper.Map<AfdelingEnum, Afdeling>(gedoe.OfficieleAfdelingen.First()) : Afdeling.Leiding;
            relationshipRequest.LeidingVan = gedoe.LidType == LidTypeEnum.Leiding
                ? Mapper.Map<IEnumerable<AfdelingEnum>, Afdeling[]>(gedoe.OfficieleAfdelingen)
                : null;
            relationshipRequest.Functies = FunctieLogic.KipCodes(gedoe.NationaleFuncties);

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc => svc.RelationshipSave(_apiKey, _siteKey, relationshipRequest));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format(
                    "{6} {7} {8}: AD {0} stamnr {1} start {2:dd/MM/yyyy} - afd {3}, lafd {4}, func {5} relID {9}",
                    adNummer, gedoe.StamNummer, relationshipRequest.StartDate, relationshipRequest.Afdeling,
                    relationshipRequest.LeidingVan == null
                        ? "n/a"
                        : String.Join(",", relationshipRequest.LeidingVan.Select(afd => afd.ToString())),
                    relationshipRequest.Functies == null
                        ? "(geen)"
                        : String.Join(",", relationshipRequest.Functies),
                    gedoe.UitschrijfDatum == null ? "Inschrijving" : "Uitschrijving", contact.FirstName,
                    contact.LastName, result.Id),
                gedoe.StamNummer, adNummer, contact.GapId);
        }

        /// <summary>
        /// Creeer lidrelatie (geen membership) voor een persoon waarvan we geen AD-nummer kennen.
        /// </summary>
        /// <param name="details">Details van de persoon waarvoor een lidrelatie gemaakt moet worden.</param>
        /// <param name="lidGedoe">Informatie over de lidrelatie.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void NieuwLidBewaren(PersoonDetails details, LidGedoe lidGedoe)
        {
            // Update of maak de persoon, en vind zijn AD-nummer
            int adNr = PersoonUpdatenOfMaken(details);

            // Bewaar de lidrelatie op basis van het AD-nummer.
            LidBewaren(adNr, lidGedoe);
        }
    }
}