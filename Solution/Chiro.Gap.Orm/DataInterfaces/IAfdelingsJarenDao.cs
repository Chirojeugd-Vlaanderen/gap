// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq.Expressions;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor AfdelingsJaren
	/// </summary>
	public interface IAfdelingsJarenDao : IDao<AfdelingsJaar>
	{
		/// <summary>
		/// Afdelingsjaar ophalen op basis van ID's van de
		/// afdeling en het groepswerkjaar.  Samen met afdelingsjaar
		/// wordt GroepsWerkJaar, OfficieleAfdeling en Afdeling teruggegeven.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="afdelingID">ID van de afdeling</param>
		/// <returns>Het gevraagde afdelingsjaar, of null indien niet
		/// gevonden.</returns>
		/// <remarks>Dit heeft enkel zin als de afdeling bepaald door
		/// AfdelingID een afdeling is van de groep bepaald door het
		/// gevraagde groepswerkjaar.</remarks>
		AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID);

		/// <summary>
		/// Afdelingsjaar ophalen op basis van ID's van de
		/// afdeling en het groepswerkjaar.  Samen met afdelingsjaar
		/// wordt GroepsWerkJaar, OfficieleAfdeling en Afdeling teruggegeven.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="afdelingID">ID van de afdeling</param>
		/// <param name="paths">Bepaalt welke gerelateerde entity's mee opgehaald moeten worden</param>
		/// <returns>Het gevraagde afdelingsjaar, of null indien niet
		/// gevonden.</returns>
		/// <remarks>Dit heeft enkel zin als de afdeling bepaald door
		/// AfdelingID een afdeling is van de groep bepaald door het
		/// gevraagde groepswerkjaar.</remarks>
		AfdelingsJaar Ophalen(
			int groepsWerkJaarID, 
			int afdelingID, 
			params Expression<Func<AfdelingsJaar, object>>[] paths);
	}
}
