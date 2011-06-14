// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
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
		/// <summary>
		/// AD-nummer van de bewoner
		/// </summary>
		[DataMember]
		public int? PersoonAdNummer { get; set; }

		/// <summary>
		/// De ID van de bewoner als gelieerde persoon
		/// </summary>
		[DataMember]
		public int GelieerdePersoonID { get; set; }

		/// <summary>
		/// De ID van de bewoner als persoon
		/// </summary>
		[DataMember]
		public int PersoonID { get; set; }

		/// <summary>
		/// Voornaam en naam van de bewoner
		/// </summary>
		[DataMember]
		public string PersoonVolledigeNaam { get; set; }

		/// <summary>
		/// Geboortedatum van de bewoner
		/// </summary>
		[DataMember]
		public DateTime? PersoonGeboorteDatum { get; set; }

		/// <summary>
		/// Geslacht van de bewoner
		/// </summary>
		[DataMember]
		public GeslachtsType PersoonGeslacht { get; set; }

		/// <summary>
		/// Het type dat de relatie van de bewoner met het adres beschrijft
		/// (bv. 'kotadres')
		/// </summary>
		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
	}
}
