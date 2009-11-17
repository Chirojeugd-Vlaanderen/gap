using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts.Mappers
{
	/// <summary>
	/// Helperfunctionaiteit voor mapping
	/// </summary>
	public static class MappingHelper
	{
		/// <summary>
		/// Definieert meteen alle nodige mappings.
		/// </summary>
		public static void MappingsDefinieren()
		{
			Mapper.CreateMap<Groep, GroepInfo>()
			    .ForMember(dst => dst.Plaats, opt => opt.MapFrom(
				src => src is ChiroGroep ? (src as ChiroGroep).Plaats : Properties.Resources.NietVanToepassing))
			    .ForMember(dst => dst.StamNummer, opt => opt.MapFrom(
				src => src.Code == null ? String.Empty : src.Code.ToUpper()));

			// Als de namen van PersoonInfo wat anders gekozen zouden zijn, dan zou dat wel wat
			// `ForMember'-regels uitsparen.

			Mapper.CreateMap<GelieerdePersoon, PersoonInfo>()
				.ForMember(
					dst => dst.GelieerdePersoonID,
					opt => opt.MapFrom(src => src.ID))
				.ForMember(
					dst => dst.IsLid,
					opt => opt.MapFrom(src => src.Lid.Count > 0))
				.ForMember(
					dst => dst.AdNummer,
					opt => opt.MapFrom(src => src.Persoon.AdNummer))
				.ForMember(
					dst => dst.GeboorteDatum,
					opt => opt.MapFrom(src => src.Persoon.GeboorteDatum))
				.ForMember(
					dst => dst.VolledigeNaam,
					opt => opt.MapFrom(src => src.Persoon.VolledigeNaam))
				.ForMember(
					dst => dst.Geslacht,
					opt => opt.MapFrom(src => src.Persoon.Geslacht));

			Mapper.CreateMap<Lid, LidInfo>()
				.ForMember(
					dst => dst.LidID,
					opt => opt.MapFrom(src => src.ID))
				.ForMember(
					dst => dst.Type,
					opt => opt.MapFrom(src => src is Kind ? LidType.Kind : LidType.Leiding))
				.ForMember(
					dst => dst.AfdelingsNamen,
					opt => opt.MapFrom(src => src.AfdelingsNamenGet()))
				.ForMember(
					dst => dst.PersoonInfo,
					opt => opt.MapFrom(src => src.GelieerdePersoon));
		}
	}
}
