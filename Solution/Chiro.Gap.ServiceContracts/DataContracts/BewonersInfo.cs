// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
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
