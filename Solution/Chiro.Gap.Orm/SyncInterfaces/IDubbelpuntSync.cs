// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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