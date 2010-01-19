using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Cdf.Data;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Data.Test
{
    /// <summary>
    /// Data-Access tests voor afdelingen en alles wat
    /// daarrond hangt.
    /// </summary>
    /// <remarks>Deze tests werken op een testgroep in
    /// de database.  Het script om de testgroep te maken,
    /// zit in SVN: 
    /// https://develop.chiro.be/subversion/cg2/trunk/database/TestData/TestData/TestGroepMaken.sql
    /// 
    /// De IDs nodig voor de test zitten in de settings van dit project.
    /// </remarks>
    [TestClass]
    public class AfdelingsTest
    {
        public AfdelingsTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [ClassInitialize]
        static public void TestsInitialiseren(TestContext context)
        {
            // Initialiseer IoC-container.  Niet zeker van of dit
            // een goeie plaats is...
            Factory.ContainerInit();

            int gwjID = TestInfo.GROEPSWERKJAARID;
            int afd2ID = TestInfo.AFDELING2ID;

            // Verwijder mogelijk afdelingsjaar voor afdeling2

            IAfdelingsJarenDao ajDao = Factory.Maak<IAfdelingsJarenDao>();
            AfdelingsJaar aj = ajDao.Ophalen(gwjID, afd2ID);

            if (aj != null)
            {
                aj.TeVerwijderen = true;
                ajDao.Bewaren(aj);
            }


        }

        /// <summary>
        /// Opzoeken van een testafdelingsjaar
        /// </summary>
        [TestMethod]
        public void AfdelingsJaarOpzoeken()
        {
            #region Arrange
            int gwjID = TestInfo.GROEPSWERKJAARID;
            int afdID = TestInfo.AFDELINGID;

            IAfdelingsJarenDao dao = Factory.Maak<IAfdelingsJarenDao>();
            #endregion

            #region Act
            AfdelingsJaar aj = dao.Ophalen(gwjID, afdID);
            #endregion

            #region Assert
            Assert.IsTrue(aj != null);
            Assert.IsTrue(aj.GroepsWerkJaar.ID == gwjID);
            Assert.IsTrue(aj.Afdeling.ID == afdID);
            #endregion
        }

        /// <summary>
        /// Haalt de afdeling op bepaald door TestAfdelingID,
        /// en controleert of die gekoppeld is aan groep
        /// bepaald door TestGroepID
        /// (zie property's)
        /// </summary>
        [TestMethod]
        public void AfdelingOphalen()
        {
            #region Arrange
            int afdID = TestInfo.AFDELINGID;
            int groepID = TestInfo.GROEPID;

            IAfdelingenDao dao = Factory.Maak<IAfdelingenDao>();
            #endregion

            #region Act
            Afdeling afd = dao.Ophalen(afdID);
            #endregion

            #region Assert
            Assert.IsTrue(afd != null);
            Assert.IsTrue(afd.Groep.ID == groepID);
            #endregion
        }

        /// <summary>
        /// Creeert een AfdelingsWerkJaar voor de afdeling
        /// bepaald door TestAfdeling2ID in het groepswerkjaar
        /// bepaald door TestGroepsWerkJaarID (zie settings)
        /// </summary>
        [TestMethod]
        public void AfdelingsJaarCreeren()
        {
            // Arrange

            IGroepsWerkJaarDao gwDao = Factory.Maak<IGroepsWerkJaarDao>();
            IAfdelingenDao aDao = Factory.Maak<IAfdelingenDao>();
            IAfdelingsJarenDao ajDao = Factory.Maak<IAfdelingsJarenDao>();
            IDao<OfficieleAfdeling> oaDao = Factory.Maak<IDao<OfficieleAfdeling>>();

            GroepsWerkJaarManager wjm = Factory.Maak<GroepsWerkJaarManager>();

            int gwjID = TestInfo.GROEPSWERKJAARID;
            int afd2ID = TestInfo.AFDELING2ID;
            int oaID = TestInfo.OFFICIELEAFDELINGID;

            int van = TestInfo.AFDELING2VAN;
            int tot = TestInfo.AFDELING2TOT;

            // Voor het gemak haal ik groepswerkjaar en afdeling via
            // de DAO's op ipv via de workers.

            GroepsWerkJaar gw = gwDao.Ophalen(gwjID, gwj=>gwj.Groep);
            Afdeling afd = aDao.Ophalen(afd2ID);
            OfficieleAfdeling oa = oaDao.Ophalen(oaID);

            // Het afdelingsjaar wordt gemaakt door een worker.

            AfdelingsJaar aj = wjm.AfdelingsJaarMaken(gw, afd, oa, van, tot);

            // Act

            // Bewaren *MOET* gebeuren via de DAO, want het is de
            // dao die we testen; we willen niet dat een fout in de
            // worker de test doet failen.

            ajDao.Bewaren(aj);

            // Nu opnieuw ophalen.

            AfdelingsJaar aj2 = ajDao.Ophalen(gwjID, afd2ID);

            // Assert

            Assert.IsTrue(aj2.ID > 0);
            Assert.IsTrue(aj2.GroepsWerkJaar.ID == gwjID);
            Assert.IsTrue(aj2.Afdeling.ID == afd2ID);
            Assert.IsTrue(aj2.OfficieleAfdeling.ID == oaID);

            // Cleanup is niet meer nodig, want het nieuw gemaakte
            // afdelingsjaar wordt verwijderd in TestsInitialiseren
        }

        /// <summary>
        /// Controleert of OphalenMetAfdelingen uit GroepenDao
        /// slechts 1 afdelingsjaar oplevert voor de testafdeling.
        /// </summary>
        [TestMethod]
        public void OphalenMetAfdelingenAfdelingsJaar()
        {
            // Arrange

            IGroepenDao dao = Factory.Maak<IGroepenDao>();

            // Act

            Groep g = dao.OphalenMetAfdelingen(TestInfo.GROEPSWERKJAARID);

            // Assert

            var ajQuery = (
                from afd in g.Afdeling
                where afd.ID == TestInfo.AFDELINGID
                select afd.AfdelingsJaar);

            Assert.IsTrue(ajQuery.Count() == 1);
        }

        /// <summary>
        /// Controleert de koppelingen Groep->Afdeling->AfdelingsJaar->GroepsWerkJaar->Groep
        /// na GroepenDao.OphalenMetAfdelingen.
        /// </summary>
        [TestMethod]
        public void OphalenMetAfdelingenGroepsWerkJaar()
        {
            // Arrange

            IGroepenDao dao = Factory.Maak<IGroepenDao>();

            // Act

            Groep g = dao.OphalenMetAfdelingen(TestInfo.GROEPSWERKJAARID);

            // Assert

            foreach (Afdeling a in g.Afdeling)
            {
                foreach (AfdelingsJaar aj in a.AfdelingsJaar)
                {
                    Assert.IsTrue(aj.GroepsWerkJaar.Groep.ID == g.ID);
                }
            }

        }
    }
}
