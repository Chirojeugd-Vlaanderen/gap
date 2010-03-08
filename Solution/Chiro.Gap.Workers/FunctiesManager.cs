// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Cdf.Validation;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Businesslogica ivm functies
	/// </summary>
	public class FunctiesManager
	{
		private IFunctiesDao _funDao;
		private ILedenDao _ledenDao;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Instantieert een FunctiesManager
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
		/// Kent de meegegeven <paramref name="functies"/> toe aan het gegeven <paramref name="lid"/>.
		/// Als het lid al andere functies had, blijven die behouden.
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
				if (f.MaxAantal > 0)
				{
					// We verwachten dat f.Lid enkel leden uit het groepswerkjaar
					// van lid bevat, maar voor de zekerheid filteren we ze eruit.

					var query = (from ld in f.Lid
								 where ld.GroepsWerkJaar.ID == lid.GroepsWerkJaar.ID
								&& ld.ID != lid.ID
								 select ld);
					if (query.Count() >= f.MaxAantal)
					{
						// TODO: Een exception is hier eigenlijk niet op zijn plaats;
						// de bedoeling is dat je ergens een warning krijgt, die je
						// aanzet om de situatie recht te zetten.
						// Zie verslag maart 2010.

						// Andere optie:
						//  Toch een exception
						//  UI toont lijst van huidige personen met deze functie,
						//  en laat gebruiker kiezen wiens functie moet afgenomen worden.
						//  dan functionaliteit 'FunctieOverNemen' of zoiets implementeren.
						// (maar dat werkt alleen als functies 1 voor 1 toegekend worden)

						throw new InvalidOperationException(
							String.Format(
								Properties.Resources.FunctieVol,
								f.MaxAantal,
								f.Naam));
					}
				}

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
		/// Controleert of de functie <paramref name="f"/> nationaal bepaald is.
		/// </summary>
		/// <param name="f">Te controleren functie</param>
		/// <returns><c>true</c> enkel als de functie nationaal bepaald is</returns>
		private bool IsNationaalBepaald(Functie f)
		{
			// FIXME: Dit is nogal gevaarlijk, want wie weet is de groep gewoon niet gefetcht.

			return f.Groep == null;
		}
	}
}
