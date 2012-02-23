// <copyright file="IAdressenSync.cs" company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

namespace Chiro.Gap.Orm.SyncInterfaces
{
    /// <summary>
    /// Regelt de synchronisatie van adresgegevens naar Kipadmin
    /// </summary>
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