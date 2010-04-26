// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers.Exceptions;

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
					opt => opt.MapFrom(src => (src.Lid.Count > 0)))
				.ForMember(
					dst => dst.AdNummer,
					opt => opt.MapFrom(src => src.Persoon.AdNummer))
				.ForMember(
					dst => dst.GeboorteDatum,
					opt => opt.MapFrom(src => src.Persoon.GeboorteDatum))
				.ForMember(
					dst => dst.Naam,
					opt => opt.MapFrom(src => src.Persoon.Naam))
				.ForMember(
					dst => dst.VoorNaam,
					opt => opt.MapFrom(src => src.Persoon.VoorNaam))
				.ForMember(
					dst => dst.CategorieLijst,
					opt => opt.MapFrom(src => src.CategorieLijstGet()))
				.ForMember(
					dst => dst.Geslacht,
					opt => opt.MapFrom(src => src.Persoon.Geslacht))
				.ForMember(
					dst => dst.ChiroLeeftijd,
					opt => opt.MapFrom(src => src.ChiroLeefTijd))
				.ForMember(
					dst => dst.PersoonID,
					opt => opt.MapFrom(src => src.Persoon.ID))
				.ForMember(
					dst => dst.VolledigeNaam,
					opt => opt.Ignore());	// @Broes: anders werkt het bij mij niet meer

			Mapper.CreateMap<AfdelingsJaar, AfdelingDetail>()
				.ForMember(
				dst => dst.AfdelingsJaarID,
				opt => opt.MapFrom(src => src.ID))
				.ForMember(
				dst => dst.OfficieleAfdelingNaam,
				opt => opt.MapFrom(src => src.OfficieleAfdeling.Naam))
				.ForMember(
				dst => dst.AfdelingNaam,
				opt => opt.MapFrom(src => src.Afdeling.Naam))
				.ForMember(
				dst => dst.IsLeeg,
				opt => opt.MapFrom(src => (src.Kind == null && src.Leiding == null) || (src.Kind != null && src.Leiding != null && src.Kind.Count + src.Leiding.Count == 0)))
				.ForMember(
				dst => dst.AfdelingAfkorting,
				opt => opt.MapFrom(src => src.Afdeling.Afkorting));

			Mapper.CreateMap<Afdeling, AfdelingInfo>();

			Mapper.CreateMap<Functie, FunctieInfo>();

			Mapper.CreateMap<Lid, LidInfo>()
				.ForMember(
					dst => dst.LidID,
					opt => opt.MapFrom(src => src.ID))
				.ForMember(
					dst => dst.PersoonInfo,
					opt => opt.MapFrom(src => src.GelieerdePersoon == null ? null : src.GelieerdePersoon))
				.ForMember(
					dst => dst.Type,
					opt => opt.MapFrom(src => src is Kind ? LidType.Kind : LidType.Leiding))
				.ForMember(
					dst => dst.NonActief,
					opt => opt.MapFrom(src => src.NonActief))
				.ForMember(
					dst => dst.LidgeldBetaald,
					opt => opt.MapFrom(src => src.LidgeldBetaald))
				.ForMember(
					dst => dst.DubbelPunt,
				// FIXME: geef hier false terug, zodat het in de lidinfo een bool is ipv bool? en het printen
				// in de ui makkelijker is
					opt => opt.MapFrom(src => src is Leiding ? ((Leiding)src).DubbelPuntAbonnement : false))
				.ForMember(
					dst => dst.AfdelingIdLijst,
					opt => opt.MapFrom(src => src.AfdelingIdLijstGet()))
				.ForMember(
					dst => dst.Functies,
					opt => opt.MapFrom(src => src.Functie))
				.ForMember(
					dst => dst.GroepsWerkJaarID,
					opt => opt.MapFrom(src => src.GroepsWerkJaar.ID));

			// Zo veel mogelijk automatisch mappen
			Mapper.CreateMap<Adres, AdresInfo>()
				.ForMember(
					dst => dst.PostNr,
					opt => opt.MapFrom(src => src.WoonPlaats.PostNummer));

			Mapper.CreateMap<Adres, GezinInfo>()
				.ForMember(
					dst => dst.PostNr,
					opt => opt.MapFrom(src => src.WoonPlaats.PostNummer))
				.ForMember(
					dst => dst.Bewoners,
					opt => opt.MapFrom(src => src.PersoonsAdres.ToList()));

			// Domme mapping
			Mapper.CreateMap<PersoonsAdres, PersoonsAdresInfo>()
				.ForMember(
					dst => dst.Bus,
					opt => opt.MapFrom(src => src.Adres.Bus))
				.ForMember(
					dst => dst.HuisNr,
					opt => opt.MapFrom(src => src.Adres.HuisNr))
				.ForMember(
					dst => dst.ID,
					opt => opt.MapFrom(src => src.Adres.ID))
				.ForMember(
					dst => dst.PostNr,
					opt => opt.MapFrom(src => src.Adres.StraatNaam.PostNummer))
				.ForMember(
					dst => dst.StraatNaamNaam,
					opt => opt.MapFrom(src => src.Adres.StraatNaam.Naam))
				.ForMember(
					dst => dst.WoonPlaatsID,
					opt => opt.MapFrom(src => src.Adres.WoonPlaats.ID))
				.ForMember(
					dst => dst.WoonPlaatsNaam,
					opt => opt.MapFrom(src => src.Adres.WoonPlaats.Naam));

			Mapper.CreateMap<Persoon, BewonersInfo>()
				.ForMember(
					dst => dst.PersoonAdNummer,
					opt => opt.MapFrom(src => src.AdNummer))
				.ForMember(
					dst => dst.PersoonGeboorteDatum,
					opt => opt.MapFrom(src => src.GeboorteDatum))
				.ForMember(
					dst => dst.PersoonVolledigeNaam,
					opt => opt.MapFrom(src => src.VolledigeNaam))
				.ForMember(
					dst => dst.PersoonGeslacht,
					opt => opt.MapFrom(src => src.Geslacht))
				.ForMember(
					dst => dst.PersoonID,
					opt => opt.MapFrom(src => src.ID))
				.ForMember(dst => dst.AdresType, opt => opt.MapFrom(src => AdresTypeEnum.Overig));

			// Als de property's van de doelobjecten strategisch gekozen namen hebben, configureert
			// Automapper alles automatisch, zoals hieronder:

			Mapper.CreateMap<PersoonsAdres, BewonersInfo>();
			Mapper.CreateMap<StraatNaam, StraatInfo>();
			Mapper.CreateMap<WoonPlaats, WoonPlaatsInfo>();
			Mapper.CreateMap<Categorie, CategorieInfo>();
			Mapper.CreateMap<PersoonsAdres, PersoonsAdresInfo2>();
			Mapper.CreateMap<CommunicatieVorm, CommunicatieInfo>();


			// Important: als er een lid is, dan is er altijd een gelieerdepersoon, maar niet omgekeerd, 
			// dus passen we de link aan in de andere richting!
			Mapper.CreateMap<GelieerdePersoon, PersoonLidInfo>()
				.ForMember(
					dst => dst.PersoonInfo,
					opt => opt.MapFrom(src => src))
				.ForMember(
					dst => dst.PersoonsAdresInfo,
					opt => opt.MapFrom(src => src.Persoon.PersoonsAdres))
				.ForMember(
					dst => dst.CommunicatieInfo,
					opt => opt.MapFrom(src => src.Communicatie))
				.ForMember(
					dst => dst.LidInfo,
					opt => opt.MapFrom(null));

			#region Mapping van Exceptions naar Faults
			// TODO: Kan het mappen van die generics niet efficienter?

			Mapper.CreateMap<BestaatAlException<Categorie>,
						BestaatAlFault<CategorieInfo>>();
			Mapper.CreateMap<OngeldigObjectException, OngeldigObjectFault>();
			Mapper.CreateMap<BlokkerendeObjectenException<GelieerdePersoon>,
					BlokkerendeObjectenFault<PersoonInfo>>()
				.ForMember(
					dst => dst.Objecten,
					opt => opt.MapFrom(src => src.Objecten.Take(Properties.Settings.Default.KleinAantal)));
			Mapper.CreateMap<BlokkerendeObjectenException<PersoonsAdres>,
					BlokkerendeObjectenFault<PersoonsAdresInfo2>>();
			Mapper.CreateMap<BestaatAlException<Afdeling>,
					BestaatAlFault<AfdelingInfo>>();
			#endregion

			// Wel even nakijken of die automagie overal gewerkt heeft:

			Mapper.AssertConfigurationIsValid();
		}
	}
}
