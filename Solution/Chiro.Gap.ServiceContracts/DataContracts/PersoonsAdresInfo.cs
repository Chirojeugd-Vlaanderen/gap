// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class PersoonsAdresInfo
	{
		[DataMember]
		public AdresInfo AdresInfo { get; set; }

		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
	}

	/// <summary>
	/// Flauw datacontractje dat enkel een PersoonID en een AdresID bevat.  Wordt gebruikt om informatie
	/// mee te geven over al bestaande adressen
	/// (Helaas had ik een datacontract geschreven met dezelfde naam als een ander datacontract van Broes.)
	/// </summary>
	[DataContract]
	public class PersoonsAdresInfo2
	{
		[DataMember]
		public int PersoonID { get; set; }
		[DataMember]
		public int AdresID { get; set; }
	}
}
