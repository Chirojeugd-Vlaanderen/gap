using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Orm.SyncInterfaces
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
		/// <param name="lid">Lid</param>
		/// <remarks>*Alle* relevante gegevens van het lidobject worden hier sowieso opnieuw opgehaald, anders was het
		/// te veel een gedoe.</remarks>
		void AfdelingenUpdaten(Lid lid);
	}
}
