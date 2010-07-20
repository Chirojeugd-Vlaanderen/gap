using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract met informatie over (gelieerde) personen dat in eerste instantie enkel gebruikt
	/// zal worden door de Excelexport.  Gegevens van voorkeursadres, voorkeurstelefoonnummer en
	/// voorkeursmailadres worden mee opgenomen
	/// </summary>
	[DataContract]
	public class PersoonOverzicht: PersoonInfo
	{
		[DataMember] public string StraatNaam;
		[DataMember] public int? HuisNummer;
		[DataMember] public string Bus;
		[DataMember] public int? PostNummer;
		[DataMember] public string WoonPlaats;
		[DataMember] public string TelefoonNummer;
		[DataMember] public string Email;
	}
}
