using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
	[DataContract]
	public class Bivak
	{
		/// <summary>
		/// Stamnummer voor groep die op bivak gaat
		/// </summary>
		[DataMember]
		public string StamNummer { get; set; }
		/// <summary>
		/// Naam van de bivakplaats
		/// </summary>
		[DataMember]
		public string BivakPlaatsNaam { get; set; }
		/// <summary>
		/// Adres van de bivakplaats
		/// </summary>
		[DataMember]
		public Adres Adres { get; set; }
	}
}
