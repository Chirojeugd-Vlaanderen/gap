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
	[DataContract]
	public class LidInfo
	{
		[DataMember]
		public int LidID { get; set; }

		//[DataMember]
		//public PersoonDetail PersoonDetail { get; set; }

		/// <summary>
		/// Kind of leiding
		/// </summary>
		[DataMember]
		public LidType Type { get; set; }

		[DataMember]
		public bool LidgeldBetaald { get; set; }

		/// <summary>
		/// De datum van het einde van de instapperiode
		/// enkel voor kinderen en niet aanpasbaar
		/// </summary>
		[DataMember]
		[DisplayName(@"Probeerperiode")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ConvertEmptyStringToNull = true)]
		public DateTime? EindeInstapperiode { get; set; }

		/// <summary>
		/// Geeft aan of het lid inactief is of niet
		/// </summary>
		[DataMember]
		public bool NonActief { get; set; }

		/// <summary>
		/// Geeft aan of de leid(st)er een abonnement heeft op dubbelpunt
		/// </summary>
		[DataMember]
		public bool DubbelPunt { get; set; }

		/// <summary>
		/// De lijst van afdelingIDs waarin het lid zit (1 voor een kind)
		/// </summary>
		[DataMember]
		public IList<int> AfdelingIdLijst { get; set; }

		/// <summary>
		/// Functies van het lid
		/// </summary>
		[DataMember]
		public IList<FunctieInfo> Functies { get; set; }

		[DataMember]
		public int GroepsWerkJaarID { get; set; }
	}
}
