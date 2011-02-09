using Chiro.Kip.Services.UpdateService;

namespace Chiro.Kip.Services.Test
{
	/// <summary>
	/// Dummy updater die zogezegd ad-nummers terug naar GAP stuurt, maar in praktijk niks doet.
	/// </summary>
	public class DummyPersoonUpdater: IUpdateService
	{
		#region IPersoonUpdater Members

		public void AdNummerToekennen(int persoonID, int adNummer)
		{
		}

		#endregion
	}
}
