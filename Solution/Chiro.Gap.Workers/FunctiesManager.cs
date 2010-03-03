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
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Instantieert een FunctiesManager
		/// </summary>
		/// <param name="funDao">Een IFunctiesDao voor data access mbt functies</param>
		/// <param name="auMgr">Een IAutorisatieManager voor de autorisatie</param>
		public FunctiesManager(IFunctiesDao funDao, IAutorisatieManager auMgr)
		{
			_funDao = funDao;
			_autorisatieMgr = auMgr;
		}

		/// <summary>
		/// Kent de meegegeven <paramref name="functies"/> toe aan het gegeven <paramref name="lid"/>.
		/// Als het lid al andere functies had, blijven die behouden.
		/// </summary>
		/// <param name="lid">Lid dat de functies moet krijgen</param>
		/// <param name="functies">Rij toe te kennen functies</param>
		/// <remarks>De functie moet gekoppeld zijn aan zijn groep (als die bestaat).  Het lid moet
		/// gekoppeld zijn aan zijn GelieerdePersoon, die op zijn beurt gekoppeld moet zijn aan
		/// Groep en Persoon.</remarks>
		public void Toekennen(Lid lid, IEnumerable<Functie> functies)
		{
			Debug.Assert(lid.GelieerdePersoon != null);
			Debug.Assert(lid.GelieerdePersoon.Groep != null);

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
				if (!IsNationaalBepaald(f) && f.Groep.ID != lid.GelieerdePersoon.Groep.ID)
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
					// Aantalconstraints op functie.  Tel huidig aantal

					if (_funDao.AantalLeden(lid.GelieerdePersoon.Groep.ID, f.ID) >= f.MaxAantal)
					{
						// TODO: Custom exception?
						throw new InvalidOperationException(
							String.Format(
								Properties.Resources.FunctieVol,
								f.MaxAantal,
								f.Naam));
					}
				}

				// Alle checks goed overleefd; functie koppelen

				throw new NotImplementedException(); // dat heb ik nog niet geimpl.
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
