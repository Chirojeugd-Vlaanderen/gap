// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Gedetailleerde informatie over een groepswerkjaar
	/// </summary>
	[DataContract]
	public class GroepsWerkJaarDetail
	{
	    /// <summary>
	    /// Het beginjaartal, bv. 2011 voor 2011-2012
	    /// </summary>
	    [DataMember]
		public int WerkJaar { get; set; }

	    /// <summary>
	    /// De ID van het werkjaar
	    /// </summary>
	    [DataMember]
		public int WerkJaarID { get; set; }

	    /// <summary>
	    /// TODO (#190): documenteren
	    /// </summary>
	    [DataMember]
		public WerkJaarStatus Status { get; set; }

	    /// <summary>
	    /// De ID van de groep die in het opgegeven werkjaar actief is
	    /// </summary>
	    [DataMember]
		public int GroepID { get; set; }

	    /// <summary>
	    /// De naam van de groep
	    /// </summary>
	    [DataMember]
		public string GroepNaam { get; set; }

	    /// <summary>
	    /// De gemeente waar de groep zich bevindt
	    /// </summary>
	    [DataMember]
		public string GroepPlaats { get; set; }

	    /// <summary>
	    /// Het stamnummer van de groep
	    /// </summary>
	    [DataMember]
		public string GroepCode { get; set; }

	    /// <summary>
	    /// Geeft aan of het om een lokale groep, een gewest of een verbond gaat
	    /// </summary>
	    [DataMember]
		public Niveau GroepNiveau { get; set; }
	}
}
