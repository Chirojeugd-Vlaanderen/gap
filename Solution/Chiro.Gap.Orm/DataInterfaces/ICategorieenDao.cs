// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Categorieën
	/// </summary>
	public interface ICategorieenDao : IDao<Categorie>
	{
		/// <summary>
		/// Haalt een lijst op met alle categorieën van de opgegeven groep
		/// </summary>
		/// <param name="groepID">ID van de groep waar we de categorieën voor ophalen</param>
		/// <returns>Een lijst van categorieën</returns>
		IList<Categorie> AllesOphalen(int groepID);

		/// <summary>
		/// Zoekt een categorie op op basis van <paramref name="groepID"/> en
		/// <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de te zoeken categorie gekoppeld moet zijn</param>
		/// <param name="code">Code van de te zoeken categorie</param>
		/// <returns>De gevonden categorie; <c>null</c> indien niet gevonden</returns>
		Categorie Ophalen(int groepID, string code);
	}
}
