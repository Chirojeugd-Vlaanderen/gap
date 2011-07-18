namespace Chiro.Gap.Orm.SyncInterfaces
{
	/// <summary>
	/// Klasse voor de synchronisatie van dubbelpuntabonnementen
	/// </summary>
	public interface IDubbelpuntSync
	{
        /// <summary>
        /// Synct een dubbelpuntabonnement naar Kipadmin
        /// </summary>
        /// <param name="abonnement">Te syncen abonnement</param>
		void Abonneren(Abonnement abonnement);
	}
}