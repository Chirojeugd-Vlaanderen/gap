using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Kip.Data
{
	public partial class ChiroGroep
	{
		/// <summary>
		/// Geeft true als de groep een gewest of een verbond is
		/// </summary>
		public bool IsGewestVerbond
		{
			get
			{
				return (String.Compare(TYPE, "G", true) == 0 ||
				        String.Compare(TYPE, "V", true) == 0);
			}
		}
	}
}
