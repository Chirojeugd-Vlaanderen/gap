using System;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Interface voor synchronisatie van lidinfo naar Kipadmin
	/// </summary>
	public class LedenSync: ILedenSync 
	{
		/// <summary>
		/// Stuurt een lid naar Kipadmin
		/// </summary>
		/// <param name="l">Te bewaren lid</param>
		public void Bewaren(Lid l)
		{
			throw new NotImplementedException();
		}
	}
}
