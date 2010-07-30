// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over een werkjaar
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
		/// Kalenderjaar waarin het werkjaar *begint*.
		/// </summary>
		[DataMember]
		public int WerkJaar { get; set; }
	}
}