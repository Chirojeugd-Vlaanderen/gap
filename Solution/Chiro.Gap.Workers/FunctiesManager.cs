// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Struct die gebruikt wordt om van een functie max aantal leden, min aantal leden en totaal aantal
	/// leden te stockeren.
	/// </summary>
	public struct Telling
	{
		public int ID;
		public int Aantal;
		public int? Max;
		public int Min;
	}
		
	/// <summary>
	/// Businesslogica ivm functies
	/// </summary>
	public class FunctiesManager
	{
		private const string NATFUNCTIESCACHEKEY = "NatFunctiesCacheKey";

		private readonly IFunctiesDao _funDao;
		private readonly ILedenDao _ledenDao;
		private readonly IGroepsWerkJaarDao _groepsWjDao;
		private readonly IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Instantieert een FunctiesManager-object
		/// </summary>
		/// <param name="funDao">Een dao voor data access mbt functies</param>
		/// <param name="ledenDao">Een dao voor data access mbt leden</param>
		/// <param name="gwjDao">Data access object voor groepswerkjaren</param>
		/// <param name="auMgr">Een IAutorisatieManager voor de autorisatie</param>
		public FunctiesManager(IFunctiesDao funDao, ILedenDao ledenDao, IGroepsWerkJaarDao gwjDao, IAutorisatieManager auMgr)
		{
			_funDao = funDao;
			_ledenDao = ledenDao;
			_groepsWjDao = gwjDao;
			_autorisatieMgr = auMgr;
		}

		/// <summary>
		/// Persisteert de gegeven <paramref name="functie"/> in de database, samen met zijn koppeling
		/// naar groep.
		/// </summary>
		/// <param name="functie">Te persisteren functie</param>
		/// <returns>De bewaarde functie</returns>
		public Functie Bewaren(Functie functie)
		{
			if (NationaalBepaaldeFunctiesOphalen().Contains(functie)
				|| functie.IsNationaal
				|| !_autorisatieMgr.IsGavGroep(functie.Groep.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				return _funDao.Bewaren(functie, fn => fn.Groep.WithoutUpdate());
			}
		}

		/// <summary>
		/// Haalt 1 functie op, samen met de gekoppelde groep
		/// </summary>
		/// <param name="functieID">ID op te halen functie</param>
		/// <returns>De opgehaalde functie met de gekoppelde groep</returns>
		public Functie Ophalen(int functieID)
		{
			return Ophalen(new int[] { functieID }).FirstOrDefault();
		}

		/// <summary>
		/// Een functie ophalen op basis van de ID, samen met de gekoppelde leden
		/// </summary>
		/// <param name="functieID">ID op te halen functie</param>
		/// <param name="metLeden">De opgehaalde functie met de gekoppelde leden</param>
		/// <returns></returns>
		public Functie Ophalen(int functieID, bool metLeden)
		{
			if (_autorisatieMgr.IsGavCategorie(functieID))
			{
				if (metLeden)
				{
					return _funDao.Ophalen(functieID, fnc => fnc.Groep, fie => fie.Lid);
				}
				else
				{
					return _funDao.Ophalen(functieID);
				}
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een lijstje functies op, uiteraard met gekoppelde groepen (indien van toepassing)
		/// </summary>
		/// <param name="functieIDs">ID's op te halen functies</param>
		/// <returns>Lijst opgehaalde functies, met gekoppelde groepen (indien van toepassing)</returns>
		public IList<Functie> Ophalen(IEnumerable<int> functieIDs)
		{
			var resultaat = _funDao.Ophalen(functieIDs, fn => fn.Groep);
			var groepIDs = (from fn in resultaat 
					where fn.Groep != null
					select fn.Groep.ID).Distinct();

			if (groepIDs.Any(id => !_autorisatieMgr.IsGavGroep(id)))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			return resultaat.ToList();
		}

		/// <summary>
		/// Haalt alle functies op die mogelijk toegekend kunnen worden aan een lid uit het groepswerkjaar
		/// bepaald door <paramref name="groepsWerkJaarID"/> en van het type <paramref name="lidType"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor de relevante functies gevraagd
		/// zijn.</param>
		/// <param name="lidType"><c>LidType.Kind</c> of <c>LidType.Leiding</c></param>
		/// <returns>
		/// Lijst met functies die mogelijk toegekend kunnen worden aan een lid uit het groepswerkjaar
		/// bepaald door <paramref name="groepsWerkJaarID"/> en van het type <paramref name="lidType"/>.
		/// </returns>
		public IList<Functie> OphalenRelevant(int groepsWerkJaarID, LidType lidType)
		{
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				GroepsWerkJaar gwj = _groepsWjDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep.Functie);

				// TODO: ook groepsgebonden functies ophalen (refs #103)
				return (from f in gwj.Groep.Functie.Union(NationaalBepaaldeFunctiesOphalen())
					where (f.WerkJaarVan == null || f.WerkJaarVan <= gwj.WerkJaar)
						&& (f.WerkJaarTot == null || f.WerkJaarTot >= gwj.WerkJaar)
						&& ((f.Type & lidType) != 0)
					select f).ToList();
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Kent de meegegeven <paramref name="functies"/> toe aan het gegeven <paramref name="lid"/>.
		/// Als het lid al andere functies had, blijven die behouden.  Persisteert niet.
		/// </summary>
		/// <param name="lid">Lid dat de functies moet krijgen</param>
		/// <param name="functies">Rij toe te kennen functies</param>
		/// <remarks>
		/// Er wordt verondersteld dat er heel wat geladen is!
		/// - lid.groepswerkjaar.groep
		/// - lid.functie
		/// - voor elke functie:
		///   - functie.lid (voor leden van dezelfde groep)
		///   - functie.groep
		/// </remarks>
		public void Toekennen(Lid lid, IEnumerable<Functie> functies)
		{
			Debug.Assert(lid.GroepsWerkJaar != null);
			Debug.Assert(lid.GroepsWerkJaar.Groep != null);

			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Eerst alle checks, zodat als er ergens een exceptie optreedt, er geen enkele
			// functie wordt toegekend.

			foreach (Functie f in functies)
			{
				if (!_autorisatieMgr.IsGavFunctie(f.ID))
				{
					throw new GeenGavException(Properties.Resources.GeenGav);
				}
				if (!f.IsNationaal && f.Groep.ID != lid.GroepsWerkJaar.Groep.ID)
				{
					throw new GapException(
						FoutNummer.FunctieNietVanGroep,
						Properties.Resources.FoutieveGroepFunctie);
				}
				if (f.WerkJaarTot < lid.GroepsWerkJaar.WerkJaar  // false als wjtot null
					|| f.WerkJaarVan > lid.GroepsWerkJaar.WerkJaar)	// false als wjvan null
				{
					throw new GapException(
						FoutNummer.FunctieNietBeschikbaar,
						Properties.Resources.FoutiefGroepsWerkJaarFunctie);
				}

				if ((f.Type & lid.Type) == 0)
				{
					throw new InvalidOperationException(Properties.Resources.FoutiefLidType);
				}

				// Ik test hier bewust niet of er niet te veel leden zijn met de functie;
				// op die manier worden inconsistenties bij het veranderen van functies toegelaten,
				// wat me voor de UI makkelijker lijkt.  De method 'AantallenControleren' kan
				// te allen tijde gebruikt worden om problemen met functieaantallen op te sporen.
			}

			// Alle checks goed overleefd; als we nog niet uit de method 'gethrowd' zijn, kunnen we
			// nu de functies toekennen.

			foreach (var f in functies)
			{
				if ((from fnc in lid.Functie where fnc.ID == f.ID select fnc).FirstOrDefault() == null)
				{
					lid.Functie.Add(f);
				}

				if ((from ld in f.Lid where ld.ID == lid.ID select ld).FirstOrDefault() == null)
				{
					f.Lid.Add(lid);
				}
			}
		}

		/// <summary>
		/// Koppelt de functies met ID's <paramref name="functieIDs"/> los van het lid
		/// <paramref name="lid"/>.  PERSISTEERT.
		/// </summary>
		/// <param name="lid">Lid waarvan functies losgekoppeld moeten worden</param>
		/// <param name="functieIDs">ID's van de los te koppelen functies</param>
		/// <returns>Het lidobject met daaraan gekoppeld de overblijvende functies</returns>
		/// <remarks>
		/// * functie-ID's van functies die niet aan het lid gekoppeld zijn, worden genegeerd.
		/// * er wordt verwacht dat voor elke te verwijderen functie alle leden met groepswerkjaar geladen zijn
		/// * er wordt niet echt losgekoppeld; de koppeling lid-functie wordt op 'te verwijderen'
		///   gezet.  (Wat wil zeggen dat verwijderen via het lid moet gebeuren, en niet via de functie)
		/// (Dat systeem met 'teVerwijderen' is eigenlijk toch verre van ideaal.)
		/// </remarks>
		public Lid LosKoppelen(Lid lid, IEnumerable<int> functieIDs)
		{
			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var losTeKoppelen = (from fun in lid.Functie
					     where functieIDs.Contains(fun.ID)
					     select fun);
			
			foreach (Functie f in losTeKoppelen)
			{
				// Ik test hier bewust niet of er niet te weinig leden zijn met de functie;
				// op die manier worden inconsistenties bij het veranderen van functies toegelaten,
				// wat me voor de UI makkelijker lijkt.  De method 'AantallenControleren' kan
				// te allen tijde gebruikt worden om problemen met functieaantallen op te sporen.

				// De essentie van deze prachtige method:

				f.TeVerwijderen = true;
			}
			return _ledenDao.Bewaren(lid, ld => ld.Functie);
		}

		/// <summary>
		/// Vervangt de functies van het lid <paramref name="lid"/> door de functies in 
		/// <paramref name="functies"/>.  Persisteert.
		/// </summary>
		/// <param name="lid">Lid waarvan de functies vervangen moeten worden</param>
		/// <param name="functies">Nieuwe lijst functies</param>
		/// <returns>Het <paramref name="lid"/> met daaraan gekoppeld de nieuwe functies</returns>
		/// <remarks>Aan <paramref name="lid"/>moeten de huidige functies gekoppeld zijn</remarks>
		public Lid Vervangen(Lid lid, IEnumerable<Functie> functies)
		{
			IList<Functie> toeTeVoegen = (from fn in functies
						      where !lid.Functie.Contains(fn)
						      select fn).ToList();
			IList<int> teVerwijderen = (from fn in lid.Functie
						    where !functies.Contains(fn)
						    select fn.ID).ToList();

			// Exception handling laten we over aan Toekennen en LosKoppelen
			Toekennen(lid, toeTeVoegen);
			return LosKoppelen(lid, teVerwijderen);	// LosKoppelen persisteert
		}

		/// <summary>
		/// Verwijdert een functie (PERSISTEERT!)
		/// </summary>
		/// <param name="functie">Te verwijderen functie, inclusief gelieerde personen</param>
		/// <param name="forceren">Indien <c>true</c> wordt de functie ook verwijderd als er
		/// personen in de functie zitten.  Anders krijg je een exception.</param>
		/// <remarks>Deze method gaat ervan uit dat de functie zijn leden bevat.</remarks>
		public void Verwijderen(Functie functie, bool forceren)
		{
			// Leden moeten gekoppeld zijn
			// (null verschilt hier expliciet van een lege lijst)
			Debug.Assert(functie.Lid  != null);

			if (!forceren && functie.Lid.Count > 0)
			{
				throw new BlokkerendeObjectenException<Lid>(
					FoutNummer.FunctieNietLeeg,
					functie.Lid,
					functie.Lid.Count(),
					Properties.Resources.FunctieNietLeeg);
			}

			LeegMaken(functie);  // verwijdert de functie bij alle leden, en persisteert

			functie.TeVerwijderen = true;	// nu de functie zelf nog
			Bewaren(functie);
		}

		/// <summary>
		/// Verwijdert alle gelieerde personen uit de categorie <paramref name="f"/>, en persisteert
		/// </summary>
		/// <param name="f">Leeg te maken functie</param>
		/// <returns>De functie zonder leden</returns>
		public Functie LeegMaken(Functie f)
		{
			if (_autorisatieMgr.IsGavCategorie(f.ID))
			{
				foreach (Lid l in f.Lid)
				{
					l.TeVerwijderen = true;
					// dit verwijdert enkel de link naar het lid
				}
				return _funDao.Bewaren(f, fie => fie.Lid);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
		/// minimumaantallen van de functies (eigen en nationaal bepaald) niet overschreden zijn.
		/// </summary>
		/// <param name="groepsWerkJaar">Te controleren werkjaar</param>
		/// <returns>Een lijst met tellingsgegevens voor de functies waar de aantallen niet kloppen.</returns>
		/// <remarks>
		/// <para>
		/// Deze functie is zich niet bewust van de aanwezigheid van een database, en verwacht
		/// dat groepsWerkJaar.Groep.Functie en groepsWerkJaar.Lid[i].Functie geladen zijn.
		/// </para>
		/// </remarks>
		public IEnumerable<Telling> AantallenControleren(GroepsWerkJaar groepsWerkJaar)
		{
			var eigenFuncties = from fn in groepsWerkJaar.Groep.Functie select fn;

			return AantallenControleren(
				groepsWerkJaar, 
				eigenFuncties.Union(NationaalBepaaldeFunctiesOphalen()));
		}

		/// <summary>
		/// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
		/// minimumaantallen van de functies <paramref name="functies"/> niet overschreden zijn.
		/// </summary>
		/// <param name="groepsWerkJaar">Te controleren werkjaar</param>
		/// <param name="functies">Functies waarop te controleren</param>
		/// <returns>Een lijst met tellingsgegevens voor de functies waar de aantallen niet kloppen.</returns>
		/// <remarks>
		/// <para>
		/// Deze functie is zich niet bewust van de aanwezigheid van een database, en verwacht
		/// dat groepsWerkJaar.Lid[i].Functie geladen is.
		/// </para>
		/// <para>
		/// Functies in <paramref name="functies"/> waar geen groep aan gekoppeld is, worden als
		/// nationaal bepaalde functies beschouwd.
		/// </para>
		/// <para>
		/// Functies die niet geldig zijn in het gevraagde groepswerkjaar, worden genegeerd
		/// </para>
		/// </remarks>
		public IEnumerable<Telling> AantallenControleren(
			GroepsWerkJaar groepsWerkJaar,
			IEnumerable<Functie> functies)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaar.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				var toegekendeFuncties =
					groepsWerkJaar.Lid.SelectMany(ld => ld.Functie)
						.Distinct()
						.Where(fn => functies.Contains(fn));

				var nietToegekendeFuncties = from fn in functies
							     where !toegekendeFuncties.Contains(fn)
								&& (fn.IsNationaal || fn.Groep == groepsWerkJaar.Groep)
								// bovenstaande vermijdt groepsvreemde functies
							     select fn;

				// toegekende functies waarvan er te veel of te weinig zijn

				var problemenToegekendeFuncties =
					from fn in toegekendeFuncties
					where (fn.WerkJaarVan == null || fn.WerkJaarVan <= groepsWerkJaar.WerkJaar)
						&& (fn.WerkJaarTot == null || fn.WerkJaarTot >= groepsWerkJaar.WerkJaar)
						&& (fn.Lid.Count() > fn.MaxAantal  // geeft false als maxaant == null
							|| fn.Lid.Count() < fn.MinAantal) // geeft false als minaant null
					select new Telling
					{
						ID = fn.ID,
						Aantal = fn.Lid.Count(),
						Max = fn.MaxAantal,
						Min = fn.MinAantal
					};

				// niet-toegekende functies waarvan er te weinig zijn

				var problemenOntbrekendeFuncties =
					from fn in nietToegekendeFuncties
					where (fn.WerkJaarVan == null || fn.WerkJaarVan <= groepsWerkJaar.WerkJaar)
						&& (fn.WerkJaarTot == null || fn.WerkJaarTot >= groepsWerkJaar.WerkJaar)
						&& fn.MinAantal > 0
					select new Telling
					{
						ID = fn.ID,
						Aantal = 0,
						Max = fn.MaxAantal,
						Min = fn.MinAantal
					};

				return (problemenToegekendeFuncties.Union(problemenOntbrekendeFuncties)).ToArray();
			}
		}

		/// <summary>
		/// Geeft de nationaal bepaalde functies terug
		/// </summary>
		/// <returns>De rij nationaal bepaalde functies</returns>
		public IEnumerable<Functie> NationaalBepaaldeFunctiesOphalen()
		{
			var cache = HttpRuntime.Cache;

			if (cache[NATFUNCTIESCACHEKEY] == null)
			{
				cache.Add(
					NATFUNCTIESCACHEKEY,
					_funDao.NationaalBepaaldeFunctiesOphalen(),
					null,
					System.Web.Caching.Cache.NoAbsoluteExpiration,
					new TimeSpan(1, 0, 0, 0) /* bewaar 1 dag */,
					System.Web.Caching.CacheItemPriority.Low,
					null);
			}

			return cache[NATFUNCTIESCACHEKEY] as IEnumerable<Functie>;
		}
	}
}
