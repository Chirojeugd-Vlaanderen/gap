// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over aangesloten leden/leiding en hun afdeling(en)
	/// </summary>
	[DataContract]
	public class InTeSchrijvenLid
	{
		/// <summary>
		/// De volledige naam van de gelieerde persoon
		/// </summary>
		[DataMember]
		public string VolledigeNaam{ get; set; }

		/// <summary>
		/// De id van de gelieerde persoon die we leiding willen maken
		/// </summary>
		[DataMember]
		public int GelieerdePersoonID{ get; set; }

		/// <summary>
		/// Geeft aan of het een leiding moet worden ipv een kind
		/// </summary>
		[DataMember]
		public bool LeidingMaken { get; set; }

		/// <summary>
		/// De ID van het eventuele gekozen afdelingsjaar, anders null. (zou nooit null mogen zijn als leidingmaken true is
		/// </summary>
		[DataMember]
		public int? AfdelingsJaarID { get; set; }
	}
}
