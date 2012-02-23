// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Orm.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van bivakaangifte naar Kipadmin
	/// </summary>
	public interface IBivakSync
	{
		/// <summary>
		/// Geeft de <paramref name="uitstap"/> door aan Kipadmin als bivakaangifte.
		/// </summary>
		/// <param name="uitstap">Te bewaren uitstap</param>
		void Bewaren(Uitstap uitstap);

		/// <summary>
		/// Verwijdert uitstap met gegeven <paramref name="uitstapID"/> uit Kipadmin.
		/// </summary>
		/// <param name="uitstapID">Te verwijderen</param>
		void Verwijderen(int uitstapID);
	}
}
