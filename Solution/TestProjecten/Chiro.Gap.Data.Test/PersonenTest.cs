// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.Workers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Summary description for PersonenTest
	/// </summary>
	[TestClass]
	public class PersonenTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[ClassInitialize]
		static public void TestInitialiseren(TestContext context)
		{
			Factory.ContainerInit();

			// Maak persoon aan, die bij het testen weer
			// verwijderd kan worden.

			int groepID = TestInfo.GROEPID;
			string naam = TestInfo.NIEUWEPERSOONNAAM;
			string voornaam = TestInfo.TEVERWIJDERENVOORNAAM;

			var gdao = Factory.Maak<IGroepenDao>();
			Groep g = gdao.Ophalen(groepID);

			var gpm = Factory.Maak<GelieerdePersonenManager>();

			// Creeer gloednieuwe persoon

			var p = new Persoon();
			p.VoorNaam = voornaam;
			p.Naam = naam;

			// Koppel aan testgroep, Chiroleeftijd 0

			GelieerdePersoon gp = gpm.Koppelen(p, g, 0);

			var gpdao = Factory.Maak<IGelieerdePersonenDao>();
			// Hier moeten we via de DAO gaan, en niet via de worker, omdat 
			// we de DAO willen testen, en niet willen failen op fouten in
			// de worker.

			gp = gpdao.Bewaren(gp);
		}

		/// <summary>
		/// Opkuis na de test; verwijdert bijgemaakte personen
		/// </summary>
		[ClassCleanup]
		static public void Opkuis()
		{
			int groepID = TestInfo.GROEPID;
			string naam = TestInfo.NIEUWEPERSOONNAAM;
			string voornaam = TestInfo.NIEUWEPERSOONVOORNAAM;

			var mgr = Factory.Maak<GelieerdePersonenManager>();

			// nog niet alle functionaliteit wordt aangeboden in de worker,
			// dus ik werk hier en daar rechtstreeks op de dao.

			var dao = Factory.Maak<IGelieerdePersonenDao>();

			IList<GelieerdePersoon> gevonden = dao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);

			foreach (GelieerdePersoon gp in gevonden)
			{
				mgr.VolledigVerwijderen(gp);	// verwijderen persisteert direct
			}
		}

		[TestMethod]
		public void ZoekenOpNaam()
		{
			// arrange
			string zoekString = TestInfo.ZOEKNAAM;
			int groepID = TestInfo.GROEPID;

			var dao = Factory.Maak<IGelieerdePersonenDao>();

			// act
			IList<GelieerdePersoon> lijst = dao.ZoekenOpNaam(groepID, zoekString);

			// assert
			Assert.IsTrue(lijst.Count >= 2);
		}

		/// <summary>
		/// Test zoeken op basis van SoundEx; wordt er een GelieerdePersoon gevonden, en komt de persoonsinfo mee?
		/// </summary>
		[TestMethod]
		public void ZoekenOpNaamOngeveer()
		{
			// arrange
			var dao = Factory.Maak<IGelieerdePersonenDao>();

			// act
			IList<GelieerdePersoon> lijst = dao.ZoekenOpNaamOngeveer(
				TestInfo.GROEPID, 
				TestInfo.ZOEKNAAM, 
				TestInfo.ZOEKVOORNAAMONGEVEER);

			// assert
			Assert.IsTrue(lijst.Count >= 1);
			Assert.IsTrue(lijst.First().Persoon != null);
		}

		/// <summary>
		/// Test om te kijken of categorie"en meekomen met details.
		/// </summary>
		[TestMethod]
		public void DetailsOphalenCategorie()
		{
			// arange
			int gpID = TestInfo.GELIEERDEPERSOONID;
			var dao = Factory.Maak<IGelieerdePersonenDao>();

			// act
			GelieerdePersoon gp = dao.DetailsOphalen(gpID);

			// assert
			Assert.IsTrue(gp.Categorie.Count > 0);
		}

		/// <summary>
		/// Kijkt na of opgehaalde details wel geserializeerd 
		/// kunnen worden door een DataContractSerializer.
		/// </summary>
		[TestMethod]
		public void OphalenSerializable()
		{
			// arange
			GelieerdePersoon gp, kloon;

			int gpID = TestInfo.GELIEERDEPERSOONID;
			var dao = Factory.Maak<IGelieerdePersonenDao>();


			using (var stream = new MemoryStream())
			{
				// act

				gp = dao.Ophalen(gpID, pers => pers.Categorie);

				var serializer = new NetDataContractSerializer();
				serializer.Serialize(stream, gp);
				stream.Position = 0;
				kloon = (GelieerdePersoon)serializer.Deserialize(stream);
			}

			// assert

			Assert.AreEqual(gp, kloon);
		}

		/// <summary>
		/// Ook als een pagina met lidinfo opgehaald wordt, moet het resultaat serializable
		/// zijn.
		/// </summary>
		[TestMethod]
		public void OphalenMetLidInfoSerializable()
		{
			// arange
			int totaal;

			var dao = Factory.Maak<IGelieerdePersonenDao>();

			// act

			// Haal 30 personen op, daar zitten allicht leden bij
			var lijst = dao.PaginaOphalenMetLidInfo(
				TestInfo.GROEPID, 1, 30, PersoonSorteringsEnum.Naam, out totaal);

			using (var stream = new MemoryStream())
			{
				// act

				var serializer = new NetDataContractSerializer();
				serializer.Serialize(stream, lijst);
				stream.Position = 0;
				var kloon = (IEnumerable<GelieerdePersoon>)serializer.Deserialize(stream);
			}

			// assert

			// Als we hier geraken zonder problemen, is het OK
			Assert.IsTrue(true);
		}

