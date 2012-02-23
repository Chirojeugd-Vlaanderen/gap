// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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
	/// Gegevenstoegangsobject voor categorieën
	/// </summary>
	public class CategorieenDao : Dao<Categorie, ChiroGroepEntities>, ICategorieenDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor categorieën
		/// </summary>
		public CategorieenDao()
		{
			ConnectedEntities = new Expression<Func<Categorie, object>>[] 
			{ 
										e => e.Groep.WithoutUpdate(), 
										e => e.GelieerdePersoon.First().WithoutUpdate() 
			};
		}

		/// <summary>
		/// Alle categorieën ophalen die in de gegeven groep gebruikt worden (of werden)
		/// </summary>
		/// <param name="groepID">ID van de groep in kwestie</param>
		/// <returns>Een lijst van categorieën</returns>
		public IList<Categorie> AllesOphalen(int groepID)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Categorie.MergeOption = MergeOption.NoTracking;

				return (
					from cv in db.Categorie
					where cv.Groep.ID == groepID
					select cv).ToList();
			}
		}

		/// <summary>
		/// Zoekt een categorie op op basis van <paramref name="groepID"/> en
		/// <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de te zoeken categorie gekoppeld moet zijn</param>
		/// <param name="code">Code van de te zoeken categorie</param>
		/// <returns>De gevonden categorie; <c>null</c> indien niet gevonden</returns>
		public Categorie Ophalen(int groepID, string code)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Categorie.MergeOption = MergeOption.NoTracking;

				return (from cat in db.Categorie
						where cat.Groep.ID == groepID && string.Compare(cat.Code, code) == 0
						select cat).FirstOrDefault();
			}
		}
	}
}
