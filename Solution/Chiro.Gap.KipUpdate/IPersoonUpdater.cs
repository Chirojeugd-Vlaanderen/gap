namespace Chiro.Gap.KipUpdate
{
	/// <summary>
	/// Updates die Kipadmin moet kunnen uitvoeren op personen
	/// </summary>
	public interface IPersoonUpdater
	{
		void AdNummerZetten(int persoonID, int adNummer);
	}
}