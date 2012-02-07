using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AutoMapper;

using Chiro.Gap.Diagnostics.ServiceContracts.DataContracts;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Diagnostics.Service
{
    /// <summary>
    /// Statische klasse die gebruikt wordt om automapper te configureren.
    /// </summary>
    public static class MappingHelper
    {
        /// <summary>
        /// Definieert de mappings waarvoor we automapper zullen gebruiken
        /// </summary>
        public static void MappingsDefinieren()
        {
            // Mappings voor diagnostic datacontracts

            Mapper.CreateMap<Groep, GroepContactInfo>()
                .ForMember(dst => dst.Plaats, opt => opt.MapFrom(src => src is ChiroGroep
                                                                             ? (src as ChiroGroep).Plaats
                                                                             : String.Empty))
                .ForMember(dst => dst.Contacten, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();

            // Mappings voor kipsync

            Sync.MappingHelper.MappingsDefinieren();
        }
    }
}