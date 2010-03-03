// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Deze fault zal over de service gestuurd worden als de gebruiker probeert een nieuwe persoon aan
	/// te maken, terwijl er al een gelijkaardige gelieerde persoon bestaat.  (tenzij de aanmaak
	/// geforceerd wordt)
	/// </summary>
	[DataContract]
	public class GelijkaardigePersoonFault
	{
		[DataMember]
		public IList<PersoonInfo> GelijkaardigePersonen { get; set; }
	}
}
