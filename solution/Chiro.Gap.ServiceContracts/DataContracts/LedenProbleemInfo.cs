// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor problemen i.v.m. ontbrekende gegevens van leden.  Per type probleem kan
	/// meegegeven worden hoe vaak het zich voordoet.
	/// </summary>
	[DataContract]
	public class LedenProbleemInfo
	{
	    /// <summary>
	    /// Het probleem
	    /// </summary>
	    [DataMember]
		public LidProbleem Probleem { get; set; }

	    /// <summary>
	    /// Hoeveel keer komt dat probleem voor?
	    /// </summary>
	    [DataMember]
		public int Aantal { get; set; }
	}
}
