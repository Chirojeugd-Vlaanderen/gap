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
	/// Gegevenstoegangsobject voor leiding
	/// </summary>
	public class LeidingDao : Dao<Leiding, ChiroGroepEntities>, ILeidingDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor leiding
		/// </summary>
		public LeidingDao()
		{
			ConnectedEntities = new Expression<Func<Leiding, object>>[] 
			{
				e => e.GroepsWerkJaar.WithoutUpdate(),
				e => e.GelieerdePersoon, 
				e => e.GelieerdePersoon.Persoon, 
				e => e.AfdelingsJaar.First(),
				e => e.AfdelingsJaar.First().Afdeling.WithoutUpdate()
			};
		}

		/// <summary>
		/// Haalt alle leiding op uit een gegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="paths">Geeft aan welke entiteiten mee opgehaald moeten worden</param>
		/// <returns>Rij opgehaalde leiding</returns>
		public IEnumerable<Leiding> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<Func<Leiding, object>>[] paths)
		{
			Leiding[] lijst;

			using (var db = new ChiroGroepEntities())
			{
				var leiding = (
					from l in db.Lid.OfType<Leiding>()
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l) as ObjectQuery<Leiding>;

				lijst = IncludesToepassen(leiding, paths).ToArray();
			}

			return Utility.DetachObjectGraph<Leiding>(lijst);
		}

		/// <summary>
		/// Haalt alle leiding op uit afdelingsjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// en <paramref name="afdelingID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar van afdelingsjaar</param>
		/// <param name="afdelingID">ID van afdeling van afdelingsjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen entiteiten</param>
		/// <returns>Alle kinderen van het gevraagde afdelngsjaar</returns>
		public IEnumerable<Leiding> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdelingID, Expression<Func<Leiding, object>>[] paths)
		{
			Leiding[] lijst;

			using (var db = new ChiroGroepEntities())
			{
				var leiding = (
					from l in db.Lid.OfType<Leiding>()
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					&& l.AfdelingsJaar.Any(aj => aj.Afdeling.ID == afdelingID)
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l) as ObjectQuery<Leiding>;

				lijst = IncludesToepassen(leiding, paths).ToArray();
			}

			return lijst;
		}

		/// <summary>
		/// Haalt alle leiding op uit groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// met functie bepaald door <paramref name="functieID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <param name="functieID">ID van functie</param>
		/// <param name="paths">Bepaalt de mee op te halen entiteiten</param>
		/// <returns>Alle leiding met gevraagde functie uit gevraagde groepswerkjaar</returns>
		public IEnumerable<Leiding> OphalenUitFunctie(int groepsWerkJaarID, int functieID, Expression<Func<Leiding, object>>[] paths)
		{
			Leiding[] lijst;

			using (var db = new ChiroGroepEntities())
			{
				var leiding = (
					from l in db.Lid.OfType<Leiding>()
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					&& l.Functie.Any(fn => fn.ID == functieID)
					select l) as ObjectQuery<Leiding>;

				lijst = IncludesToepassen(leiding, paths).ToArray();
			}

			return lijst;
		}
	}
}
