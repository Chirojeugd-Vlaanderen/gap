using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Details van een uitstap
	/// </summary>
	/// <remarks>Startdatum en einddatum zijn <c>DateTime?</c>, opdat we dit
	/// datacontract ook als model zouden kunnen gebruiken in de webappl.  Als
	/// Startdatum en einddatum nullable zijn, dan zijn ze bij het aanmaken
	/// van een nieuwe uitstap gewoon leeg, ipv een nietszeggende datum in het
	/// jaar 1 als ze niet nullable zijn.</remarks>
	[DataContract]
	public class UitstapDetail: UitstapInfo
	{
		// Momenteel nog geen extra's tov UitstapInfo, maar op termijn komt 
		// hier o.m. de plaats.
		[DataMember]
		public string Dummy { get; set; }
	}
}
