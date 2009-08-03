using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Objects;

using Cg2.EfWrapper.Entity;

namespace Cg2.EfWrapper.Test
{
    /// <summary>
    /// Summary description for ToevoegTest
    /// </summary>
    [TestClass]
    public class ToevoegTest
    {
        public ToevoegTest()
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

        /// <summary>
        /// Test op toevoegen van 1 entity via AttachEntityGraph
        /// </summary>
        [TestMethod]
        public void EntityToevoegenViaAttachEntityGraph()
        {
            #region Arrange

            GelieerdePersoon p = new GelieerdePersoon { ChiroLeefTijd = 1 };
            int nieuwID;

            #endregion
            #region Act

            using (Entities db = new Entities())
            {
                GelieerdePersoon geattacht = db.AttachObjectGraph(p, null);
                db.SaveChanges();

                nieuwID = geattacht.ID;
            }

            #endregion
            #region Assert

            using (Entities db2 = new Entities())
            {
                GelieerdePersoon q = (from gp in db2.GelieerdePersoon
                                      where gp.ID == nieuwID
                                      select gp).FirstOrDefault();
                Assert.AreEqual(p.ChiroLeefTijd, 1);
            }

            #endregion
        }

        /// <summary>
        /// Test op toevoegen van graaf
        /// 2 keer een 1 op veelrelatie toegevoegd,
        /// van 'veel' naar '1'.
        /// </summary>
        [TestMethod]
        public void VolledigeGraafToevoegen_VeelNaar1()
        {
            #region Arrange

            GelieerdePersoon p = new GelieerdePersoon { ChiroLeefTijd = -1 };
            Adres a1 = new Adres { Bus = "b", PostCode="" };

            // In het echte programma wordt natuurlijk de
            // PersonenManager gebruikt om adressen te koppelen,
            // zodat je zelf niet moet wakker liggen van alle 
            // referenties goed te leggen.  Maar in deze test
            // werken we niet op de echte entities, maar op
            // entities uit een testdatabase, vandaar dat het
            // manueel moet.  (Ik doe niet graag automatische
            // tests op de echte database.)

            PersoonsAdres pa1 = new PersoonsAdres { GelieerdePersoon = p, Adres = a1, Opmerking = "Eerste adres", IsStandaard = true };

            p.PersoonsAdres.Add(pa1);
            a1.PersoonsAdres.Add(pa1);

            int nieuwePersoonID;

            #endregion
            #region Act

            using (Entities db = new Entities())
            {
                PersoonsAdres geattacht = db.AttachObjectGraph(pa1, bla => bla.GelieerdePersoon, bla => bla.Adres);
                db.SaveChanges();

                nieuwePersoonID = geattacht.GelieerdePersoon.ID;
            }

            #endregion
            #region Assert

            using (Entities db2 = new Entities())
            {
                GelieerdePersoon q = (from gp in db2.GelieerdePersoon.Include("PersoonsAdres").Include("PersoonsAdres.Adres")
                                      where gp.ID == nieuwePersoonID
                                      select gp).FirstOrDefault();

                Assert.AreEqual(q.PersoonsAdres.Count(), 1);
                Assert.AreEqual(q.PersoonsAdres.First().Opmerking, "Eerste adres");
                Assert.AreEqual(q.PersoonsAdres.First().Adres.Bus, "b");
            }

            #endregion
        }


        /// <summary>
        /// Test op toevoegen van graaf met enkel nieuwe
        /// nodes.
        /// </summary>
        [TestMethod]
        public void VolledigeGraafToevoegen()
        {
            #region Arrange

            GelieerdePersoon p = new GelieerdePersoon { ChiroLeefTijd = -1 };
            Adres a1 = new Adres { Bus = "", PostCode = "" };
            Adres a2 = new Adres { Bus = "", PostCode = "" };

            // In het echte programma wordt natuurlijk de
            // PersonenManager gebruikt om adressen te koppelen,
            // zodat je zelf niet moet wakker liggen van alle 
            // referenties goed te leggen.  Maar in deze test
            // werken we niet op de echte entities, maar op
            // entities uit een testdatabase, vandaar dat het
            // manueel moet.  (Ik doe niet graag automatische
            // tests op de echte database.)

            PersoonsAdres pa1 = new PersoonsAdres { GelieerdePersoon = p, Adres = a1, Opmerking = "Eerste adres", IsStandaard = true };
            PersoonsAdres pa2 = new PersoonsAdres { GelieerdePersoon = p, Adres = a2, Opmerking = "Tweede adres", IsStandaard = false };

            p.PersoonsAdres.Add(pa1);
            p.PersoonsAdres.Add(pa2);
            a1.PersoonsAdres.Add(pa1);
            a2.PersoonsAdres.Add(pa2);

            int nieuwePersoonID;

            #endregion
            #region Act

            using (Entities db = new Entities())
            {
                GelieerdePersoon geattacht = db.AttachObjectGraph(p, bla => bla.PersoonsAdres, bla => bla.PersoonsAdres.First().Adres);
                db.SaveChanges();

                nieuwePersoonID = geattacht.ID;
            }

            #endregion
            #region Assert

            using (Entities db2 = new Entities())
            {
                GelieerdePersoon q = (from gp in db2.GelieerdePersoon.Include("PersoonsAdres").Include("PersoonsAdres.Adres")
                                      where gp.ID == nieuwePersoonID
                                      select gp).FirstOrDefault();

                Assert.AreEqual(q.PersoonsAdres.Count(), 2);
                Assert.IsTrue((q.PersoonsAdres.First().Opmerking == "Eerste adres"
                    && q.PersoonsAdres.Last().Opmerking == "Tweede adres")
                    || (q.PersoonsAdres.First().Opmerking == "Tweede adres"
                    && q.PersoonsAdres.Last().Opmerking == "Eerste adres"));
                Assert.AreNotEqual(q.PersoonsAdres.First().Adres, q.PersoonsAdres.Last().Adres);
            }

            #endregion
        }

        /// <summary>
        /// Test op toevoegen van 1 node in bestaande graaf
        /// </summary>
        [TestMethod]
        public void AdresToevoegen()
        {
            #region Arrange
            int persoonID = TestHelpers.PersoonMetTweeAdressenMaken();
            GelieerdePersoon p = TestHelpers.PersoonOphalen(persoonID);
            #endregion

            #region Act
            Adres a1 = new Adres { Bus = "a", PostCode = "" };
            PersoonsAdres pa1 = new PersoonsAdres { GelieerdePersoon = p, Adres = a1, Opmerking = "Toegevoegd adres", IsStandaard = false };

            p.PersoonsAdres.Add(pa1);
            a1.PersoonsAdres.Add(pa1);

            using (Entities db2 = new Entities())
            {
                db2.AttachObjectGraph(p, bla => bla.PersoonsAdres, bla => bla.PersoonsAdres.First().Adres);
                db2.SaveChanges();
            }
            #endregion

            #region Assert
            using (Entities db3 = new Entities())
            {
                GelieerdePersoon q = (from gp in db3.GelieerdePersoon.Include("PersoonsAdres.Adres")
                                      where gp.ID == persoonID
                                      select gp).FirstOrDefault();

                Assert.AreEqual(q.PersoonsAdres.Count, 3);
            }
            #endregion

        }
    }
}
