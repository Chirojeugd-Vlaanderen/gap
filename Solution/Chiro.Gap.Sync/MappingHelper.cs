using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.Sync.SyncService;

using Adres = Chiro.Gap.Orm.Adres;
using Persoon = Chiro.Gap.Orm.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse om mappings naar de datacontracts voor de SyncService te definieren.
	/// </summary>
	public static class MappingHelper
	{
		public static void MappingsDefinieren()
		{
			Mapper.CreateMap<Persoon, SyncService.Persoon>()
				.ForMember(dst => dst.ExtensionData, opt => opt.Ignore()); // Members met dezelfde naam mappen automatisch

			Mapper.CreateMap<Adres, SyncService.Adres>()
				.ForMember(dst => dst.ExtensionData, opt => opt.Ignore())
				.ForMember(dst => dst.Land, opt => opt.Ignore()) // TODO (#238): buitenlandse adressen
				.ForMember(dst => dst.PostNr, opt => opt.MapFrom(src => src.StraatNaam.PostNummer))
				.ForMember(dst => dst.Straat, opt => opt.MapFrom(src => src.StraatNaam.Naam))
				.ForMember(dst => dst.WoonPlaats, opt => opt.MapFrom(src => src.WoonPlaats.Naam));

			Mapper.CreateMap<CommunicatieVorm, CommunicatieMiddel>()
				.ForMember(dst => dst.ExtensionData, opt => opt.Ignore())
				.ForMember(dst => dst.GeenMailings, opt => opt.MapFrom(src => !src.IsVoorOptIn))
				.ForMember(dst => dst.Type, opt => opt.MapFrom(src => (SyncService.CommunicatieType) src.CommunicatieType.ID))
				.ForMember(dst => dst.Waarde, opt => opt.MapFrom(src => src.Nummer));

			Mapper.CreateMap<GelieerdePersoon, SyncService.PersoonDetails>()
				.ForMember(dst => dst.Persoon, opt => opt.MapFrom(src => src.Persoon))
				.ForMember(dst => dst.Adres, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres))
				.ForMember(dst => dst.AdresType,
				           opt =>
				           opt.MapFrom(
				           	src =>
				           	src.PersoonsAdres == null
				           		? AdresTypeEnum.ANDER
				           		: (SyncService.AdresTypeEnum) src.PersoonsAdres.AdresType))
				.ForMember(dst => dst.Communicatie, opt => opt.MapFrom(src => src.Communicatie))
				.ForMember(dst => dst.ExtensionData, opt => opt.Ignore());

			Mapper.AssertConfigurationIsValid();
		}
	}
}
