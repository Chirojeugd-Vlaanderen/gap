using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Klasse voor Chirogroep, die het meeste gewoon erft van Groep.
	/// </summary>
	public partial class ChiroGroep
	{
		/// <summary>
		/// Het niveau van een Chirogroep is altijd Niveau.Groep
		/// </summary>
		public override Niveau Niveau
		{
			get { return Niveau.Groep; }
		}
	}
}
