using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	[DataContract]
	public enum LidType
	{
		[EnumMember]
		Kind = 1,
		[EnumMember]
		Leiding = 2
	}

	public partial class Lid : IEfBasisEntiteit
	{
		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}

		public override int GetHashCode()
		{
			return 5;
		}

		/// <summary>
		/// Creeert een string met daarin een concatenatie van de namen van de
		/// afdelingen waaraan het lid gekoppeld is.
		/// </summary>
		/// <returns>comma separated afdelingsnamen</returns>
		/// <remarks>Een lid is hoogstens aan 1 afdeling gekoppeld</remarks>
		public string AfdelingsNamenGet()
		{
			if (this is Kind)
			{
				if ((this as Kind).AfdelingsJaar == null)
				{
					return Properties.Resources.Geen;
				}
				else
				{
					return (this as Kind).AfdelingsJaar.Afdeling.Naam;
				}
			}
			else if (this is Kind)
			{
				StringBuilder builder = new StringBuilder();

				foreach (AfdelingsJaar aj in (this as Leiding).AfdelingsJaar)
				{
					if (builder.Length > 0)
					{
						builder.Append(", ");
					}
					builder.Append(aj.Afdeling.Naam);
				}
				return builder.ToString();
			}
			else
			{
				Debug.Assert(false, "Lid moet kind of leiding zijn.");
				return String.Empty;	// Hier komen we toch niet.
			}
		}
	}
}
