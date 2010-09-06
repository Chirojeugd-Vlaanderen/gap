namespace Chiro.Gap.Orm.SyncInterfaces
{
	/// <summary>
	/// Klasse voor de synchronisatie van dubbelpuntabonnementen
	/// </summary>
	public interface IDubbelpuntSync
	{
		/// <summary>
		/// Synct een Dubbelpuntabonnement voor het huidige groepswerkjaar naar Kipadmin.
		/// </summary>
		/// <param name="gp">Gelieerde persoon die een abonnement wil voor dit werkjaar</param>
		void Abonneren(GelieerdePersoon gp);
	}
}