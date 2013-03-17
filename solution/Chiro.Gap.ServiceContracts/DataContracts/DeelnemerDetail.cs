// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte info ivm een deelnemer van een uitstap.
	/// </summary>
	[DataContract]
	public class DeelnemerDetail : DeelnemerInfo
	{
	    /// <summary>
	    /// De ID van de gelieerde persoon die deelneemt
	    /// </summary>
	    [DataMember]
		public int GelieerdePersoonID { get; set; }

	    /// <summary>
	    /// Gedetailleerde persoonsinfo over de deelnemer
	    /// </summary>
	    [DataMember]
		public PersoonOverzicht PersoonOverzicht { get; set; }

	    /// <summary>
	    /// De ID van de uitstap waar die persoon aan deelneemt
	    /// </summary>
	    [DataMember]
        public int UitstapID { get; set; }

	    /// <summary>
	    /// De voornaam van de deelnemer
	    /// </summary>
	    [DataMember]
		public string VoorNaam { get; set; }

	    /// <summary>
	    /// De familienaam van de deelnemer
	    /// </summary>
	    [DataMember]
		public string FamilieNaam { get; set; }

	    /// <summary>
	    /// Geeft aan of de deelnemer deelneemt, begeleidt of meegaat als logistiek medewerker
	    /// </summary>
	    [DataMember]
		public DeelnemerType Type { get; set; }

	    /// <summary>
	    /// TODO (#190): documenteren
	    /// </summary>
	    [DataMember]
		public IList<AfdelingInfo> Afdelingen { get; set; }

	    /// <summary>
	    /// Is de deelnemer contactpersoon voor de uitstap in kwestie?
	    /// </summary>
	    [DataMember]
		public bool IsContact { get; set; }
	}
}