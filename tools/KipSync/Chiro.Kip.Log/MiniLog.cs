using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Kip.Data;

namespace Chiro.Kip.Log
{
	/// <summary>
	/// Zeer minimale logging, in afwachting van iets beters. (TODO #307)
	/// </summary>
	public class MiniLog : IMiniLog
	{
		/// <summary>
		/// Logt een bericht mbt de groep met id <paramref name="groepID"/>.
		/// </summary>
		/// <param name="groepID">(Kipdorp)ID van groep waarop logbericht van toepassing</param>
		/// <param name="bericht">Te loggen bericht</param>
		public void Log(int groepID, string bericht)
		{
			using (var db = new kipadminEntities())
			{
				string userDef;	// string die beeld geeft over de user
				DateTime tijd = DateTime.Now;

				var groep = db.Groep.Where(grp => grp.GroepID == groepID).FirstOrDefault();
                                    
				if (groep == null)
				{
					userDef = "KipSync";
				}
				else if (groep is ChiroGroep)
				{
					userDef = String.Format("KipSync{0} {1}", groepID, (groep as ChiroGroep).STAMNR);
				}
				else
				{
					userDef = String.Format("KipSync{0}", groepID);
				}

				Console.WriteLine("{0}:{1}:{2}", tijd, userDef, bericht);

				var b = new Bericht {bericht = bericht, datum = tijd, ernstig = false, gebruiker = userDef};

				db.AddToBerichtSet(b);
				db.SaveChanges();
			}
		}
	}
}
