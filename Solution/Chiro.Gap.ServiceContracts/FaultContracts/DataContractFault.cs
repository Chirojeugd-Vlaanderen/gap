// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Een <c>DataContractFault</c> is een probleem met de gegevens in het datacontract, waarbij er
	/// specifiek gezegd kan worden in welk 'onderdeel' van het datacontract de problemen zitten
	/// (vandaar de dictionary <c>Berichten</c>, die een 'onderdeel' koppelt met een foutbericht)
	/// </summary>
	/// <typeparam name="T">Type van de foutcode (meestal een enum)</typeparam>
	[DataContract]
	public class DataContractFault<T>
	{
		[DataMember]
		public Dictionary<string, FoutBericht<T>> Berichten { get; set; }

		public DataContractFault()
		{
			Berichten = new Dictionary<string, FoutBericht<T>>();
		}

		/// <summary>
		/// Voegt een foutbericht toe
		/// </summary>
		/// <param name="foutCode">Code van de fout</param>
		/// <param name="component">Omschrijving van de component waar het fout 
		/// liep</param>
		/// <param name="bericht">Foutboodschap.  Mag ook leeg zijn.</param>
		public void BerichtToevoegen(T foutCode, string component, string bericht)
		{
			Berichten.Add(component, new FoutBericht<T> { FoutCode = foutCode, Bericht = bericht });
		}
	}
}
