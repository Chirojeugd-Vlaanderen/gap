// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class AdresInfo
	{
		private string _bus;

		/// <summary>
		/// Het AdresID
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Bus
		{
			// Vervangt eventuele null door String.Empty
			// Zie ticket #202: https://develop.chiro.be/trac/cg2/ticket/202
			get { return _bus; }
			set { _bus = value ?? String.Empty; }
		}

		[DataMember]
		public int PostNr { get; set; }

		[DataMember]
		public int HuisNr { get; set; }

		[DataMember]
		[DisplayName(@"Straat")]
		public String StraatNaamNaam { get; set; }

		[DataMember]
		[DisplayName(@"Woonplaats")]
		public String WoonPlaatsNaam { get; set; }

		[DataMember]
		public int WoonPlaatsID { get; set; }
	}
}
