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

using AutoMapper;
using Chiro.CiviCrm.ServiceContracts.DataContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public static class MappingHelper
    {
        public static void MappingsDefinieren()
        {
            Mapper.CreateMap<Persoon, Contact>()
                .ForMember(src => src.BirthDate, opt => opt.MapFrom(src => src.GeboorteDatum))
                .ForMember(src => src.ContactType, opt => opt.MapFrom(src => ContactType.Individual))
                .ForMember(src => src.DeceasedDate, opt => opt.MapFrom(src => src.SterfDatum))
                .ForMember(src => src.ExternalId, opt => opt.MapFrom(src => src.AdNummer))
                .ForMember(src => src.FirstName, opt => opt.MapFrom(src => src.VoorNaam))
                .ForMember(src => src.Gender, opt => opt.MapFrom(src => (Gender) (3 - (int) src.Geslacht)))
                .ForMember(src => src.GenderId, opt => opt.MapFrom(src => 3 - (int) src.Geslacht))
                .ForMember(src => src.Id, opt => opt.Ignore()) // contactID != persoonID
                .ForMember(src => src.IsDeceased, opt => opt.MapFrom(src => src.SterfDatum != null))
                .ForMember(src => src.LastName, opt => opt.MapFrom(src => src.Naam))
                .ForMember(src => src.BirthDateString, opt => opt.Ignore())
                .ForMember(src => src.DeceasedDateString, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();
        }
    }
}