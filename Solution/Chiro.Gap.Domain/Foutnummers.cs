using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Domain
{
	public static class FoutNummers
	{
		public static readonly int AlgemeneFout = 0x00;
		public static readonly int GeenGav = 0x01;
		public static readonly int BestaatAl = 0x02;

		#region adressen
		public static readonly int WoonPlaatsNietGevonden = 0x20;
		public static readonly int StraatNietGevonden = 0x21;

		public static readonly int WonenDaarAl = 0x28;
		#endregion

		#region verkeerde groep
		public static readonly int CategorieNietVanGroep = 0x30;
		public static readonly int FunctieNietVanGroep = 0x31;
		public static readonly int AfdelingNietVanGroep = 0x32;
		#endregion

		#region niet beschikbaar in werkjaar
		public static readonly int GroepsWerkJaarNietBeschikbaar = 0x40;
		public static readonly int FunctieNietBeschikbaar = 0x41;
		public static readonly int AfdelingNietBeschikbaar = 0x42;
		#endregion

		#region nog te verdelen
		public static readonly int CategorieNietLeeg = 0xf0;
		#endregion
	}
}
