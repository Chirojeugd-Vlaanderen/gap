// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor problemen met aantallen en functies
	/// </summary>
	[DataContract]
	public class FunctieProbleemInfo
	{
		/// <summary>
		/// Uniek identificatienummer
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De afkorting van de functie
		/// </summary>
		[DataMember]
		public string Code { get; set; }

		/// <summary>
		/// De naam van de functie
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// Het maximum aantal leden dat deze functie mag hebben per groep
		/// </summary>
		[DataMember]
		public int? MaxAantal { get; set; }

		/// <summary>
		/// Het minimum aantal leden dat deze functie moet hebben per groep
		/// </summary>
		[DataMember]
		public int MinAantal { get; set; }
		
		/// <summary>
		/// Het aantal leden van de groep dat de functie heeft
		/// </summary>
		[DataMember]
		public int EffectiefAantal { get; set; }
	}
}
