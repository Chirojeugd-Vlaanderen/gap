// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over leden/leiding
	/// </summary>
	[DataContract]
	public class LidInfo
	{
		/// <summary>
		/// De ID van het lid
		/// </summary>
		[DataMember]
		public int LidID { get; set; }

		/// <summary>
		/// Kind of leiding
		/// </summary>
		[DataMember]
		public LidType Type { get; set; }

		/// <summary>
		/// Geeft aan of het lidgeld voor dat lid al betaald is of niet
		/// </summary>
		[DataMember]
		[DisplayName(@"Lidgeld betaald?")]
		public bool LidgeldBetaald { get; set; }

		/// <summary>
		/// De datum van het einde van de instapperiode
		/// enkel voor kinderen en niet aanpasbaar
		/// </summary>
		[DataMember]
		[DisplayName(@"Probeerperiode")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ConvertEmptyStringToNull = true)]
		public DateTime EindeInstapperiode { get; set; }

		/// <summary>
		/// Geeft aan of het lid inactief is of niet
		/// </summary>
		[DataMember]
		[DisplayName(@"Non-actief")]
		public bool NonActief { get; set; }

		/// <summary>
		/// De lijst van afdelingIDs waarin het lid zit (1 voor een kind)
		/// </summary>
		[DataMember]
		public IList<int> AfdelingIdLijst { get; set; }

		/// <summary>
		/// Functies van het lid
		/// </summary>
		[DataMember]
		public IList<FunctieDetail> Functies { get; set; }

		/// <summary>
		/// Groepswerkjaar waarvoor het lid ingeschreven is
		/// </summary>
		[DataMember]
		public int GroepsWerkJaarID { get; set; }

		/// <summary>
		/// Geeft aan of het lid verzekerd is tegen loonverlies
		/// </summary>
		[DataMember]
		public bool VerzekeringLoonVerlies { get; set; }
	}
}
