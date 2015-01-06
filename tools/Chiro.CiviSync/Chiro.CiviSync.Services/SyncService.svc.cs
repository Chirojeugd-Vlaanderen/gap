/*
   Copyright 2013-2015 Chirojeugd-Vlaanderen vzw

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
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using System.ServiceModel;
using Chiro.Cdf.ServiceHelper;
using Chiro.ChiroCivi.ServiceContracts.DataContracts;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SyncService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SyncService.svc or SyncService.svc.cs at the Solution Explorer and start debugging.
    public class SyncService : ISyncPersoonService
    {
        private readonly string _siteKey = Properties.Settings.Default.SiteKey;
        private readonly string _apiKey = Properties.Settings.Default.ApiKey;
        private readonly IMiniLog _log;
        private readonly ServiceHelper _serviceHelper;

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Creates a new service instance.
        /// </summary>
        /// <param name="serviceHelper">Servicehelper that will connect to the CiviCRM API</param>
        /// <param name="log">Logger</param>
        public SyncService(ServiceHelper serviceHelper, IMiniLog log)
        {
            _serviceHelper = serviceHelper;
            _log = log;
        }

        /// <summary>
        /// Updatet de persoonsgegevens van <paramref name="persoon"/> in CiviCRM
        /// </summary>
        /// <param name="persoon">Persoon wiens gegevens te updaten zijn</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void PersoonUpdaten(Persoon persoon)
        {
            // TODO: personen met AD-nummer in aanvraag
            // TODO: GAP-ID bewaren

            // CiviCrm.net is generiek, en kent onze custom fields niet. Als ik de CiviCRM-API
            // aanspreek, krijg ik een gewoon contact.
            Contact civiContact = persoon.AdNummer == null
                ? new Contact()
                : ServiceHelper.CallService<ICiviCrmApi, Contact>(svc => svc.ContactGetSingle(
                    _apiKey,
                    _siteKey,
                    new ExternalIdentifierRequest(persoon.AdNummer.ToString()))) ?? new Contact();

            // Ik map dat naar een ChiroContact. Een ChiroContact heeft een GapID; dat is een custom
            // field. Onderstaande mapping laat dat GapID null.
            ChiroContact chiroContact = Mapper.Map<Contact, ChiroContact>(civiContact);

            // De mapping van persoon naar chiroContact overschrijft wat er al was met nieuwe
            // informatie. Hier krijgt het chiroContact ook zijn GapID, namelijk het ID van de
            // persoon.
            Mapper.Map(persoon, chiroContact);

            chiroContact.ApiOptions = new ApiOptions { Match = "external_identifier" };

            // Custom fields worden wel meegenomen met een save-operatie.
            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(svc => svc.ContactSave(
                _apiKey,
                _siteKey,
                chiroContact));

            AssertValid(result);

            _log.Loggen(
                Niveau.Info,
                String.Format("Contact {0} {1} bewaard (gid {3}, AD {2}).", persoon.VoorNaam, persoon.Naam,
                    persoon.AdNummer, result.Id),
                null,
                persoon.AdNummer,
                persoon.ID);
        }

        /// <summary>
        /// Throws an exception of the API <paramref name="result"/> is an error.
        /// </summary>
        /// <param name="result">A result of the CiviCRM API</param>
        private void AssertValid(ApiResult result)
        {
            if (result.IsError > 0)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Maakt het gegeven <paramref name="adres"/> het standaardadres van de gegeven <paramref name="bewoners"/>.
        /// Als het adres al bestond voor de gegeven bewoner, dan wordt het bestaande adres het standaardadres.
        /// Zo niet, wordt een nieuw adres als standaardadres gekoppeld, en het oude voorkeursadres verwijderd.
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
                if (bewoner.Persoon.AdNummer != null)
                {
                    // Adressen ophalen via chaining :-)
                    var civiContact = ServiceHelper.CallService<ICiviCrmApi, Contact>(svc => svc.ContactGetSingle(
                        _apiKey,
                        _siteKey,
                        new ExternalIdentifierRequest
                        {
                            ExternalIdentifier = bewoner.Persoon.AdNummer.ToString(),
                            ReturnFields = "id",
                            ChainedEntities = new[] { CiviEntity.Address }
                        }));

                    var adressen = civiContact.ChainedAddresses.Values;

                    // Als het adres al bestaat, dan kunnen we het overschrijven om casing te veranderen,
                    // voorkeursadres te zetten, en adrestype te bewaren.

                    var bestaande = (from a in adressen where IsHetzelfde(a, nieuwAdres) select a).FirstOrDefault();

                    if (bestaande != null)
                    {
                        nieuwAdres.Id = bestaande.Id;
                        // Neem het ID van het bestaande adres over, zodanig dat we het bestaande
                        // overschrijven.
                    }

                    nieuwAdres.ContactId = civiContact.Id;
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

                    var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(svc => svc.AddressSave(_apiKey, _siteKey, nieuwAdres));
                    AssertValid(result);
                    _log.Loggen(
                        Niveau.Info,
                        String.Format(
                            "Nieuw adres voor {0} {1} (gid {2} cid {3}): {4}, {5} {6} {7}. {8}",
                            bewoner.Persoon.VoorNaam, bewoner.Persoon.Naam, bewoner.Persoon.ID, civiContact.Id,
                            nieuwAdres.StreetAddress, nieuwAdres.PostalCode, nieuwAdres.PostalCodeSuffix, nieuwAdres.City,
                            nieuwAdres.Country),
                        null,
                        bewoner.Persoon.AdNummer,
                        bewoner.Persoon.ID);


                    // Verwijder oude voorkeuradres.

                    var teVerwijderenAdressen = (from adr in adressen
                                                 where adr.IsPrimary && adr.Id != nieuwAdres.Id
                                                 select adr).ToList();
                    foreach (var tvAdres in teVerwijderenAdressen)
                    {
                        result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(svc => svc.AddressDelete(_apiKey, _siteKey, new IdRequest(tvAdres.Id.Value)));
                        AssertValid(result);
                        _log.Loggen(
                            Niveau.Info,
                            String.Format(
                                "Oud voorkeursadres voor {0} {1} verwijderd (gid {2} cid {3}): {4}, {5} {6} {7}. {8}",
                                bewoner.Persoon.VoorNaam, bewoner.Persoon.Naam, bewoner.Persoon.ID, civiContact.Id,
                                tvAdres.StreetAddress, tvAdres.PostalCode, tvAdres.PostalCodeSuffix, tvAdres.City,
                                tvAdres.Country),
                            null,
                            bewoner.Persoon.AdNummer,
                            bewoner.Persoon.ID);
                    }
                }
            }
        }

        private bool IsHetzelfde(Address a1, Address a2)
        {
            return (String.Equals(a1.City, a2.City, StringComparison.InvariantCultureIgnoreCase) &&
                    String.Equals(a1.Country, a2.Country, StringComparison.InvariantCultureIgnoreCase) && a1.PostalCode == a2.PostalCode &&
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

        /// <summary>
        /// Creeer lidrelatie (geen membership) voor een persoon waarvan we geen AD-nummer kennen.
        /// </summary>
        /// <param name="details">Details van de persoon waarvoor een lidrelatie gemaakt moet worden.</param>
        /// <param name="lidGedoe">Informatie over de lidrelatie.</param>
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
    }
}
