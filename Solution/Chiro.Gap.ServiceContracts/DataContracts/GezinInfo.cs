using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
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
