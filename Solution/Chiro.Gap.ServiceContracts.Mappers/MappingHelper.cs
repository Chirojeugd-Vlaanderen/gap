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
					dst => dst.CategorieLijst,
					opt => opt.MapFrom(src => src.CategorieLijstGet()))
				.ForMember(
					dst => dst.Geslacht,
					opt => opt.MapFrom(src => src.Persoon.Geslacht))
				.ForMember(
					dst => dst.PersoonID,
					opt => opt.MapFrom(src => src.Persoon.ID));

            Mapper.CreateMap<AfdelingsJaar, AfdelingInfo>()
                .ForMember(
                    dst => dst.ID,
                    opt => opt.MapFrom(src => src.Afdeling.ID))
                .ForMember(
                    dst => dst.AfdelingsJaarID,
                    opt => opt.MapFrom(src => src.ID))
                .ForMember(
                    dst => dst.OfficieleAfdelingNaam,
                    opt => opt.MapFrom(src => src.OfficieleAfdeling.Naam))
                .ForMember(
                    dst => dst.Naam,
                    opt => opt.MapFrom(src => src.Afdeling.Naam))
                .ForMember(
                    dst => dst.AfdelingsJaarMagVerwijderdWorden,
                    opt => opt.MapFrom(src => src.Leiding.Count + src.Kind.Count > 0 ? false : true))
                .ForMember(
                    dst => dst.Afkorting,
                    opt => opt.MapFrom(src => src.Afdeling.Afkorting));

            Mapper.CreateMap<Lid, LidInfo>()
				.ForMember(
					dst => dst.LidID,
					opt => opt.MapFrom(src => src.ID))
				.ForMember(
					dst => dst.Type,
					opt => opt.MapFrom(src => src is Kind ? LidType.Kind : LidType.Leiding))
                .ForMember(
                    dst => dst.AfdelingIdLijst,
                    opt => opt.MapFrom(src => src.AfdelingIdLijstGet()))
                .ForMember(
					dst => dst.PersoonInfo,
					opt => opt.MapFrom(src => src.GelieerdePersoon));

			Mapper.CreateMap<Adres, AdresInfo>()
				.ForMember(
					dst => dst.Gemeente,
					opt => opt.MapFrom(src => src.Subgemeente.Naam))
				.ForMember(
					dst => dst.Straat,
					opt => opt.MapFrom(src => src.Straat.Naam))
				.ForMember(
					dst => dst.HuisNr,
					opt => opt.MapFrom(src => src.HuisNr))
				.ForMember(
					dst => dst.PostNr,
					opt => opt.MapFrom(src => src.Subgemeente.PostNr))
				.ForMember(
					dst => dst.Bewoners,
					opt => opt.MapFrom(src => src.PersoonsAdres.Select(x => x.Persoon).ToList()))
				.ForMember(
					dst => dst.Bus,
					opt => opt.MapFrom(src => src.Bus))
				.ForMember(
					dst => dst.ID,
					opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<Persoon, GewonePersoonInfo>()
				.ForMember(
					dst => dst.AdNummer,
					opt => opt.MapFrom(src => src.AdNummer))
				.ForMember(
					dst => dst.GeboorteDatum,
					opt => opt.MapFrom(src => src.GeboorteDatum))
				.ForMember(
					dst => dst.VolledigeNaam,
					opt => opt.MapFrom(src => src.VolledigeNaam))
				.ForMember(
					dst => dst.Geslacht,
					opt => opt.MapFrom(src => src.Geslacht))
				.ForMember(
					dst => dst.PersoonID,
					opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<Straat, StraatInfo>()
				.ForMember(
					dst => dst.PostNr,
					opt => opt.MapFrom(src => src.PostNr))
				.ForMember(
					dst => dst.Naam,
					opt => opt.MapFrom(src => src.Naam));

			Mapper.CreateMap<Subgemeente, GemeenteInfo>()
				.ForMember(
					dst => dst.PostNr,
					opt => opt.MapFrom(src => src.PostNr))
				.ForMember(
					dst => dst.Naam,
					opt => opt.MapFrom(src => src.Naam));
		}
	}
}
