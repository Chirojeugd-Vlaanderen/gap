using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte informatie over een functie
	/// </summary>
	[DataContract]
	[KnownType(typeof(FunctieDetail))]
	public class FunctieInfo
	{
		[DataMember]
		public int ID { get; set; }
		[Verplicht, StringLengte(10), StringMinimumLengte(2)]
		[DataMember]
		public string Code { get; set; }
		[Verplicht, StringLengte(80), StringMinimumLengte(2)]
		[DataMember]
		public string Naam { get; set; }
	}
}
