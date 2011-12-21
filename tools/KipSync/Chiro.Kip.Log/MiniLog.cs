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
		public void BerichtLoggen(int groepID, string bericht)
		{
			Loggen(groepID, bericht, false);
		}

		/// <summary>
		/// Logt een foutbericht mbt de groep met id <paramref name="groepID"/>.
		/// </summary>
		/// <param name="groepID">(Kipdorp)ID van groep waarop logbericht van toepassing</param>
		/// <param name="bericht">Te loggen foutbericht</param>
		public void FoutLoggen(int groepID, string bericht)
		{
			Loggen(groepID, bericht, true);
		}


		/// <summary>
		/// Logt een bericht of fout mbt de groep met id <paramref name="groepID"/>.
		/// </summary>
		/// <param name="groepID">(Kipdorp)ID van groep waarop logbericht van toepassing</param>
		/// <param name="bericht">Te loggen bericht</param>
		/// <param name="ernstig"><c>true</c> als het om een fout gaat, <c>false</c> voor
		/// een 'gewoon' bericht</param>
		public void Loggen(int groepID, string bericht, Boolean ernstig)
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

			    var b = new Bericht
			                {
			                    bericht =
			                        bericht.Length > Properties.Settings.Default.MaxLenLogBericht
			                            ? bericht.Substring(0, Properties.Settings.Default.MaxLenLogBericht)
			                            : bericht,
			                    datum = tijd,
			                    ernstig = ernstig,
			                    gebruiker = userDef
			                };

				db.AddToBerichtSet(b);
				db.SaveChanges();
			}
		}

	}
}
