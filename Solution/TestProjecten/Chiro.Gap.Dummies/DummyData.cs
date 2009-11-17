using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy data om mee te spelen in tests
	/// </summary>
	public static class DummyData
	{
		private static Groep _dummyGroep;		// testgroep
		private static GroepsWerkJaar _huidigGwj;	// testgroepswerkjaar
		private static GelieerdePersoon _gelieerdeJos;	// gelieerdePersoon genaamd 'Jos' 

		/// <summary>
		/// Een groep met daaraan gekoppeld een aantal leden
		/// </summary>
		public static Groep DummyGroep { get { return _dummyGroep; } }

		/// <summary>
		/// Huidig groepswerkjaar voor de testgroep
		/// </summary>
		public static GroepsWerkJaar HuidigGwj { get { return _huidigGwj; } }

		public static GelieerdePersoon GelieerdeJos { get { return _gelieerdeJos; } }


		/// <summary>
		/// Bouwt de dummy data op
		/// </summary>
		static DummyData()
		{
			WerkJaarManager wjMgr = Factory.Maak<WerkJaarManager>();
			GelieerdePersonenManager gpMgr = Factory.Maak<GelieerdePersonenManager>();
			GroepenManager gMgr = Factory.Maak<GroepenManager>();
			LedenManager lMgr = Factory.Maak<LedenManager>();

			_dummyGroep = new Groep { Naam = "St.-Unittestius", Code = "tst/0001" };

			_huidigGwj = gMgr.GroepsWerkJaarMaken(_dummyGroep, 2009);

			OfficieleAfdeling ribbels = new OfficieleAfdeling { Naam = "Ribbels" };
			OfficieleAfdeling rakwis = new OfficieleAfdeling { Naam = "Rakwi's" };

			Afdeling unittestjes = gMgr.AfdelingToevoegen(_dummyGroep, "unittestjes", "ut");
			Afdeling speelkwis = gMgr.AfdelingToevoegen(_dummyGroep, "speelkwi's", "sk");

			wjMgr.AfdelingsJaarMaken(_huidigGwj, unittestjes, ribbels, 2001, 2003);
			wjMgr.AfdelingsJaarMaken(_huidigGwj, speelkwis, rakwis, 1998, 2000);

			Persoon jos = new Persoon { 
				Naam = "Bosmans", 
				VoorNaam = "Jos", 
				GeboorteDatum = new DateTime(2000, 6, 28) };

			Persoon irene = new Persoon {
				Naam = "Bosmans",
				VoorNaam = "Irène",
				GeboorteDatum = new DateTime(1990, 3, 8) };

			_gelieerdeJos = gpMgr.Koppelen(jos, _dummyGroep, 0);
			gpMgr.Koppelen(irene, _dummyGroep, 0);

			lMgr.LidMaken(_gelieerdeJos, _huidigGwj);						
		}
	}
}
