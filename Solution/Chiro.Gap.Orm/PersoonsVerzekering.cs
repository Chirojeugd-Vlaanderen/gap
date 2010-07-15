using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Entity voor PersoonsVerzekering
	/// </summary>
	public partial class PersoonsVerzekering: IEfBasisEntiteit
	{
		private bool _teVerwijderen;

		/// <summary>
		/// Wordt gebruikt om te verwjderen entiteiten mee te markeren
		/// </summary>
		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		/// <summary>
		/// Versiestring voor concurrency control
		/// </summary>
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

		/// <summary>
		/// Hashcode gewoon overnemen van ID
		/// </summary>
		/// <returns>De hashcode</returns>
		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			IEfBasisEntiteit andere = obj as VerzekeringsType;
			// Als obj geen VerzekeringsType is, wordt andere null.

			return andere != null && (ID != 0) && (ID == andere.ID)
				|| (ID == 0 || andere.ID == 0) && base.Equals(andere);

			// Is obj geen Verzekeringstype, dan is de vergelijking altijd vals.
			// Hebben beide objecten een ID verschillend van 0, en zijn deze
			// ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
			// base.Equals, wat eigenlijk niet helemaal correct is.
		}
	}
}
