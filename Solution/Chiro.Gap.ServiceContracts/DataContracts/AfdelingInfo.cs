using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Informatie over een afdeling waarvoor er in het huidige werkjaar een groepswerkjaar bestaat.
	/// </summary>
	[DataContract]
	public class AfdelingInfo
	{
		/// <summary>
		/// ID van de afdeling
		/// </summary>
		[DataMember]
		public int AfdelingID;

		/// <summary>
		/// ID van het afdelingsjaar
		/// </summary>
		[DataMember]
		public int AfdelingsJaarID;

		/// <summary>
		/// Naam van de afdeling
		/// </summary>
		[DataMember]
		public string Naam;

		/// <summary>
		/// Afkorting van de afdeling
		/// </summary>
		[DataMember]
		public string Afkorting;

		/// <summary>
		/// Naam van de corresponderende officiele afdeling
		/// </summary>
		[DataMember]
		public string OfficieleAfdelingNaam;

		/// <summary>
		/// Geboortejaar oudste leden van de afdeling 
		/// </summary>
		[DataMember]
		public int GeboorteJaarVan;

		/// <summary>
		/// Geboortejaar jongste leden van de afdeling
		/// </summary>
		[DataMember]
		public int GeboorteJaarTot;

		/// <summary>
		/// <c>true</c> indien geverifieerd werd dat er geen leden zijn in het afdelingsjaar, anders <c>false</c>
		/// </summary>
		[DataMember]
		public bool AfdelingsJaarMagVerwijderdWorden;

	}
}