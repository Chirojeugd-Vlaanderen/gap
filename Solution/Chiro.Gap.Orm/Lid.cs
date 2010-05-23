// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
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

		/// <summary>
		/// Geeft een LidType weer, wat Kind of Leiding kan zijn.
		/// </summary>
		public LidType Type
		{
			get { return (this is Kind ? LidType.Kind : LidType.Leiding); }
		}

		// public override int GetHashCode()
		// {
		//         return ID.GetHashCode();
		// }
		 
		// public override bool Equals(object obj)
		// {
		//         return this.ChiroEquals(obj);
		// }
	}
}
