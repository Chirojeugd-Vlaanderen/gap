// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq;

using AutoMapper;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;

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
			Mapper.CreateMap<GelieerdePersoon, PersoonDetail>()
				.ForMember(
					dst => dst.GelieerdePersoonID,
					opt => opt.MapFrom(src => src.ID))
				.ForMember(
					dst => dst.IsLid,
					opt => opt.MapFrom(src => (src.Lid.Any(e => e.Type == LidType.Kind))))
				.ForMember(
					dst => dst.IsLeiding,
					opt => opt.MapFrom(src => (src.Lid.Any(e => e.Type == LidType.Leiding))))
				.ForMember(
					dst => dst.KanLidWorden,
					opt => opt.MapFrom(src => false))
				.ForMember(
					dst => dst.KanLeidingWorden,
					opt => opt.MapFrom(src => false))
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
					dst => dst.ChiroLeefTijd,
					opt => opt.MapFrom(src => src.ChiroLeefTijd))
				.ForMember(
					dst => dst.PersoonID,
					opt => opt.MapFrom(src => src.Persoon.ID))
				.ForMember(
					dst => dst.VoorkeursAdresID,
					opt => opt.MapFrom(src => src.PersoonsAdres == null ? 0 : src.PersoonsAdres.ID))
				.ForMember(
					dst => dst.VolledigeNaam,
					opt => opt.Ignore());

			Mapper.CreateMap<GelieerdePersoon, PersoonOverzicht>()
				.ForMember(dst => dst.AdNummer, opt => opt.MapFrom(src => src.Persoon.AdNummer))
				.ForMember(dst => dst.Bus, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.Bus))
				.ForMember(dst => dst.Email, opt => opt.MapFrom(src => VoorkeurCommunicatie(src, CommunicatieTypeEnum.Email)))
				.ForMember(dst => dst.GeboorteDatum, opt => opt.MapFrom(src => src.Persoon.GeboorteDatum))
				.ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => src.Persoon.Geslacht))
				.ForMember(dst => dst.HuisNummer, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.HuisNr))
				.ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.Persoon.Naam))
				.ForMember(dst => dst.PostNummer, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : (int?)src.PersoonsAdres.Adres.WoonPlaats.PostNummer))
				.ForMember(dst => dst.StraatNaam, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.StraatNaam.Naam))
				.ForMember(dst => dst.TelefoonNummer,
				           opt => opt.MapFrom(src => VoorkeurCommunicatie(src, CommunicatieTypeEnum.TelefoonNummer)))
				.ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.Persoon.VoorNaam))
				.ForMember(dst => dst.WoonPlaats, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null: src.PersoonsAdres.Adres.WoonPlaats.Naam));


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
					dst => dst.AfdelingAfkorting,
					opt => opt.MapFrom(src => src.Afdeling.Afkorting));

            Mapper.CreateMap<Groep, GroepDetail>()
				.ForMember(
					dst => dst.Plaats, opt => opt.MapFrom(
					src => src is ChiroGroep ? (src as ChiroGroep).Plaats : String.Empty))
				.ForMember(
					dst => dst.StamNummer, opt => opt.MapFrom(
					src => src.Code == null ? String.Empty : src.Code.ToUpper()))
				.ForMember(
					dst => dst.Categorieen, opt => opt.MapFrom(
					src => src.Categorie))
				.ForMember(
					dst => dst.Functies, opt => opt.MapFrom(
					src => src.Functie))
				.ForMember(
					dst => dst.Afdelingen, 
					opt => opt.Ignore());

			Mapper.CreateMap<Afdeling, AfdelingInfo>();

			Mapper.CreateMap<Functie, FunctieDetail>();

			Mapper.CreateMap<Lid, LidInfo>()
				.ForMember(
					dst => dst.LidID,
					opt => opt.MapFrom(src => src.ID))
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
					dst => dst.Dubbelpunt,
				// FIXME: geef hier false terug, zodat het in de lidinfo een bool is ipv bool? en het printen
				// in de ui makkelijker is
					opt => opt.MapFrom(src => src is Leiding ? ((Leiding)src).DubbelPuntAbonnement : false))
				.ForMember(
					dst => dst.AfdelingIdLijst,
					opt => opt.MapFrom(src => LedenManager.AfdelingIdLijstGet(src)))
				.ForMember(
					dst => dst.Functies,
					opt => opt.MapFrom(src => src.Functie))
				.ForMember(
					dst => dst.GroepsWerkJaarID,
					opt => opt.MapFrom(src => src.GroepsWerkJaar != null ? src.GroepsWerkJaar.ID : 0))
				.ForMember(
					dst => dst.VerzekeringLoonVerlies,
					opt => opt.MapFrom(src => IsVerzekerd(
						src, 
						Verzekering.LoonVerlies)));

			Mapper.CreateMap<Lid, PersoonLidInfo>()
				.ForMember(
					dst => dst.PersoonDetail,
					opt => opt.MapFrom(src => src.GelieerdePersoon))
				.ForMember(
					dst => dst.LidInfo,
					opt => opt.MapFrom(src => src))
				.ForMember(
					dst => dst.PersoonsAdresInfo,
					opt => opt.MapFrom(null))
				.ForMember(
					dst => dst.CommunicatieInfo,
					opt => opt.MapFrom(null));

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
					dst => dst.PersoonsAdresID,
					opt => opt.MapFrom(src => src.ID))
				.ForMember(
					dst => dst.PostNr,
					opt => opt.MapFrom(src => src.Adres.StraatNaam.PostNummer))
				.ForMember(
					dst => dst.StraatNaamNaam,
					opt => opt.MapFrom(src => src.Adres.StraatNaam.Naam))
				.ForMember(
					dst => dst.WoonPlaatsNaam,
					opt => opt.MapFrom(src => src.Adres.WoonPlaats.Naam));

			Mapper.CreateMap<GelieerdePersoon, BewonersInfo>()
				.ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dst => dst.AdresType, opt => opt.MapFrom(src => AdresTypeEnum.Overig));

			Mapper.CreateMap<PersoonsAdres, BewonersInfo>()
				.ForMember(
					dst => dst.GelieerdePersoonID, 
					opt => opt.MapFrom(src => src.Persoon.GelieerdePersoon.FirstOrDefault() == null ? 0 : src.Persoon.GelieerdePersoon.First().ID));

			// Als de property's van de doelobjecten strategisch gekozen namen hebben, configureert
			// Automapper alles automatisch, zoals hieronder:

			Mapper.CreateMap<StraatNaam, StraatInfo>();
			Mapper.CreateMap<WoonPlaats, WoonPlaatsInfo>();
			Mapper.CreateMap<CommunicatieType, CommunicatieTypeInfo>();
			Mapper.CreateMap<Categorie, CategorieInfo>();
			Mapper.CreateMap<PersoonsAdres, PersoonsAdresInfo2>();
			Mapper.CreateMap<CommunicatieVorm, CommunicatieInfo>();

			Mapper.CreateMap<Groep, GroepInfo>()
				.ForMember(dst => dst.Plaats, opt => opt.MapFrom(
					src => src is ChiroGroep ? (src as ChiroGroep).Plaats : Properties.Resources.NietVanToepassing))
				.ForMember(dst => dst.StamNummer, opt => opt.MapFrom(
					src => src.Code == null ? String.Empty : src.Code.ToUpper()));

			Mapper.CreateMap<CommunicatieInfo, CommunicatieVorm>()
				.ForMember(dst => dst.TeVerwijderen, opt => opt.Ignore())
				.ForMember(dst => dst.Versie, opt => opt.Ignore())
				.ForMember(dst => dst.GelieerdePersoon, opt => opt.Ignore())
				.ForMember(dst => dst.GelieerdePersoonReference, opt => opt.Ignore())
				.ForMember(dst => dst.CommunicatieType, opt => opt.Ignore())
				.ForMember(dst => dst.CommunicatieTypeReference, opt => opt.Ignore())
				.ForMember(dst => dst.EntityKey, opt => opt.Ignore());

			Mapper.CreateMap<AfdelingsJaar, ActieveAfdelingInfo>()
				.ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.Afdeling.Naam))
				.ForMember(dst => dst.Afkorting, opt => opt.MapFrom(src => src.Afdeling.Afkorting))
				.ForMember(dst => dst.AfdelingsJaarID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dst => dst.ID, opt => opt.MapFrom(src => src.Afdeling.ID));

			Mapper.CreateMap<GroepsWerkJaar, WerkJaarInfo>();
			Mapper.CreateMap<AfdelingsJaar, AfdelingsJaarDetail>()
				.ForMember(dst => dst.AfdelingsJaarID, opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<PersoonInfo, Persoon>()
				.ForMember(dst => dst.ID, opt => opt.Ignore())
				.ForMember(dst => dst.TeVerwijderen, opt => opt.Ignore())
				.ForMember(dst => dst.VolledigeNaam, opt => opt.Ignore())
				.ForMember(dst => dst.SterfDatum, opt => opt.Ignore())
				.ForMember(dst => dst.Versie, opt => opt.Ignore())
				.ForMember(dst => dst.GelieerdePersoon, opt => opt.Ignore())
				.ForMember(dst => dst.PersoonsAdres, opt => opt.Ignore())
				.ForMember(dst => dst.EntityKey, opt => opt.Ignore())
				.ForMember(dst => dst.PersoonsVerzekering, opt => opt.Ignore());

			// Important: als er een lid is, dan is er altijd een gelieerdepersoon, maar niet omgekeerd, 
			// dus passen we de link aan in de andere richting!
			// Maar kunnen er meerdere leden zijn?
			Mapper.CreateMap<GelieerdePersoon, PersoonLidInfo>()
				.ForMember(
					dst => dst.PersoonDetail,
					opt => opt.MapFrom(src => src))
				.ForMember(
					dst => dst.PersoonsAdresInfo,
					opt => opt.MapFrom(src => src.Persoon.PersoonsAdres))
				.ForMember(
					dst => dst.CommunicatieInfo,
					opt => opt.MapFrom(src => src.Communicatie))
				.ForMember(
					dst => dst.LidInfo,
					opt => opt.MapFrom(src => src.Lid.FirstOrDefault())); //omdat je altijd maar 1 lid mag opvragen

			#region Mapping van Exceptions naar Faults
			// TODO: Kan het mappen van die generics niet efficienter?

			Mapper.CreateMap<BestaatAlException<Categorie>,
						BestaatAlFault<CategorieInfo>>();
			Mapper.CreateMap<OngeldigObjectException, OngeldigObjectFault>();
			Mapper.CreateMap<BlokkerendeObjectenException<GelieerdePersoon>,
					BlokkerendeObjectenFault<PersoonDetail>>()
				.ForMember(
					dst => dst.Objecten,
					opt => opt.MapFrom(src => src.Objecten.Take(Properties.Settings.Default.KleinAantal)));
			Mapper.CreateMap<BlokkerendeObjectenException<PersoonsAdres>,
					BlokkerendeObjectenFault<PersoonsAdresInfo2>>();
			Mapper.CreateMap<BlokkerendeObjectenException<Lid>,
				BlokkerendeObjectenFault<PersoonLidInfo>>()
				.ForMember(
					dst => dst.Objecten,
					opt => opt.MapFrom(src => src.Objecten.Take(Properties.Settings.Default.KleinAantal)));
			Mapper.CreateMap<BestaatAlException<Afdeling>,
					BestaatAlFault<AfdelingInfo>>();
			#endregion

			// Wel even nakijken of die automagie overal gewerkt heeft:

			Mapper.AssertConfigurationIsValid();
		}

		#region Helperfuncties waarvan ik niet zeker ben of ze hier goed staan.

		/// <summary>
		/// Controleert of een lid <paramref name="src"/>in zijn werkjaar verzekerd is wat betreft de verzekering gegeven
		/// door <paramref name="verzekering"/>.
		/// </summary>
		/// <param name="src">lid waarvan moet nagekeken worden of het verzekerd is</param>
		/// <param name="verzekering">type verzekering waarop gecontroleerd moet worden</param>
		/// <returns><c>true</c> alss het lid een verzekering loonverlies heeft.</returns>
		private static bool IsVerzekerd(Lid src, Verzekering verzekering)
		{
			if (src.GelieerdePersoon == null)
			{
				return false;
			}
			else
			{
				var persoonsverzekeringen = from v in src.GelieerdePersoon.Persoon.PersoonsVerzekering
							    where v.VerzekeringsType.ID == (int)verzekering &&
								  (LedenManager.DatumInWerkJaar(v.Van, src.GroepsWerkJaar.WerkJaar) ||
								   LedenManager.DatumInWerkJaar(v.Tot, src.GroepsWerkJaar.WerkJaar))
							    select v;

				return persoonsverzekeringen.FirstOrDefault() != null;
			}
		}

		/// <summary>
		/// Voorkeurtelefoonnr, voorkeure-maialdres,... van een gelieerde persoon
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <param name="type">Communicatietype waarvan voorkeur gevraagd wordt.</param>
		/// <returns>Voorkeurtelefoonnr, -maiadres,... van de gelieerde persoon.  
		/// <c>null</c> indien onbestaand.</returns>
		private static string VoorkeurCommunicatie(GelieerdePersoon gp, CommunicatieTypeEnum type)
		{
			var query = from c in gp.Communicatie
			             where (c.CommunicatieType.ID == (int)type) && c.Voorkeur
			             select c.Nummer;

			return query.FirstOrDefault();
		}

		#endregion

	}
}
