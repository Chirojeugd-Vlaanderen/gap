// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor leden
	/// </summary>
	public class LedenDao : Dao<Lid, ChiroGroepEntities>, ILedenDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor leden
		/// </summary>
		public LedenDao()
		{
			connectedEntities = new Expression<Func<Lid, object>>[] 
            { 
				e => e.GroepsWerkJaar.WithoutUpdate(), 
				e => e.GelieerdePersoon.Persoon.WithoutUpdate()
            };
		}

		/// <summary>
		/// Zoekt lid op op basis van GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			return Ophalen(gelieerdePersoonID, groepsWerkJaarID, getConnectedEntities());
		}

		/// <summary>
		/// Haalt lid met gerelateerde entity's op, op basis van 
		/// GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <param name="paths">Lambda-expressies die de extra op te halen
		/// informatie definieren</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			using (var db = new ChiroGroepEntities())
			{
				var query = (from l in db.Lid
							 where l.GelieerdePersoon.ID == gelieerdePersoonID
							 && l.GroepsWerkJaar.ID == groepsWerkJaarID
							 select l) as ObjectQuery<Lid>;
				return (IncludesToepassen(query, paths).FirstOrDefault());
			}
		}

		/// <summary>
		/// Sorteert een lijst van leiding. Eerst volgens de gegeven ordening, dan steeds op naam.
		/// <para />
		/// De sortering is vrij complex om met meerdere opties rekening te houden.
		/// <para />
		/// Steeds wordt eerst gesorteerd op lege velden/gevulde velden, de lege komen laatst.
		/// Dan wordt gesorteerd op "sortering"
		///		Naam => Naam+Voornaam
		///		Afdeling => Naam van de afdeling van het afdelingsjaar dat eerst in de lijst staat #TODO dit is mss niet optimaal
		///		Leeftijd => Op leeftijd, jongste eerst
		/// Dan worden overblijvende gelijke records op naam+voornaam gesorteerd
		/// </summary>
		/// <param name="lijst">De te sorteren lijst</param>
		/// <param name="sortering">Hoe te sorteren</param>
		/// <returns>De gesorteerde lijst!!! In place sorteren lijkt niet mogelijk!!!</returns>
		private static List<Leiding> SorteerLijst(IEnumerable<Leiding> lijst, LedenSorteringsEnum sortering)
		{
			IEnumerable<Leiding> lijst2;
			switch (sortering)
			{
				case LedenSorteringsEnum.Naam:
					lijst2 = lijst.OrderBy(gp => String.Format(
									"{0} {1}",
									gp.GelieerdePersoon.Persoon.Naam,
									gp.GelieerdePersoon.Persoon.VoorNaam));
					break;
				case LedenSorteringsEnum.Leeftijd:
					lijst2 = lijst
						.OrderBy(gp => gp.GelieerdePersoon.Persoon.GeboorteDatum == null)
						.ThenByDescending(gp => gp.GelieerdePersoon.Persoon.GeboorteDatum)
						.ThenBy(gp => String.Format(
							"{0} {1}",
							gp.GelieerdePersoon.Persoon.Naam,
							gp.GelieerdePersoon.Persoon.VoorNaam));
					break;
				case LedenSorteringsEnum.Afdeling:
					lijst2 = lijst
						.OrderBy(gp => gp.AfdelingsJaar.FirstOrDefault() == null)
						.ThenBy(
							gp => (gp.AfdelingsJaar.FirstOrDefault() == null ? null : gp.AfdelingsJaar.First().Afdeling.Naam))
						.ThenBy(gp => String.Format(
							"{0} {1}",
							gp.GelieerdePersoon.Persoon.Naam,
							gp.GelieerdePersoon.Persoon.VoorNaam));
					break;
				default: // Stom dat C# niet kan detecteren dat alle cases gecontroleerd zijn?
					lijst2 = new List<Leiding>();
					break;
			}
			return lijst2.ToList();
		}

		/// <summary>
		/// Sorteert een lijst van kinderen. Eerst volgens de gegeven ordening, dan steeds op naam.
		/// <para />
		/// De sortering is vrij complex om met meerdere opties rekening te houden.
		/// <para />
		/// Steeds wordt eerst gesorteerd op lege velden/gevulde velden, de lege komen laatst.
		/// Dan wordt gesorteerd op "sortering"
		///		Naam => Naam+Voornaam
		///		Afdeling => Naam van de afdeling van het afdelingsjaar dat eerst in de lijst staat #TODO dit is mss niet optimaal
		///		Leeftijd => Op leeftijd, jongste eerst
		/// Dan worden overblijvende gelijke records op naam+voornaam gesorteerd
		/// </summary>
		/// <param name="lijst">De te sorteren lijst</param>
		/// <param name="sortering">Hoe te sorteren</param>
		/// <returns>De gesorteerde lijst!!! In place sorteren lijkt niet mogelijk!!!</returns>
		private static List<Kind> SorteerLijst(IEnumerable<Kind> lijst, LedenSorteringsEnum sortering)
		{
			IEnumerable<Kind> lijst2;
			switch (sortering)
			{
				case LedenSorteringsEnum.Naam:
					lijst2 = lijst.OrderBy(gp => String.Format(
									"{0} {1}",
									gp.GelieerdePersoon.Persoon.Naam,
									gp.GelieerdePersoon.Persoon.VoorNaam));
					break;
				case LedenSorteringsEnum.Leeftijd:
					lijst2 = lijst
						.OrderBy(gp => gp.GelieerdePersoon.Persoon.GeboorteDatum == null)
						.ThenByDescending(gp => gp.GelieerdePersoon.Persoon.GeboorteDatum)
						.ThenBy(gp => String.Format(
							"{0} {1}",
							gp.GelieerdePersoon.Persoon.Naam,
							gp.GelieerdePersoon.Persoon.VoorNaam));
					break;
				case LedenSorteringsEnum.Afdeling:
					lijst2 = lijst
						.OrderBy(gp => gp.AfdelingsJaar == null)
						.ThenBy(gp => (gp.AfdelingsJaar.Afdeling.Naam))
						.ThenBy(gp => String.Format(
							"{0} {1}",
							gp.GelieerdePersoon.Persoon.Naam,
							gp.GelieerdePersoon.Persoon.VoorNaam));
					break;
				default: // Stom dat C# niet kan detecteren dat alle cases gecontroleerd zijn?
					lijst2 = lijst;
					break;
			}
			return lijst2.ToList();
		}

		/// <summary>
		/// Maak een lijst van de gegeven ingeschreven leden (kinderen en leiding),
		/// gesorteerd volgens de opgegeven parameter
		/// </summary>
		/// <param name="kinderen">De lijst van kinderen</param>
		/// <param name="leiding">De lijst van leiders en/of leidsters</param>
		/// <param name="sortering">De parameter waarop de samengestelde lijst gesorteerd moet worden</param>
		/// <returns>De samengestelde en daarna gesorteerde lijst</returns>
		private static List<Lid> MaakLedenLijst(List<Kind> kinderen, List<Leiding> leiding, LedenSorteringsEnum sortering)
		{
			kinderen = SorteerLijst(kinderen, sortering);
			leiding = SorteerLijst(leiding, sortering);

			var lijst = new List<Lid>();
			lijst.AddRange(leiding.Cast<Lid>());
			lijst.AddRange(kinderen.Cast<Lid>());

			return lijst.OrderBy(e => e.NonActief).ToList();
		}

		/// <summary>
		/// Een lijst ophalen van alle leden voor het opgegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <returns>Een lijst alle leden voor het opgegeven groepswerkjaar</returns>
		public IList<Lid> AllesOphalen(int groepsWerkJaarID, LedenSorteringsEnum sortering)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				return MaakLedenLijst(kinderen, leiding, sortering);
			}
		}

		/// <summary>
		/// Haalt een pagina op van de gevraagde gegevens:
		/// leden van een bepaalde groep in een gegeven werkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
		/// <returns>De leden die de groep in dat werkjaar heeft/had</returns>
		/// <remarks>
		/// Pagineren gebeurt per werkjaar.
		/// De parameters pagina, paginaGrootte en aantalTotaal zijn hier niet nodig
		/// omdat alle leden van dat werkjaar samen getoond worden.
		/// </remarks>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, LedenSorteringsEnum sortering)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				return MaakLedenLijst(kinderen, leiding, sortering);
			}
		}

		/// <summary>
		/// Haalt een pagina op van de gevraagde gegevens:
		/// leden van een bepaalde groep in een gegeven werkjaar, die in de gegeven afdeling zitten
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
		/// <param name="afdelingsID">ID van de afdeling waar de leden in moeten zitten</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
		/// <returns>De leden die de groep in dat werkjaar heeft/had en die in de gegeven afdeling zitten/zaten</returns>
		/// <remarks>
		/// Pagineren gebeurt per werkjaar.
		/// </remarks>
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LedenSorteringsEnum sortering)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.AfdelingsJaar.Afdeling.ID == afdelingsID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.AfdelingsJaar.Any(x => x.Afdeling.ID == afdelingsID)
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				return MaakLedenLijst(kinderen, leiding, sortering);
			}
		}

		/// <summary>
		/// Haalt een pagina op van de gevraagde gegevens:
		/// leden van een bepaalde groep in een gegeven werkjaar, die een bepaalde functie hebben/hadden
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
		/// <param name="functieID">ID van de functie die de leden moeten hebben</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
		/// <returns>De leden met de gegeven functie die de groep in dat werkjaar heeft/had</returns>
		/// <remarks>
		/// Pagineren gebeurt per werkjaar.
		/// Haalt GEEN afdeling mee op (nakijken of dit ook effectief niet nodig is?)
		/// </remarks>
		public IList<Lid> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID, LedenSorteringsEnum sortering)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.Functie.Any(e => e.ID == functieID)
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.Functie.Any(e => e.ID == functieID)
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				return MaakLedenLijst(kinderen, leiding, sortering);
			}
		}

		/// <summary>
		/// Lid met afdelingsjaren, afdelingen en gelieerdepersoon
		/// </summary>
		/// <param name="lidID">ID van het lid waarvan we gegevens willen opvragen</param>
		/// <returns>Een lid met afdelingsjaren, afdelingen en gelieerdepersoon</returns>
		public Lid OphalenMetDetails(int lidID)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var lid = (
			from t in db.Lid.Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("Functie")
			where t.ID == lidID
			select t).FirstOrDefault();

				if (lid is Kind)
				{
					return (
						from t in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling").Include("Functie")
						where t.ID == lidID
						select t).FirstOrDefault();
				}
				if (lid is Leiding)
				{
					return (
						from t in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling").Include("Functie")
						where t.ID == lidID
						select t).FirstOrDefault();
				}
				return lid;
			}
		}

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben
		/// </summary>
		/// <param name="functieID">ID van een functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben.</returns>
		public IList<Lid> OphalenUitFunctie(int functieID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			IList<Lid> result;
			using (var db = new ChiroGroepEntities())
			{
				var query = (from lid in db.Lid
							 where lid.Functie.Any(fnc => fnc.ID == functieID) &&
							lid.GroepsWerkJaar.ID == groepsWerkJaarID
							 select lid) as ObjectQuery<Lid>;
				result = (IncludesToepassen(query, paths)).ToList();
			}

			return Utility.DetachObjectGraph(result);
		}

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.
		/// </summary>
		/// <param name="f">Type gepredefinieerde functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.</returns>
		public IList<Lid> OphalenUitFunctie(NationaleFunctie f, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			return OphalenUitFunctie((int)f, groepsWerkJaarID, paths);
		}

		/// <summary>
		/// Geeft <c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c>
		/// </summary>
		/// <param name="lidID">ID van lid waarvoor na te gaan of het al dan niet leiding is</param>
		/// <returns><c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c></returns>
		public bool IsLeiding(int lidID)
		{
			using (var db = new ChiroGroepEntities())
			{
				Lid l = (from ld in db.Lid
						 where ld.ID == lidID
						 select ld).FirstOrDefault();
				return (l is Leiding);
			}
		}

		/// <summary>
		/// Haalt het lid op bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon waarvoor het lidobject gevraagd is.</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar in hetwelke het lidobject gevraagd is</param>
		/// <returns>
		/// Het lid bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </returns>
		public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var lid = (
					from t in db.Lid
					where t.GelieerdePersoon.ID == gelieerdePersoonID
							&&
							t.GroepsWerkJaar.ID == groepsWerkJaarID
					select t).FirstOrDefault();

				if (lid != null)
				{
					int lidID = lid.ID;

					if (lid is Kind)
					{
						return (
							from t in db.Lid.OfType<Kind>()
								.Include("GelieerdePersoon.Persoon")
								.Include("GroepsWerkJaar")
								.Include("AfdelingsJaar.Afdeling")
								.Include(knd => knd.Functie)
							where t.ID == lidID
							select t).FirstOrDefault();
					}
					if (lid is Leiding)
					{
						return (
							from t in db.Lid.OfType<Leiding>()
								.Include("GelieerdePersoon.Persoon")
								.Include("GroepsWerkJaar")
								.Include("AfdelingsJaar.Afdeling")
								.Include(leid => leid.Functie)
							where t.ID == lidID
							select t).FirstOrDefault();
					}
				}
				return lid;
			}
		}
	}
}