/*		/// <summary>
		/// Test voor het ophalen van een persoon, inclusief lidinfo en afdelingen.  De persoon in kwestie
		/// is een kind.
		/// </summary>
		[TestMethod]
		public void OphalenPersoonMetAfdelingenKind()
		{
			// Arrange
			var dao = Factory.Maak<IGelieerdePersonenDao>();

			// Act
			GelieerdePersoon resultaat = dao.OphalenMetAfdelingen(
				TestInfo.GELIEERDEPERSOON5ID, 
				TestInfo.GROEPSWERKJAARID, 
				gp => gp.Persoon);

			
			// Assert

			Assert.IsTrue(resultaat.Lid.Count() == 1);	// 1 lidobject opgehaald
			Assert.IsTrue(resultaat.Lid.First() is Kind);	// het is een kind
			Assert.IsTrue((resultaat.Lid.First() as Kind).AfdelingsJaar.Afdeling != null);	
		}

		/// <summary>
		/// Test voor het ophalen van een persoon, inclusief lidinfo en afdelingen.  De persoon in kwestie
		/// is leiding.
		/// </summary>
		[TestMethod]
		public void OphalenPersoonMetAfdelingenLeiding()
		{
			// Arrange
			var dao = Factory.Maak<IGelieerdePersonenDao>();

			// Act
			GelieerdePersoon resultaat = dao.OphalenMetAfdelingen(
				TestInfo.GELIEERDEPERSOON3ID,
				TestInfo.GROEPSWERKJAARID,
				gp => gp.Persoon);


			// Assert

			// slechts 1 lidobject opgehaald
			Assert.IsTrue(resultaat.Lid.Count() == 1);
			// het is leiding
			Assert.IsTrue(resultaat.Lid.First() is Leiding);
			// van 2 afdelingen
			Assert.IsTrue((resultaat.Lid.First() as Leiding).AfdelingsJaar.Count() == 2);
		}

		/// <summary>
		/// Test voor het ophalen van een persoon, inclusief lidinfo en afdelingen.  Hier wordt
		/// expliciet getest op gerelateerde op te halen entiteiten
		/// </summary>
		[TestMethod]
		public void OphalenPersoonMetAfdelingenGerelateerdeEntiteiten()
		{
			// Arrange
			var dao = Factory.Maak<IGelieerdePersonenDao>();

			// Act
			GelieerdePersoon resultaat = dao.OphalenMetAfdelingen(
				TestInfo.GELIEERDEPERSOON5ID,
				TestInfo.GROEPSWERKJAARID,
				gp => gp.Persoon);


			// Assert

			Assert.IsTrue(resultaat.Persoon != null);
		}*/

		/// <summary>
		/// Nieuwe (gelieerde) persoon bewaren via GelieerdePersonenDAO.
		/// </summary>
		[TestMethod]
		public void NieuwePersoon()
		{
			#region Arrange

			int groepID = TestInfo.GROEPID;
			string naam = TestInfo.NIEUWEPERSOONNAAM;
			string voornaam = TestInfo.NIEUWEPERSOONVOORNAAM;

			var gdao = Factory.Maak<IGroepenDao>();
			Groep g = gdao.Ophalen(groepID);

			var gpm = Factory.Maak<GelieerdePersonenManager>();

			// Creeer gloednieuwe persoon

			var p = new Persoon();
			p.VoorNaam = voornaam;
			p.Naam = naam;

			// Koppel aan testgroep, Chiroleeftijd 0

			GelieerdePersoon gp = gpm.Koppelen(p, g, 0);

			var gpdao = Factory.Maak<IGelieerdePersonenDao>();
			#endregion

			#region Act
			// Hier moeten we via de DAO gaan, en niet via de worker, omdat 
			// we de DAO willen testen, en niet willen failen op fouten in
			// de worker.

			gp = gpdao.Bewaren(gp);
			#endregion

			#region Assert
			IList<GelieerdePersoon> gevonden = gpdao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);
			Assert.IsTrue(gevonden.Count > 0);
			#endregion

		}


		/// <summary>
		/// Test om persoon te verwijderen.
		/// </summary>
		[TestMethod]
		public void VerwijderPersoon()
		{
			#region Arrange
			// zoek te verwijderen personen op

			int groepID = TestInfo.GROEPID;
			string naam = TestInfo.NIEUWEPERSOONNAAM;
			string voornaam = TestInfo.TEVERWIJDERENVOORNAAM;

			var mgr = Factory.Maak<GelieerdePersonenManager>();

			// nog niet alle functionaliteit wordt aangeboden in de worker,
			// dus ik werk hier en daar rechtstreeks op de dao.

			var dao = Factory.Maak<IGelieerdePersonenDao>();

			IList<GelieerdePersoon> gevonden = dao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);
			#endregion

			#region Act
			foreach (GelieerdePersoon gp in gevonden)
			{
				mgr.VolledigVerwijderen(gp);	// verwijderen persisteert direct
			}
			#endregion

			#region Assert
			IList<GelieerdePersoon> gevonden2 = dao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);
			Assert.IsTrue(gevonden2.Count == 0);
			#endregion

		}

	}
}
