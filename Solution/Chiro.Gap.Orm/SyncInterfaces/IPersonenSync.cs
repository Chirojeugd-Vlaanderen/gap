// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Orm.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van persoonsinfo naar Kipadmin
	/// </summary>
	public interface IPersonenSync
	{
		/// <summary>
		/// Stuurt de persoonsgegevens, samen met eventueel adressen en/of communicatie, naar Kipadmin
		/// </summary>
		/// <param name="gp">Gelieerde persoon, persoonsinfo</param>
		/// <param name="metStandaardAdres">Stuurt ook het standaardadres mee (moet dan wel gekoppeld zijn)</param>
		/// <param name="metCommunicatie">Stuurt ook communicatie mee.  Hiervoor wordt expliciet alle
		/// communicatie-info opgehaald, omdat de workers typisch niet toestaan dat de gebruiker alle
		/// communicatie ophaalt.</param>
		void Bewaren(GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie);

		/// <summary>
		/// Stuurt enkel de communicatie van een gelieerde persoon naar Kipadmin
		/// </summary>
		/// <param name="gp">Gelieerde persoon, met daaraan gekoppeld zijn communicatie</param>
		void CommunicatieUpdaten(GelieerdePersoon gp);
	}
}