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
		Groep OphalenMetAfdelingen(int groepID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepID">ID van de groep waar het over gaat</param>
		/// <returns></returns>
		Groep OphalenMetGroepsWerkJaren(int groepID);

		/// <summary>
		/// Haalt groep op met gegeven stamnummer
		/// </summary>
		/// <param name="code">Stamnummer op te halen groep</param>
		/// <returns>Groep met <paramref name="code"/> als stamnummer</returns>
		Groep Ophalen(string code);
	}
}
