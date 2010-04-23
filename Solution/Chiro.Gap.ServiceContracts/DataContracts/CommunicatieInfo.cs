// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class CommunicatieInfo
	{
		[DataMember]
		public int ID
		{
			get;
			set;
		}

		[DataMember]
		public string Nummer
		{
			get;
			set;
		}

		[DataMember]
		public bool Voorkeur
		{
			get;
			set;
		}

		[DataMember]
		public string Nota
		{
			get;
			set;
		}

		[DataMember]
		public CommunicatieType CommunicatieType
		{
			get;
			set;
		}

	}
}
