using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

		[Verplicht]
		[DataMember]
		public string Nummer
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName(@"Gebruiken om persoon te contacteren?")]
		public bool Voorkeur
		{
			get;
			set;
		}

		[DataMember]
		[StringLengte(320)]
		[DataType(DataType.MultilineText)]
		public string Nota
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName(@"Voor heel het gezin?")]
		public bool IsGezinsGebonden { get; set; }

		[DataMember]
		public string VersieString { get; set; }

		[DataMember]
		public int CommunicatieTypeID { get; set; }
	}
}
