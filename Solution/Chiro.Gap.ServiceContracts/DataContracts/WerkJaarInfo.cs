// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over een werkJaar
	/// </summary>
	[DataContract]
	public class WerkJaarInfo
	{
		/// <summary>
		/// ID van het *groepswerkjaar*.
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Kalenderjaar waarin het werkJaar *begint*.
		/// </summary>
		[DataMember]
		public int WerkJaar { get; set; }
	}
}