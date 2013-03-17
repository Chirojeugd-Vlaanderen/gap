// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Informatie over een afdeling waarvoor er in het huidige werkJaar een afdelingsjaar bestaat.
	/// <para />
	/// Dit is geen specialisatie van afdelingInfo omdat er dan minder code duplicatie is, maar bevat dus wel opnieuw naam en afkorting.
	/// </summary>
	[DataContract]
	public class AfdelingDetail : AfdelingsJaarDetail
	{
		/// <summary>
		/// Naam van de afdeling
		/// </summary>
		[DataMember]
		public string AfdelingNaam { get; set; }

		/// <summary>
		/// Afkorting van de afdeling
		/// </summary>
		[DataMember]
		public string AfdelingAfkorting { get; set; }

		/// <summary>
		/// Naam van de corresponderende officiële afdeling
		/// </summary>
		[DataMember]
		public string OfficieleAfdelingNaam { get; set; }
	}
}