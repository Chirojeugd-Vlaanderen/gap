// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Informatie over een afdeling waarvoor er in het huidige werkjaar een groepswerkjaar bestaat.
	/// </summary>
	[DataContract]
	public class AfdelingDetail
	{
		/// <summary>
		/// ID van de afdeling
		/// </summary>
		[DataMember]
		public int AfdelingID { get; set; }

		/// <summary>
		/// ID van het afdelingsjaar
		/// </summary>
		[DataMember]
		public int AfdelingsJaarID { get; set; }

		/// <summary>
		/// ID van de corresponderende officiele afdeling
		/// </summary>
		[DataMember]
		public int OfficieleAfdelingID { get; set; }

		/// <summary>
		/// Naam van de afdeling
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// Afkorting van de afdeling
		/// </summary>
		[DataMember]
		public string Afkorting { get; set; }

		/// <summary>
		/// Naam van de corresponderende officiële afdeling
		/// </summary>
		[DataMember]
		public string OfficieleAfdelingNaam { get; set; }

		/// <summary>
		/// Geboortejaar oudste leden van de afdeling 
		/// </summary>
		[DataMember]
		public int GeboorteJaarVan { get; set; }

		/// <summary>
		/// Geboortejaar jongste leden van de afdeling
		/// </summary>
		[DataMember]
		public int GeboorteJaarTot { get; set; }

		/// <summary>
		/// Geslacht van de afdeling
		/// </summary>
		[DataMember]
		public GeslachtsType Geslacht { get; set; }

		/// <summary>
		/// <c>True</c> indien geverifieerd werd dat er geen leden zijn in het afdelingsjaar, anders <c>false</c>
		/// </summary>
		[DataMember]
		public bool IsLeeg { get; set; }
	}
}