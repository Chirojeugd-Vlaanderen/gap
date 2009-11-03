using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.ServiceContracts.Mappers
{
	/// <summary>
	/// Klasse voor het mappen van GelieerdePersoon naar PersoonInfo.
	/// Gebruikt automapper.  De statische constructor wordt gebruikt om de mapping te definieren.
	/// </summary>
	public class PersoonInfoMapper
	{
		/// <summary>
		/// Configureert automapper voor de mapping van GelieerdePersoon naar PersoonInfo.
		/// </summary>
		static PersoonInfoMapper()
		{
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
		}

		/// <summary>
		/// Mapt een <c>GelieerdePersoon</c> <paramref name="src" /> naar <c>PersoonInfo</c>
		/// <paramref name="dst" />.
		/// </summary>
		/// <param name="src">te mappen <c>GelieerdePersoon</c></param>
		/// <param name="dst"><c>PersoonInfo</c> voor het resultaat</param>
		public static void Map(GelieerdePersoon src, PersoonInfo dst)
		{
			Mapper.Map(src, dst);
		}

		/// <summary>
		/// Mapt een <c>GelieerdePersoon</c> <paramref name="src" /> naar <c>PersoonInfo</c>
		/// </summary>
		/// <param name="src">te mappen <c>GelieerdePersoon</c></param>
		/// <returns>gemapte <c>PersoonInfo</c>t</returns>
		public static PersoonInfo Map(GelieerdePersoon src)
		{
			PersoonInfo resultaat = new PersoonInfo();
			Map(src, resultaat);
			return resultaat;
		}

		/// <summary>
		/// Mapt een rij gelieerde personen <paramref name="src" /> naar <c>PersoonInfo</c>
		/// </summary>
		/// <param name="src">te mappen gelieerde personen</param>
		/// <returns>een rij gemapte <c>PersoonInfo</c></returns>
		public static IList<PersoonInfo> Map(IEnumerable<GelieerdePersoon> src)
		{
			IList<PersoonInfo> resultaat = new List<PersoonInfo>();
			foreach (var gp in src)
			{
				resultaat.Add(Map(gp));
			}
			return resultaat;
		}

	}
}
