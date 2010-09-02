using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.KipUpdate;

namespace Chiro.Kip.Services.Test
{
	/// <summary>
	/// Dummy updater die zogezegd ad-nummers terug naar GAP stuurt, maar in praktijk niks doet.
	/// </summary>
	public class DummyPersoonUpdater: IPersoonUpdater
	{
		#region IPersoonUpdater Members

		public void AdNummerZetten(int persoonID, int adNummer)
		{
		}

		#endregion
	}
}
