// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Fouten;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.Validatie
{
	/// <summary>
	/// Extension methods voor IValidatieDictionary
	/// </summary>
	public static class ValidatieDictionaryMethods
	{
		/// <summary>
		/// Voegt berichten uit een BusinessFault toe aan een
		/// 'IValidatieDictionary'
		/// </summary>
		/// <typeparam name="T">Type foutcode voor BusinessFault</typeparam>
		/// <param name="dst">IValidatieDictionary waaraan de berichten
		/// moeten worden toegevoegd.</param>
		/// <param name="src">BusinessFault met toe te voegen berichten</param>
		/// <param name="keyPrefix">Prefix toe te voegen aan de keys van src,
		/// alvorens ze toe te voegen aan dst</param>
		public static void BerichtenToevoegen<T>(this IValidatieDictionary dst, OngeldigObjectFault<T> src, string keyPrefix)
		{
			foreach (KeyValuePair<string, FoutBericht<T>> paar in src.Berichten)
			{
				dst.BerichtToevoegen(String.Format("{0}{1}", keyPrefix, paar.Key), paar.Value.Bericht);
			}
		}
	}
}
