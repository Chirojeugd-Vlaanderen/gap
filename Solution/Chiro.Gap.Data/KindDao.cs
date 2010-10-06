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

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor kinderen
	/// </summary>
	public class KindDao : Dao<Kind, ChiroGroepEntities>, IKindDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor kinderen
		/// </summary>
		public KindDao()
		{
			ConnectedEntities = new Expression<Func<Kind, object>>[] 
			{ 
				e => e.GroepsWerkJaar.WithoutUpdate(), 
				e => e.GelieerdePersoon.WithoutUpdate(), 
				e => e.GelieerdePersoon.Persoon.WithoutUpdate(), 
				e => e.AfdelingsJaar.WithoutUpdate(),
				e => e.AfdelingsJaar.Afdeling.WithoutUpdate() 
			};
		}

		/// <summary>
		/// Haalt alle kinderen op uit een gegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="paths">Geeft aan welke entiteiten mee opgehaald moeten worden</param>
		/// <returns>Rij opgehaalde kinderen</returns>
		public IEnumerable<Kind> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<Func<Kind, object>>[] paths)
		{
			Kind[] lijst;

			using (var db = new ChiroGroepEntities())
			{
				var kinderen = (
					from l in db.Lid.OfType<Kind>()
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					select l) as ObjectQuery<Kind>;

				lijst = IncludesToepassen(kinderen, paths).ToArray();
			}

			return Utility.DetachObjectGraph<Kind>(lijst);			
		}

		/// <summary>
		/// Haalt alle kinderen op uit afdelingsjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// en <paramref name="afdelingID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar van afdelingsjaar</param>
		/// <param name="afdelingID">ID van afdeling van afdelingsjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen entiteiten</param>
		/// <returns>Alle kinderen van het gevraagde afdelngsjaar</returns>
		public IEnumerable<Kind> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdelingID, Expression<Func<Kind, object>>[] paths)
		{
			Kind[] lijst;

			using (var db = new ChiroGroepEntities())
			{
				var kinderen = (
					from l in db.Lid.OfType<Kind>()
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					&& l.AfdelingsJaar.Afdeling.ID == afdelingID
					select l) as ObjectQuery<Kind>;

				lijst = IncludesToepassen(kinderen, paths).ToArray();
			}

			return lijst;
		}

		/// <summary>
		/// Haalt alle kinderen op uit groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// met functie bepaald door <paramref name="functieID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <param name="functieID">ID van functie</param>
		/// <param name="paths">Bepaalt de mee op te halen entiteiten</param>
		/// <returns>Alle kinderen van het gevraagde afdelngsjaar</returns>
		public IEnumerable<Kind> OphalenUitFunctie(int groepsWerkJaarID, int functieID, Expression<Func<Kind, object>>[] paths)
		{
			Kind[] lijst;

			using (var db = new ChiroGroepEntities())
			{
				var kinderen = (
					from l in db.Lid.OfType<Kind>()
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					&& l.Functie.Any(fn=>fn.ID == functieID)
					select l) as ObjectQuery<Kind>;

				lijst = IncludesToepassen(kinderen, paths).ToArray();
			}

			return lijst;
		}
	}
}
