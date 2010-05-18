// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Domain
{
	public class FoutNummers
	{
		private FoutNummers()
		{
			// Deze klasse bevat enkel constanten, en mag niet geinstantieerd worden.
			// Vandaar een private constructor.
		}

		public const int AlgemeneFout = 0x01;
		public const int GeenGav = 0x02;
		public const int BestaatAl = 0x03;
		public const int ValidatieFout = 0x04;

		#region adressen
		public const int WoonPlaatsNietGevonden = 0x20;
		public const int StraatNietGevonden = 0x21;

		public const int WonenDaarAl = 0x28;
		#endregion

		#region verkeerde groep
		public const int CategorieNietVanGroep = 0x30;
		public const int FunctieNietVanGroep = 0x31;
		public const int AfdelingNietVanGroep = 0x32;
		#endregion

		#region niet beschikbaar in werkjaar
		public const int GroepsWerkJaarNietBeschikbaar = 0x40;
		public const int FunctieNietBeschikbaar = 0x41;
		public const int AfdelingNietBeschikbaar = 0x42;
		#endregion

		#region nog te verdelen
		public const int CategorieNietLeeg = 0xf0;
		#endregion
	}
}
