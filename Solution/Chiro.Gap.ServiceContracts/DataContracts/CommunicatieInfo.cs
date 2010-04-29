using System.Runtime.Serialization;

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
		public bool IsGezinsGebonden { get; set; }

		[DataMember]
		public string VersieString { get; set; }

		[DataMember]
		public int CommunicatieTypeID { get; set; }
	}
}
