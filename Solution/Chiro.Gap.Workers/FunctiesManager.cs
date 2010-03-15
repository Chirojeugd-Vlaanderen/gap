// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Cdf.Validation;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using System.Web;

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
		public int Max;
		public int Min;
	}
		
	/// <summary>
	/// Businesslogica ivm functies
	/// </summary>
	public class FunctiesManager
	{
		private IFunctiesDao _funDao;
		private ILedenDao _ledenDao;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Instantieert een FunctiesManager-object
		/// </summary>
		/// <param name="funDao">Een dao voor data access mbt functies</param>
		/// <param name="ledenDao">Een dao voor data access mbt leden</param>
		/// <param name="auMgr">Een IAutorisatieManager voor de autorisatie</param>
		public FunctiesManager(IFunctiesDao funDao, ILedenDao ledenDao, IAutorisatieManager auMgr)
		{
			_funDao = funDao;
			_ledenDao = ledenDao;
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
				|| functie.Groep == null
				|| !_autorisatieMgr.IsGavGroep(functie.Groep.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavFunctie);
			}
			else
			{
				return _funDao.Bewaren(functie, fn => fn.Groep);
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
		/// - lid.gelieerdePersoon.Persoon (voor leeftijd)
		/// - voor elke functie:
		///   - functie.lid (voor leden van dezelfde groep)
		///   - functie.groep
		/// </remarks>
		public void Toekennen(Lid lid, IEnumerable<Functie> functies)
		{
			Debug.Assert(lid.GelieerdePersoon != null);
			Debug.Assert(lid.GelieerdePersoon.Persoon != null);
			Debug.Assert(lid.GroepsWerkJaar != null);
			Debug.Assert(lid.GroepsWerkJaar.Groep != null);

			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}

			foreach (Functie f in functies)
			{
				if (!_autorisatieMgr.IsGavFunctie(f.ID))
				{
					throw new GeenGavException(Properties.Resources.GeenGavFunctie);
				}
				if (!IsNationaalBepaald(f) && f.Groep.ID != lid.GroepsWerkJaar.Groep.ID)
				{
					throw new FoutieveGroepException(Properties.Resources.FoutieveGroepFunctie);
				}
				if (f.MinLeefTijd != 0)
				{
					// leeftijdsconstraints op functie.  Check geboortedatum

					Debug.Assert(lid.GelieerdePersoon.Persoon != null);

					if (lid.GelieerdePersoon.Persoon.GeboorteDatum == null)
					{
						// geboortedatum ontbreekt

						// TODO: custom exception?
						throw new InvalidOperationException(
							Properties.Resources.GeboorteDatumOntbreekt);
					}
					else if (DatumHelper.LeefTijd(
						(DateTime)lid.GelieerdePersoon.Persoon.GeboorteDatum) < f.MinLeefTijd)
					{
						// is persoon oud genoeg?

						// TODO: Custom exception?
						throw new InvalidOperationException(Properties.Resources.TeJong);
					}
				}

				// Ik test hier bewust niet of er niet te veel leden zijn met de functie;
				// op die manier worden inconsistenties bij het veranderen van functies toegelaten,
				// wat me voor de UI makkelijker lijkt.  De method 'AantallenControleren' kan
				// te allen tijde gebruikt worden om problemen met functieaantallen op te sporen.

				// Alle checks goed overleefd; functie koppelen als dat nog niet
				// gebeurd moest zijn.

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
				throw new GeenGavException(Properties.Resources.GeenGavLid);
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
		/// Controleert of de functie <paramref name="f"/> nationaal bepaald is.
		/// </summary>
		/// <param name="f">Te controleren functie</param>
		/// <returns><c>true</c> enkel als de functie nationaal bepaald is</returns>
		private bool IsNationaalBepaald(Functie f)
		{
			// FIXME: Dit is nogal gevaarlijk, want wie weet is de groep gewoon niet gefetcht.

			return f.Groep == null;
		}

		/// <summary>
		/// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
		/// minimumaantallen van de functies (eigen en nationaal bepaald) niet overschreden zijn.
		/// </summary>
		/// <param name="groepsWerkJaar">te controleren werkjaar</param>
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
		/// <param name="groepsWerkJaar">te controleren werkjaar</param>
		/// <param name="functies">functies waarop te controleren</param>
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
		/// </remarks>
		public IEnumerable<Telling> AantallenControleren(
			GroepsWerkJaar groepsWerkJaar,
			IEnumerable<Functie> functies)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaar.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}
			else
			{
				var toegekendeFuncties =
					groepsWerkJaar.Lid.SelectMany(ld => ld.Functie)
						.Distinct()
						.Where(fn => functies.Contains(fn));

				var nietToegekendeFuncties = from fn in functies
							     where !toegekendeFuncties.Contains(fn)
								&& (fn.Groep == null || fn.Groep == groepsWerkJaar.Groep)
								// bovenstaande vermijdt groepsvreemde functies
							     select fn;

				// toegekende functies waarvan er te veel of te weinig zijn

				var problemenToegekendeFuncties =
					from fn in toegekendeFuncties
					where fn.MaxAantal > 0 && fn.Lid.Count() > fn.MaxAantal
						|| fn.MinAantal > 0 && fn.Lid.Count() < fn.MinAantal
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
					where fn.MinAantal > 0
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

			if (cache[Properties.Settings.Default.NatFunctiesCacheKey] == null)
			{
				cache.Insert(
					Properties.Settings.Default.NatFunctiesCacheKey,
					_funDao.NationaalBepaaldeFunctiesOphalen());
			}

			return cache[Properties.Settings.Default.NatFunctiesCacheKey] as IEnumerable<Functie>;
		}
	}
}
