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
		/// Haalt alle personen op die op een zelfde
		/// adres wonen als de gelieerde persoon met het gegeven ID.
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
		/// Ophalen van een lijst personen met gekoppelde entiteiten
		/// </summary>
		/// <param name="ids">PersoonID's van op te halen personen</param>
		/// <param name="metGelieerdePersonen">Indien true, worden aan elke persoon de gelieerde personen gekoppeld waar
		/// de gebruiker met login <paramref name="login"/> GAV voor is.</param>
		/// <param name="login">Enkel relevant indien <paramref name="metGelieerdePersonen"/> <c>true</c> is: de username
		/// van de gebruiker.</param>
		/// <param name="paths">Omschrijft mee op te halen gekoppelde entiteiten</param>
		/// <returns>Een lijst opgehaalde entiteiten</returns>
		IList<Persoon> Ophalen(
			IEnumerable<int> ids,
			bool metGelieerdePersonen,
			string login,
			params Expression<Func<Persoon, object>>[] paths);
	}
}
