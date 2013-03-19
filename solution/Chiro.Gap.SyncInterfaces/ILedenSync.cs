// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van lidinfo naar Kipadmin
	/// </summary>
	public interface ILedenSync
	{
		/// <summary>
		/// Stuurt een lid naar Kipadmin
		/// </summary>
		/// <param name="l">Te bewaren lid</param>
		void Bewaren(Lid l);

		/// <summary>
		/// Updatet de functies van het lid in Kipadmin
		/// </summary>
		/// <param name="lid">Lid met functies</param>
		/// <remarks>Als er geen persoonsgegevens meegeleverd zijn, halen we die wel even op :)</remarks>
		void FunctiesUpdaten(Lid lid);

		/// <summary>
		/// Updatet de afdelingen van <paramref name="lid"/> in Kipadmin
		/// </summary>
		/// <param name="lid">Het lidobject dat we bewerken</param>
		/// <remarks>Alle (!) relevante gegevens van het lidobject worden hier sowieso opnieuw opgehaald, anders was het
		/// te veel een gedoe.</remarks>
		void AfdelingenUpdaten(Lid lid);

		/// <summary>
		/// Updatet het lidtype van <paramref name="lid"/> in Kipadmin
		/// </summary>
		/// <param name="lid">Lid waarvan het lidtype geupdatet moet worden</param>
		void TypeUpdaten(Lid lid);

        /// <summary>
        /// Verwijdert een lid uit Kipadmin
        /// </summary>
        /// <param name="lid">Te verwijderen lid</param>
	    void Verwijderen(Lid lid);

        /// <summary>
        /// Stuurt een aantal leden naar Kipadmin
        /// </summary>
        /// <param name="leden">Te bewaren leden</param>
	    void Bewaren(IList<Lid> leden);
	}
}
