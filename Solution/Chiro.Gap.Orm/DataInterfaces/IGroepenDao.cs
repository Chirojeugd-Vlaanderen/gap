// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Groepen
	/// </summary>
	public interface IGroepenDao : IDao<Groep>
	{
		/// <summary>
		/// Ophalen van groep, groepswerkjaar, afdeling, afdelingsjaar en officiële afdelingen 
		/// voor gegeven groepswerkjaar.
		/// </summary>
		/// <remarks>Deze functie haalde origineel de afdelingen op voor een groep in het
		/// huidige werkjaar, maar 'huidige werkjaar' vind ik precies wat veel business
		/// voor in de DAL.</remarks>
		/// <param name="groepsWerkJaarID">ID van gevraagde groepswerkjaar</param>
		/// <returns>Groep, afdelingsjaar, afdelingen en officiële afdelingen</returns>
		Groep OphalenMetAfdelingen(int groepsWerkJaarID);

		/// <summary>
		/// Haalt een groep op met de werkjaren waarin ze aangesloten leden had
		/// </summary>
		/// <param name="groepID">ID van de groep in kwestie</param>
		/// <returns>De groep met haar groepswerkjaren</returns>
		Groep OphalenMetGroepsWerkJaren(int groepID);

		/// <summary>
		/// Haalt groep op met gegeven stamnummer
		/// </summary>
		/// <param name="code">Stamnummer op te halen groep</param>
		/// <returns>Groep met <paramref name="code"/> als stamnummer</returns>
		Groep Ophalen(string code);
	}
}
