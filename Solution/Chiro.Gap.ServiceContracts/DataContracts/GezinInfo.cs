// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Informatie over mensen die eenzelfde adres delen
	/// </summary>
	[DataContract]
	public class GezinInfo : AdresInfo
	{
		/// <summary>
		/// Personen die wonen op het gegeven adres
		/// </summary>
		[DataMember]
		public IList<BewonersInfo> Bewoners;
	}
}
