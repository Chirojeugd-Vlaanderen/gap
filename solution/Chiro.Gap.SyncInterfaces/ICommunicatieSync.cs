// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van communicatiemiddelen naar Kipadmin
	/// </summary>
	public interface ICommunicatieSync
	{
		/// <summary>
		/// Verwijdert een communicatievorm uit Kipadmin
		/// </summary>
		/// <param name="communicatieVorm">Te verwijderen communicatievorm, gekoppeld aan een gelieerde persoon 
		/// met ad-nummer.</param>
		void Verwijderen(CommunicatieVorm communicatieVorm);

		/// <summary>
		/// Bewaart een communicatievorm in Kipadmin
		/// </summary>
		/// <param name="commvorm">Te bewaren communicatievorm, gekoppeld aan persoon</param>
		void Toevoegen(CommunicatieVorm commvorm);

        /// <summary>
        /// Stuurt de gegeven <paramref name="communicatieVorm"/> naar Kipadmin. Om te weten welk de
        /// originele communicatievorm is, kijken we naar de gekoppelde persoon, en gebruiken we
        /// het oorspronkelijke nummer (<paramref name="origineelNummer"/>)
        /// </summary>
        /// <param name="communicatieVorm">Te updaten communicatievorm</param>
        /// <param name="origineelNummer">Oorspronkelijk nummer van die communicatievorm</param>
        /// <remarks>Het is best mogelijk dat het 'nummer' niet is veranderd, maar bijv. enkel de vlag 
        /// 'opt-in'</remarks>
        void Bijwerken(CommunicatieVorm communicatieVorm, string origineelNummer);
    }
}