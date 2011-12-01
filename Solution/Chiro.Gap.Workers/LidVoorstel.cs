using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Workers
{
	public class LidVoorstel
	{
		/// <summary>
		/// In welk afdelingsjaren het lid moet worden ingeschreven.
		/// </summary>
		public IEnumerable<int> AfdelingsJaarIDs;
		/// <summary>
		/// True als er geen rekening moet worden gehouden met de inhoud van afdelingsjaarIDs
		/// </summary>
		public bool AfdelingsJarenIrrelevant;
		/// <summary>
		/// Of het lid moet worden ingeschreven als leiding
		/// </summary>
		public bool LeidingMaken;
	}
}
