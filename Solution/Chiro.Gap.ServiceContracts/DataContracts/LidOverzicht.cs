﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract met informatie over een lid (kind of leiding).  In eerste instantie enkel gemaakt voor
	/// Excelexport, maar later hopelijk voor meer bruikbaar.
	/// </summary>
	[DataContract]
	public class LidOverzicht : PersoonOverzicht
	{
		/// <summary>
		/// Type lid (kind, leiding)
		/// </summary>
		[DataMember]
		public LidType Type { get; set; }

		/// <summary>
		/// Afdelingen van lid
		/// </summary>
		[DataMember]
		public List<AfdelingInfo> Afdelingen { get; set; }
		
		/// <summary>
		/// Functies van lid
		/// </summary>
		[DataMember]
		public List<FunctieInfo> Functies { get; set; }
	}
}