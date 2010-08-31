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
		void FunctiesUpdaten(Lid lid);
	}
}
