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
using AutoMapper;
using Chiro.CiviCrm.ClientInterfaces;
using Chiro.CiviCrm.ServiceContracts.DataContracts;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

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

        public void PersoonUpdaten(Persoon persoon)
        {
            var contact = (persoon.AdNummer == null
                ? new Contact {Id = 0}
                : _civiCrmClient.ContactFind(persoon.AdNummer.Value)) ?? new Contact {Id = 0};

            Mapper.Map<Persoon, Contact>(persoon, contact);
            _civiCrmClient.ContactSave(contact);
        }

        public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
        {
            throw new NotImplementedException();
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
