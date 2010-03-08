using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Cdf.Data.Entity;
using Chiro.Cdf.Data;
namespace Chiro.Gap.Orm
{
	[DataContract]
	public enum AdresTypeEnum
	{
		[EnumMember]
		Thuis = 1,
		[EnumMember]
		Kot = 2,
		[EnumMember]
		Werk = 3,
		[EnumMember]
		Overig = 4
	}

	public partial class PersoonsAdres : IEfBasisEntiteit
	{
		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get
			{
				return _teVerwijderen;
			}
			set
			{
				_teVerwijderen = value;
			}
		}

		public string VersieString
		{
			get
			{
				return this.VersieStringGet();
			}
			set
			{
				this.VersieStringSet(value);
			}
		}

		public AdresTypeEnum AdresType
		{
			get
			{
				return (AdresTypeEnum)this.AdresTypeInt;
			}
			set
			{
				this.AdresTypeInt = (int)value;
			}
		}

		public override int GetHashCode()
		{
			return 2;
		}
	}
}
