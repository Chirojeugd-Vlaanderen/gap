using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

using Cg2.EfWrapper.Entity;

namespace Cg2.EfWrapper.Test
{
    internal static class TestHelpers
    {
        /// <summary>
        /// Stomme functie die een nieuwe persoon aanmaakt in de
        /// DB met 2 adressen.
        /// </summary>
        /// <returns>GelieerdePersoonsID</returns>
        public static int PersoonMetTweeAdressenMaken()
        {
            GelieerdePersoon p = new GelieerdePersoon { ChiroLeefTijd = 2 };
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

            using (Entities db = new Entities())
            {
                GelieerdePersoon geattacht = db.AttachObjectGraph(p, bla => bla.PersoonsAdres, bla => bla.PersoonsAdres.First().Adres);
                db.SaveChanges();

                nieuwePersoonID = geattacht.ID;
            }

            return nieuwePersoonID;
        }

        internal static GelieerdePersoon PersoonOphalen(int persoonID)
        {
            GelieerdePersoon p;
            using (Entities db = new Entities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                p = (from gp in db.GelieerdePersoon.Include("PersoonsAdres.Adres")
                     where gp.ID == persoonID
                     select gp).FirstOrDefault();
            }
            return p;
        }
    }
}
