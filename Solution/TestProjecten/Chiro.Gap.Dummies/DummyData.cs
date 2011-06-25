using System;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy data om mee te spelen in tests
	/// </summary>
	public class DummyData
	{
		private const string NIEUWEFUNCODE = "DOM";
		private const string NIEUWEFUNNAAM = "Domme functie";
		private const int NIEUWEFUNID = 206;

		private readonly ChiroGroep _dummyGroep;		// testgroep
		private readonly KaderGroep _dummyGewest;		// testgewest
		private readonly GroepsWerkJaar _huidigGwj;		// testgroepswerkjaar
		private readonly GroepsWerkJaar _vorigGwj;		// vorig groepswerkjaar
		private readonly GroepsWerkJaar _gwjGewest;		// groepswerkjaar van het gewest
		private readonly GelieerdePersoon _gelieerdeJos;	// gelieerdePersoon genaamd 'Jos' 
		private readonly GelieerdePersoon _gelieerdeIrene;	// gelieerdePersoon genaamd 'Irene'
		private readonly GelieerdePersoon _gelieerdeYvonne;	// gelieerdePersoon genaamd 'Yvonne'
		private readonly GelieerdePersoon _gelieerdeKaderJos;	// Jos gekoppeld aan het gewest
		private readonly Categorie _vervelend;			// categorie voor vervelende mensen
		private readonly Functie _redactie;			// functie voor 1 persoon
		private readonly Functie _contactPersoonFunctie;	// domme algemene functie
		private readonly Functie _feestcomite;			// nog een functie
		private readonly Functie _wcmadam;			// en nog eentje; deze mag nergens toegekend zijn
		private readonly Lid _leiderJos;			// lidobject Jos
		private readonly Lid _leiderJosVorigJaar;		// lidobject Jos vorig jaar
		private readonly Lid _lidYvonne;			// lidobject Yvonne
		private readonly Lid _kaderJos;				// lidobject Jos in het gewest

		/// <summary>
		/// Een groep met daaraan gekoppeld een aantal leden
		/// </summary>
		public Groep DummyGroep { get { return _dummyGroep; } }

		/// <summary>
		/// Een gewest
		/// </summary>
		public KaderGroep DummyGewest { get { return _dummyGewest;  } }

		/// <summary>
		/// Huidig groepswerkjaar voor de testgroep
		/// </summary>
		public GroepsWerkJaar HuidigGwj { get { return _huidigGwj; } }

		/// <summary>
		/// Huidig groepswerkjaar voor het testgewest
		/// </summary>
		public GroepsWerkJaar GwjGewest { get { return _gwjGewest; } }

		/// <summary>
		/// De gelieerde testpersoon 'Jos'
		/// </summary>
		public GelieerdePersoon GelieerdeJos { get { return _gelieerdeJos; } }

		/// <summary>
		/// De gelieerde testpersoon 'Irene'
		/// </summary>
		public GelieerdePersoon GelieerdeIrene { get { return _gelieerdeIrene; } }

		/// <summary>
		/// De gelieerde testpersoon 'Yvonne'
		/// </summary>
		public GelieerdePersoon GelieerdeYvonne { get { return _gelieerdeYvonne; } }

		/// <summary>
		/// Jos gekoppeld aan het gewest
		/// </summary>
		public GelieerdePersoon GelieerdeKaderJos { get { return _gelieerdeKaderJos; } }

		/// <summary>
		/// Jos' lidobject
		/// </summary>
		public Lid LeiderJos { get { return _leiderJos; } }

		/// <summary>
		/// Yvonnes lidobject
		/// </summary>
		public Lid LidYvonne { get { return _lidYvonne; } }

		/// <summary>
		/// Jos' lidobject in het kader
		/// </summary>
		public Lid KaderJos { get { return _kaderJos; } }

		/// <summary>
		/// De categorie voor vervelende mensen
		/// </summary>
		public Categorie Vervelend { get { return _vervelend; } }

		/// <summary>
		/// Functie voor 1 persoon
		/// </summary>
		public Functie UniekeFunctie { get { return _redactie; } }

		/// <summary>
		/// Een functie die zowel vorig jaar als dit jaar gebruikt is.
		/// </summary>
		public Functie TraditieFunctie { get { return _feestcomite; } }

		/// <summary>
		/// Functie die nooit iemand gehad heeft
		/// </summary>
		public Functie OngebruikteFunctie { get { return _wcmadam; } }

		/// <summary>
		/// Algemene functie, van toepassing op eender welk lid
		/// </summary>
		public Functie ContactPersoonFunctie { get { return _contactPersoonFunctie; } }


		/// <summary>
		/// Herstelt de dummydata naar de oorspronkelijke toestand
		/// </summary>
		/// <remarks>Deze method reset de unity configuratie, om te vermijden dat de werking beïnvloed
		/// wordt door vorige tests die met de IOC-container hebben 'gemoost'.</remarks>
		public DummyData()
		{
			Factory.ContainerInit();

			// TODO: Door de objecten te bewaren krijgen ze ID's, waardoor de tests betrouwbaarder
			// worden.  Op het einden zou er dus ergens 
			// GroepenDao.Bewaren(_dummyGroep, lambda-expressie-die-alles-meeneemt)
			// aangeroepen moeten worden.

			var wjMgr = Factory.Maak<GroepsWerkJaarManager>();
			var gpMgr = Factory.Maak<GelieerdePersonenManager>();
			var gMgr = Factory.Maak<GroepenManager>();
			var cgMgr = Factory.Maak<ChiroGroepenManager>();
			var lMgr = Factory.Maak<LedenManager>();
			var cMgr = Factory.Maak<CategorieenManager>();
			var afdMgr = Factory.Maak<AfdelingsJaarManager>();
			var fMgr = Factory.Maak<FunctiesManager>();

			// Groep en groepswerkjaar

			_dummyGewest = new KaderGroep { Naam = "Gewest Test", Code = "tst/0000", NiveauInt = 8};

			_dummyGroep = new ChiroGroep { Naam = "St.-Unittestius", Code = "tst/0001", KaderGroep = _dummyGewest};
			_huidigGwj = gMgr.GroepsWerkJaarMaken(_dummyGroep, 2009);
			_gwjGewest = gMgr.GroepsWerkJaarMaken(_dummyGewest, 2009);
			_vorigGwj = gMgr.GroepsWerkJaarMaken(_dummyGroep, 2008);

			// Categorie

			_vervelend = gMgr.CategorieToevoegen(_dummyGroep, "vervelende mensen", "last");

			// Functie (nationaal bepaald, maar dit ter zijde)

			_redactie = gMgr.FunctieToevoegen(_dummyGroep, "Hoofdredacteur boekje", "HRE", 1, 0, LidType.Alles, null);
			_feestcomite = gMgr.FunctieToevoegen(_dummyGroep, "Feestcomite", "ZUIP", null, 0, LidType.Alles, null);
			_wcmadam = gMgr.FunctieToevoegen(_dummyGroep, "WC-madam", "SHT", null, 0, LidType.Alles, null);

			_contactPersoonFunctie = new Functie
			                   	{
							ID = (int)NationaleFunctie.ContactPersoon,
			                   		Code = "CP",
			                   		Naam = "Algemene Functie",
			                   		MinAantal = 0,
			                   		MaxAantal = null,
			                   		Niveau = Niveau.Alles,
							IsNationaal = true,
			                   		Groep = null		// 't is een nationale functie
			                   	};

			// Afdelingen gekoppeld aan officiële afdelingen in afdelingsjaren

			var ribbels = new OfficieleAfdeling { Naam = "Ribbels" };
			var rakwis = new OfficieleAfdeling { Naam = "Rakwi's" };

			var unittestjes = cgMgr.AfdelingToevoegen(_dummyGroep, "unittestjes", "ut");
			var speelkwis = cgMgr.AfdelingToevoegen(_dummyGroep, "speelkwi's", "sk");

			afdMgr.Aanmaken(unittestjes, ribbels, _huidigGwj, 2001, 2003, GeslachtsType.Gemengd);
			afdMgr.Aanmaken(speelkwis, rakwis, _huidigGwj, 1998, 2000, GeslachtsType.Gemengd);

			// Gelieerde personen

			var jos = new Persoon
			{
				Naam = "Bosmans",
				VoorNaam = "Jos",
				GeboorteDatum = new DateTime(1988, 6, 28),	// oud genoeg, zodat hij leider wordt :)
				AdNummer = 1,
				Geslacht = GeslachtsType.Man
			};

			var irene = new Persoon
			{
				Naam = "Bosmans",
				VoorNaam = "Irène",
				GeboorteDatum = new DateTime(1990, 3, 8),
				Geslacht = GeslachtsType.Vrouw
			};

			var yvonne = new Persoon
			{
				Naam = "Bosmans",
				VoorNaam = "Yvonne",
				GeboorteDatum = new DateTime(1999, 3, 8),
				Geslacht = GeslachtsType.Vrouw
			};

			_gelieerdeJos = gpMgr.Koppelen(jos, _dummyGroep, 0);
			_gelieerdeIrene = gpMgr.Koppelen(irene, _dummyGroep, 0);
			_gelieerdeYvonne = gpMgr.Koppelen(yvonne, _dummyGroep, 0);
			_gelieerdeKaderJos = gpMgr.Koppelen(jos, _dummyGewest, 0);

			// Koppelingen allerhanden
			gpMgr.CategorieKoppelen(new GelieerdePersoon[] { _gelieerdeJos }, _vervelend);

			// We moeten hier expliciet lid maken in _huidigGwj, anders werken een aantal
			// unit tests niet meer.  (Zie #259)

			_leiderJos = lMgr.AutomagischInschrijven(_gelieerdeJos, _huidigGwj, false);
			_leiderJosVorigJaar = lMgr.AutomagischInschrijven(_gelieerdeJos, _vorigGwj, false);
			_lidYvonne = lMgr.AutomagischInschrijven(_gelieerdeYvonne, _huidigGwj, false);
			_kaderJos = lMgr.AutomagischInschrijven(_gelieerdeKaderJos, _gwjGewest, false);

			// ID's worden niet toegekend als de DAO's gemockt zijn, dus delen we die manueel
			// uit.

			_vorigGwj.ID = 1;
			_huidigGwj.ID = 2;

			_leiderJos.ID = 1;
			_lidYvonne.ID = 2;
			_kaderJos.ID = 3;

			// Jos krijgt functies

			fMgr.Toekennen(_leiderJos, new Functie[] { _redactie, _feestcomite });

			// Functie van vorig jaar kan ik niet toekennen via de workers, dus manueel gepruts
			_leiderJosVorigJaar.Functie.Add(_feestcomite);
			_feestcomite.Lid.Add(_leiderJosVorigJaar);

			fMgr.Toekennen(_kaderJos, new Functie[] {_contactPersoonFunctie});
		}

		/// <summary>
		/// Genereert een kloon van de gelieerde persoon Jos, zoals die bijv. door
		/// een service opgeleverd zou kunnen worden.
		/// </summary>
		/// <returns></returns>
		public GelieerdePersoon KloonJos()
		{
			var testData = new DummyData();

			var gp = new GelieerdePersoon();
			gp.Persoon = new Persoon();

			gp.ID = testData.GelieerdeJos.ID;
			gp.Persoon.AdNummer = testData.GelieerdeJos.Persoon.AdNummer;
			gp.Persoon.GeboorteDatum = testData.GelieerdeJos.Persoon.GeboorteDatum;
			gp.Persoon.Geslacht = testData.GelieerdeJos.Persoon.Geslacht;
			gp.Persoon.ID = testData.GelieerdeJos.Persoon.ID;
			gp.Persoon.Naam = testData.GelieerdeJos.Persoon.Naam;

			return gp;
		}

		/// <summary>
		/// Property voor een code van een niet-bestaande functie
		/// </summary>
		public string NieuweFunctieCode { get { return NIEUWEFUNCODE; } }

		/// <summary>
		/// Property voor een naam van een niet-bestaande functie
		/// </summary>
		public string NieuweFunctieNaam { get { return NIEUWEFUNNAAM; } }

		/// <summary>
		/// ID dat toegewezen mag worden aan de nieuwe functie
		/// </summary>
		public int NieuweFunctieID { get { return NIEUWEFUNID;  } }
	}
}
