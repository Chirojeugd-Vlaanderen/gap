using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using AutoMapper;

namespace Chiro.Gap.ServiceContracts.Mappers
{
    /// <summary>
    /// Klasse voor het mappen van een Groep naar GroepInfo.
    /// Gebruikt automapper.  De statische constructor zal automapper configureren.
    /// </summary>
    public class GroepInfoMapper
    {
        /// <summary>
        /// Configureert automapper zodat de mapfuncties naar behoren werken.
        /// </summary>
        static GroepInfoMapper()
        {
            Mapper.CreateMap<Groep, GroepInfo>()
                .ForMember(dst => dst.Plaats, opt => opt.MapFrom(
                    src => src is ChiroGroep ? (src as ChiroGroep).Plaats : Properties.Resources.NietVanToepassing))
                .ForMember(dst => dst.StamNummer, opt => opt.MapFrom(
                    src => src.Code == null ? String.Empty : src.Code.ToUpper()));
        }

        /// <summary>
        /// Mapt een Groep naar GroepInfo
        /// </summary>
        /// <param name="src">te mappen groep</param>
        /// <param name="dst">groepinfo waarnaar te mappen</param>
        public static void Map(Groep src, GroepInfo dst)
        {
            Mapper.Map(src, dst);
        }

        /// <summary>
        /// Mapt een Groep naar GroepInfo
        /// </summary>
        /// <param name="g">te mappen Groep</param>
        /// <returns>gemapte GroepInfo</returns>
        public static GroepInfo mapGroep(Groep g)
        {
            GroepInfo result = new GroepInfo();
            Map(g, result);
            return result;
        }

        /// <summary>
        /// Mapt een rij groepen op een rij GroepInfo-objecten.
        /// </summary>
        /// <param name="groepen">rij te mappen groepen</param>
        /// <returns>rij gemapte GroepInfo</returns>
        public static IList<GroepInfo> mapGroepen(IEnumerable<Groep> groepen)
        {
            IList<GroepInfo> giList = new List<GroepInfo>();

            foreach (var gr in groepen)
            {
                giList.Add(mapGroep(gr));
            }
            return giList;
        }

    }
}
