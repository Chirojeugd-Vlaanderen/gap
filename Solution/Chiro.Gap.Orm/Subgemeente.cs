using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Een straat heeft geen versie (timestamp) in de database.
	/// Dat lijkt me ook niet direct nodig voor een klasse die
	/// bijna nooit wijzigt.
	/// 
	/// Het feit dat er geen timestamp is, wil wel zeggen dat
	/// 'concurrencygewijze' de laatste altijd zal winnen.    
	/// </summary>
	public partial class Subgemeente : IEfBasisEntiteit
	{
		#region IBasisEntiteit Members

		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		// SubGemeente wordt nooit geüpdatet, dus ook nooit
		// concurrency.  VersieString is dus niet nodig.
		public string VersieString
		{
			get { return null; }
			set { /*Doe niets*/ }
		}

		#endregion

		public override int GetHashCode()
		{
			return 18;
		}
	}
}
