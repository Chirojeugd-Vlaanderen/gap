using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Datacontract dat gebruikt wordt voor informatie over een bewoner.  Het kan ook dienen voor
	/// minimale persoonsinfo, als je <c>AdresType</c> negeert.
	/// </summary>
	/// <remarks>De namen van de property's zijn zodanig gekozen, dat AutoMapper niet
	/// speciaal geconfigureerd moet worden om te mappen van PersoonsAdres.</remarks>
	[DataContract]
	public class BewonersInfo
	{
		[DataMember]
		public int? PersoonAdNummer { get; set; }

		[DataMember]
		public int PersoonID { get; set; }

		[DataMember]
		public string PersoonVolledigeNaam { get; set; }

		[DataMember]
		public DateTime? PersoonGeboorteDatum { get; set; }

		[DataMember]
		public GeslachtsType PersoonGeslacht { get; set; }

		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
	}
}
