/*
   Copyright 2013 Chirojeugd-Vlaanderen vzw

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
using AutoMapper;
using Chiro.CiviCrm.ClientInterfaces;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.CiviCrm.Domain;
using System.ServiceModel;

namespace Chiro.CiviSync.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SyncService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SyncService.svc or SyncService.svc.cs at the Solution Explorer and start debugging.
    public class SyncService : ISyncPersoonService, IDisposable
    {
        private readonly ICiviCrmClient _civiCrmClient;

        public SyncService(ICiviCrmClient civiCrmClient)
        {
            _civiCrmClient = civiCrmClient;
        }

        /// <summary>
        /// Updatet de persoonsgegevens van <paramref name="persoon"/> in CiviCRM
        /// </summary>
        /// <param name="persoon">Persoon wiens gegevens te updaten zijn</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void PersoonUpdaten(Persoon persoon)
        {
            // TODO: personen met Civi-ID in aanvraag
            var contact = (persoon.CiviID == null
                ? new Contact {Id = 0}
                : _civiCrmClient.ContactGet(persoon.CiviID.Value)) ?? new Contact {Id = 0};

            Mapper.Map(persoon, contact);
            _civiCrmClient.ContactSave(contact);
        }

        /// <summary>
        /// Maakt het gegeven <paramref name="adres"/> het standaardadres van de gegeven <paramref name="bewoners"/>.
        /// Als het adres al bestond voor de gegeven bewoner, dan wordt het bestaande adres het standaardadres.
        /// Zo niet, wordt een nieuw adres als standaardadres gekoppeld.
        /// </summary>
        /// <param name="adres">Nieuw standaardadres van de gegeven <paramref name="bewoners"/></param>
        /// <param name="bewoners">Bewoners die het nieuw <paramref name="adres"/> krijgen</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
        {
            var nieuwAdres = Mapper.Map<Adres, Address>(adres);

            // TODO: personen met civi-ID in aanvraag
            foreach (var bewoner in bewoners)
            {
                if (bewoner.Persoon.CiviID != null)
                {
                    var adressen = _civiCrmClient.ContactAddressesGet(bewoner.Persoon.CiviID.Value);

                    var bestaande = (from a in adressen where IsHetzelfde(a, nieuwAdres) select a).FirstOrDefault();

                    if (bestaande != null)
                    {
                        nieuwAdres.Id = bestaande.Id;
                        // Door het ID te bewaren, overschrijven we het bestaande adres
                        nieuwAdres.ContactId = bestaande.ContactId;
                    }
                    else
                    {
                        nieuwAdres.ContactId = bewoner.Persoon.CiviID.Value;
                    }

                    nieuwAdres.IsBilling = true;
                    nieuwAdres.IsPrimary = true;

                    switch (bewoner.AdresType)
                    {
                        case AdresTypeEnum.Thuis:
                            nieuwAdres.LocationTypeId = 1;
                            break;
                        case AdresTypeEnum.Werk:
                            nieuwAdres.LocationTypeId = 2;
                            break;
                        default:
                            nieuwAdres.LocationTypeId = 4; // (other)
                            // TODO: kot (type moet uit de database gehaald worden)
                            break;
                    }

                    _civiCrmClient.AddressSave(nieuwAdres);
                }
            }
        }

        private bool IsHetzelfde(Address a1, Address a2)
        {
            return (String.Equals(a1.City, a2.City, StringComparison.InvariantCultureIgnoreCase) &&
                    a1.CountryId == a2.CountryId && a1.PostalCode == a2.PostalCode &&
                    String.Equals(a1.PostalCodeSuffix, a2.PostalCodeSuffix, StringComparison.InvariantCultureIgnoreCase) &&
                    a1.StateProvinceId == a2.StateProvinceId &&
                    String.Equals(a1.StreetAddress, a2.StreetAddress, StringComparison.InvariantCultureIgnoreCase));
        }

        public void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
            throw new NotImplementedException();
        }

        public void CommunicatieBijwerken(Persoon persoon, string nummerBijTeWerken, CommunicatieMiddel communicatieMiddel)
        {
            throw new NotImplementedException();
        }

        public void AlleCommunicatieBewaren(Persoon persoon, IEnumerable<CommunicatieMiddel> communicatieMiddelen)
        {
            throw new NotImplementedException();
        }

        public void CommunicatieVerwijderen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
            throw new NotImplementedException();
        }

        public void LidBewaren(int adNummer, LidGedoe gedoe)
        {
            throw new NotImplementedException();
        }

        public void NieuwLidBewaren(PersoonDetails details, LidGedoe lidGedoe)
        {
            throw new NotImplementedException();
        }

        public void LidVerwijderen(int adNummer, string stamNummer, int werkjaar, DateTime uitschrijfDatum)
        {
            throw new NotImplementedException();
        }

        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkjaar, DateTime uitschrijfDatum)
        {
            throw new NotImplementedException();
        }

        public void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<FunctieEnum> functies)
        {
            throw new NotImplementedException();
        }

        public void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType)
        {
            throw new NotImplementedException();
        }

        public void AfdelingenUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen)
        {
            throw new NotImplementedException();
        }

        public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar)
        {
            throw new NotImplementedException();
        }

        public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar)
        {
            throw new NotImplementedException();
        }

        public void BivakBewaren(Bivak bivak)
        {
            throw new NotImplementedException();
        }

        public void BivakPlaatsBewaren(int uitstapID, string plaatsNaam, Adres adres)
        {
            throw new NotImplementedException();
        }

        public void BivakContactBewaren(int uitstapID, int adNummer)
        {
            throw new NotImplementedException();
        }

        public void BivakContactBewarenAdOnbekend(int uitstapID, PersoonDetails details)
        {
            throw new NotImplementedException();
        }

        public void BivakVerwijderen(int uitstapID)
        {
            throw new NotImplementedException();
        }

        public void GroepUpdaten(Groep g)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _civiCrmClient.Dispose();
        }
    }
}
