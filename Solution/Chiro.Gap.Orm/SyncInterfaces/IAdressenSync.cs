using System.Collections.Generic;

namespace Chiro.Gap.Orm.SyncInterfaces
{
	public interface IAdressenSync
	{
		/// <summary>
		/// Stelt de gegeven persoonsadressen in als standaardadressen in Kipadmin
		/// </summary>
		/// <param name="persoonsAdressen">Persoonsadressen die als standaardadressen (adres 1) naar
		/// Kipadmin moeten.  Personen moeten gekoppeld zijn, net zoals adressen met straatnaam en gemeente</param>
		void StandaardAdressenBewaren(IEnumerable<PersoonsAdres> persoonsAdressen);
	}
}