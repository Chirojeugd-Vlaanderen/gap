// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een Afdeling-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	public partial class Afdeling : IEfBasisEntiteit
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

		#region Identity en equality

		public override int GetHashCode()
		{
			return 17;
		}

		public override bool Equals(object obj)
		{
			IEfBasisEntiteit andere = obj as Afdeling;
			// Als obj geen Afdeling is, wordt andere null.

			return andere != null && (ID != 0) && (ID == andere.ID)
				|| (ID == 0 || andere.ID == 0) && base.Equals(andere);

			// Is obj geen Afdeling, dan is de vergelijking altijd vals.
			// Hebben beide objecten een ID verschillend van 0, en zijn deze
			// ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
			// base.Equals, wat eigenlijk niet helemaal correct is.
		}

		#endregion
	}
}
