using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.FaultContracts
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
		/// <param name="foutCode">foutcode</param>
		/// <param name="component">omschrijving van de component waar het fout 
		/// liep.</param>
		/// <param name="bericht">foutboodschap.  Mag ook leeg zijn.</param>
		public void BerichtToevoegen(T foutCode, string component, string bericht)
		{
			Berichten.Add(component, new FoutBericht<T> { FoutCode = foutCode, Bericht = bericht });
		}
	}
}
