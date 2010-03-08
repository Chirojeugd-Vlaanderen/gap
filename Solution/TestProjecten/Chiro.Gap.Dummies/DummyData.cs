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
		private static GelieerdePersoon _gelieerdeIrene; // gelieerdePersoon genaamd 'Irene'
		private static GelieerdePersoon _gelieerdeYvonne; // gelieerdePersoon genaamd 'Yvonne'
		private static Categorie _vervelend;		// categorie voor vervelende mensen
		private static Functie _redactie;		// functie voor 1 persoon
		private static Lid _lidJos;			// lidobject Jos
		private static Lid _lidYvonne;			// lidobject Yvonne

		/// <summary>
		/// Een groep met daaraan gekoppeld een aantal leden
		/// </summary>
		public static Groep DummyGroep { get { return _dummyGroep; } }

		/// <summary>
		/// Huidig groepswerkjaar voor de testgroep
		/// </summary>
		public static GroepsWerkJaar HuidigGwj { get { return _huidigGwj; } }

		/// <summary>
		/// De gelieerde testpersoon 'Jos'
		/// </summary>
		public static GelieerdePersoon GelieerdeJos { get { return _gelieerdeJos; } }

		/// <summary>
		/// De gelieerde testpersoon 'Irene'
		/// </summary>
		public static GelieerdePersoon GelieerdeIrene { get { return _gelieerdeIrene; } }

		/// <summary>
		/// De gelieerde testpersoon 'Yvonne'
		/// </summary>
		public static GelieerdePersoon GelieerdeYvonne { get { return _gelieerdeYvonne; } }

		/// <summary>
		/// Jos' lidobject
		/// </summary>
		public static Lid LidJos { get { return _lidJos; } }

		/// <summary>
		/// Yvonnes lidobject
		/// </summary>
		public static Lid LidYvonne { get { return _lidYvonne; } }

		/// <summary>
		/// De categorie voor vervelende mensen
		/// </summary>
		public static Categorie Vervelend { get { return _vervelend; } }

		/// <summary>
		/// Functie voor 1 persoon
		/// </summary>
		public static Functie UniekeFunctie { get { return _redactie; } }

		/// <summary>
		/// Bouwt de dummy data op
		/// </summary>
		static DummyData()
		{
			GroepsWerkJaarManager wjMgr = Factory.Maak<GroepsWerkJaarManager>();
			GelieerdePersonenManager gpMgr = Factory.Maak<GelieerdePersonenManager>();
			GroepenManager gMgr = Factory.Maak<GroepenManager>();
			LedenManager lMgr = Factory.Maak<LedenManager>();
			CategorieenManager cMgr = Factory.Maak<CategorieenManager>();
			FunctiesManager fMgr = Factory.Maak<FunctiesManager>();

			// Groep en groepswerkjaar

			_dummyGroep = new Groep { Naam = "St.-Unittestius", Code = "tst/0001" };

			_huidigGwj = gMgr.GroepsWerkJaarMaken(_dummyGroep, 2009);

			// Categorie

			_vervelend = gMgr.CategorieToevoegen(_dummyGroep, "vervelende mensen", "last");

			// Functie

			_redactie = new Functie { Code = "HRE", Naam = "Hoofdredacteur", MaxAantal = 1 };

			// Afdelingen gekoppeld aan officiële afdelingen in afdelingsjaren

			OfficieleAfdeling ribbels = new OfficieleAfdeling { Naam = "Ribbels" };
			OfficieleAfdeling rakwis = new OfficieleAfdeling { Naam = "Rakwi's" };

			Afdeling unittestjes = gMgr.AfdelingToevoegen(_dummyGroep, "unittestjes", "ut");
			Afdeling speelkwis = gMgr.AfdelingToevoegen(_dummyGroep, "speelkwi's", "sk");

			wjMgr.AfdelingsJaarMaken(_huidigGwj, unittestjes, ribbels, 2001, 2003);
			wjMgr.AfdelingsJaarMaken(_huidigGwj, speelkwis, rakwis, 1998, 2000);

			// Gelieerde personen

			Persoon jos = new Persoon { 
				Naam = "Bosmans", 
				VoorNaam = "Jos", 
				GeboorteDatum = new DateTime(2000, 6, 28),
				AdNummer = 1};

			Persoon irene = new Persoon {
				Naam = "Bosmans",
				VoorNaam = "Irène",
				GeboorteDatum = new DateTime(1990, 3, 8) };
			
			Persoon yvonne = new Persoon
			{
				Naam = "Bosmans",
				VoorNaam = "Yvonne",
				GeboorteDatum = new DateTime(1999, 3, 8)

			};


			_gelieerdeJos = gpMgr.Koppelen(jos, _dummyGroep, 0);
			_gelieerdeIrene = gpMgr.Koppelen(irene, _dummyGroep, 0);
			_gelieerdeYvonne = gpMgr.Koppelen(yvonne, _dummyGroep, 0);


			// Koppelingen allerhanden
			gpMgr.CategorieKoppelen(new GelieerdePersoon[] { _gelieerdeJos }, _vervelend);

			// We moeten hier expliciet lid maken in _huidigGwj, anders werken een aantal
			// unit tests niet meer.  (Zie #259)

			_lidJos = lMgr.KindMaken(_gelieerdeJos, _huidigGwj);
			_lidYvonne = lMgr.KindMaken(_gelieerdeYvonne, _huidigGwj);

			// ID's worden niet toegekend als de DAO's gemockt zijn, dus delen we die manueel
			// uit.

			_lidJos.ID = 1;
			_lidYvonne.ID = 2;

			fMgr.Toekennen(_lidJos, new Functie[] { _redactie });
			

		}

		/// <summary>
		/// Genereert een kloon van de gelieerde persoon Jos, zoals die bijv. door
		/// een service opgeleverd zou kunnen worden.
		/// </summary>
		/// <returns></returns>
		public static GelieerdePersoon KloonJos()
		{
			GelieerdePersoon gp = new GelieerdePersoon();
			gp.Persoon = new Persoon();

			gp.ID = DummyData.GelieerdeJos.ID;
			gp.Persoon.AdNummer = DummyData.GelieerdeJos.Persoon.AdNummer;
			gp.Persoon.GeboorteDatum = DummyData.GelieerdeJos.Persoon.GeboorteDatum;
			gp.Persoon.Geslacht = DummyData.GelieerdeJos.Persoon.Geslacht;
			gp.Persoon.ID = DummyData.GelieerdeJos.Persoon.ID;
			gp.Persoon.Naam = DummyData.GelieerdeJos.Persoon.Naam;

			return gp;
		}
	}
}
