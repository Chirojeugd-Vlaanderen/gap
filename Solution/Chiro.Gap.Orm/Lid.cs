// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
	/// <summary>
	/// Maakt een onderscheid tussen kinderen en leiding
	/// </summary>
	[DataContract]
	public enum LidType
	{
		[EnumMember]
		Kind = 1,
		[EnumMember]
		Leiding = 2
	}

	/// <summary>
	/// Instantieert een Lid-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
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
		/// Geeft een lijst terug van alle afdelingen waaraan het lid gekoppeld is.
		/// </summary>
		/// <returns>Lijst met afdelingen</returns>
		/// <remarks>Een kind is hoogstens aan 1 afdeling gekoppeld</remarks>
		public IList<int> AfdelingIdLijstGet()
		{
			IList<int> result = new List<int>();
			if (this is Kind)
			{
				if ((this as Kind).AfdelingsJaar != null)
				{
					result.Add((this as Kind).AfdelingsJaar.Afdeling.ID);
				}
			}
			else if (this is Leiding)
			{
				foreach (AfdelingsJaar aj in (this as Leiding).AfdelingsJaar)
				{
					result.Add(aj.Afdeling.ID);
				}
			}
			else
			{
				Debug.Assert(false, "Lid moet kind of leiding zijn.");
			}

			return result;
		}
	}
}
