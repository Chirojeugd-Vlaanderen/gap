// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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
		/// Naam van de corresponderende officiële afdeling
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
		/// <c>True</c> indien geverifieerd werd dat er geen leden zijn in het afdelingsjaar, anders <c>false</c>
		/// </summary>
		[DataMember]
		public bool AfdelingsJaarMagVerwijderdWorden;
	}
}