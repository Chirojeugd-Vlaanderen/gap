// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Personen
	/// </summary>
	/// <remarks>
	/// Met een GelieerdePersoon moet altijd het geassocieerde
	/// persoonsobject meekomen, anders heeft het weinig zin.
	/// </remarks>
	public interface IPersonenDao : IDao<Persoon>
	{
		/// <summary>
		/// Haalt alle gelieerde personen op (incl persoonsinfo) die op een zelfde
		/// adres wonen en gelieerd zijn aan dezelfde groep als de gelieerde persoon met het gegeven ID.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gegeven gelieerde
		/// persoon.</param>
		/// <returns>Lijst met GelieerdePersonen (inc. persoonsinfo)</returns>
		/// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
		/// huisgenoot.</remarks>
		IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID);

		/// <summary>
		/// Haalt de persoon op die correspondeert met een gelieerde persoon.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde
		/// persoon.</param>
		/// <returns>Persoon inclusief adresinfo</returns>
		Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID);

		/// <summary>
		///  Haalt een lijst op van personen, op basis van een lijst <paramref name="gelieerdePersoonIDs"/>.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van *GELIEERDE* personen, waarvan de corresponderende persoonsobjecten
		/// opgehaald moeten worden.</param>
		/// <param name="paths">Bepaalt welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>De gevraagde personen></returns>
		IEnumerable<Persoon> OphalenViaGelieerdePersoon(IEnumerable<int> gelieerdePersoonIDs, params Expression<Func<Persoon, object>>[] paths);
	}
}
