// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Categorieën
	/// </summary>
	public interface ICategorieenDao : IDao<Categorie>
	{
		/*/// <summary>
		/// Haalt de gekoppelde groep van een categorie op uit de database
		/// (als dat nog niet gebeurd zou zijn)
		/// </summary>
		/// <param name="categorie">Categorie met op te halen groep</param>
		/// <returns>Dezelfde categorie</returns>
		Categorie GroepLaden(Categorie categorie);*/

		IEnumerable<Categorie> OphalenVanGroep(int groepID);

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
