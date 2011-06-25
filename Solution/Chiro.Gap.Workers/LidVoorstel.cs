using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Workers
{
	public class LidVoorstel
	{
		/// <summary>
		/// In welk afdelingsjaar het lid moet worden ingeschreven.
		/// </summary>
		public int? AfdelingsJaarID;
		/// <summary>
		/// Of het lid moet worden ingeschreven als leiding
		/// </summary>
		public bool LeidingMaken;
	}
}
