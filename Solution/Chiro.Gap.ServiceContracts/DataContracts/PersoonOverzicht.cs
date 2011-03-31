// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract met informatie over (gelieerde) personen dat in eerste instantie enkel gebruikt
	/// zal worden door de Excelexport.  Gegevens van voorkeursadres, voorkeurstelefoonnummer en
	/// voorkeursmailadres worden mee opgenomen
	/// </summary>
	[DataContract]
	[KnownType(typeof(LidOverzicht))]
	public class PersoonOverzicht : PersoonInfo
	{
		[DataMember]
		public string StraatNaam;

		[DataMember]
		public int? HuisNummer;
		
		[DataMember]
		public string Bus;
		
		[DataMember]
		public int? PostNummer;

		[DataMember] public string PostCode;
		
		[DataMember]
		public string WoonPlaats;

		[DataMember] public string Land;
		
		[DataMember]
		public string TelefoonNummer;
		
		[DataMember]
		public string Email;
	}
}
