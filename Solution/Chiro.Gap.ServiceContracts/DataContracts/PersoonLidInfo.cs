// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract waarin een gelieerde persoon, zijn/haar LidInfo, zijn/haar adressen 
	/// en zijn/haar communicatievormen aan elkaar koppeld zijn
	/// </summary>
	[DataContract]
	public class PersoonLidInfo
	{
		/// <summary>
		/// Uitgebreid info-object over de persoon
		/// </summary>
		[DataMember]
		public PersoonDetail PersoonDetail { get; set; }

		// TODO booleans voor wat er geladen is

		/// <summary>
		/// Info-object van het lidmaatschap van de persoon
		/// </summary>
		[DataMember]
		public LidInfo LidInfo { get; set; }

		/// <summary>
		/// De lijst van adressen waar de persoon zoal verblijft
		/// </summary>
		[DataMember]
		public IEnumerable<PersoonsAdresInfo> PersoonsAdresInfo { get; set; }

		/// <summary>
		/// De lijst van communicatievormen die de persoon gebruikt
		/// </summary>
		[DataMember]
		public IEnumerable<CommunicatieDetail> CommunicatieInfo { get; set; }

	    /// <summary>
	    /// Einddatum van GAV-schap van deze gelieerde persoon, of null als hij geen GAV is voor zijn groep.
	    /// </summary>
        [DataMember]
        [DataType(DataType.Date)]
        public DateTime? GavTot { get; set; }
	}
}
