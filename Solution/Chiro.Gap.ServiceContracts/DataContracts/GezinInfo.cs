using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Informatie over mensen die eenzelfde adres delen
	/// </summary>
	[DataContract]
	public class GezinInfo: AdresInfo
	{
		/// <summary>
		/// Personen die wonen op het gegeven adres
		/// </summary>
		[DataMember] public IList<BewonersInfo> Bewoners;
	}
}
