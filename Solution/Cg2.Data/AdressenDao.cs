using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using System.Diagnostics;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    /// <summary>
    /// Data access voor adressen; zie IAdressenDao voor documentatie.
    /// </summary>
    public class AdressenDao: Dao<Adres>, IAdressenDao
    {
        public override Adres Creeren(Adres adr)
        {
            // Deze assertions moeten eigenlijk afgedwongen worden
            // door de businesslaag.  En eigenlijk moet deze method ook
            // werken zonder die asserties (en dan de juiste dingen
            // bijcreeren).
            // Voorlopig niet dus.

            Debug.Assert(adr.Straat != null);
            Debug.Assert(adr.Subgemeente != null);
            Debug.Assert(adr.Straat.ID != 0);
            Debug.Assert(adr.Subgemeente.ID != 0);

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // Ik ga ervan uit dat straat en subgemeente niet gewijzigd
                // zijn.  Maar ik denk dat de DAO wel het wijzigen van
                // straten moet toelaten (bijv voor een admingebruiker of zo)
                // Maar voorlopig dus niet.

                // Entity Framework heeft problemen met het koppelen van
                // een combinatie van nieuwe en detached objecten aan de
                // objectcontext.  In dit geval: het adres is nieuw,
                // straat en subgemeente zijn gedetached.  Vandaar deze lelijke
                // hack.

                Straat s = adr.Straat;
                Subgemeente sg = adr.Subgemeente;

                adr.Straat = null;
                adr.Subgemeente = null;

                db.AddToAdres(adr);
                db.Attach(s);
                db.Attach(sg);

                adr.Straat = s;
                adr.Subgemeente = sg;

                db.SaveChanges();
                // Als er een dubbel adres wordt toegevoegd,
                // krijg je hier een exceptie.
                // FXIME: dat is momenteel wel een probleem,
                // want syncen verloopt niet in 1 transactie
            }

            return adr;
        }

        public Adres BewonersOphalen(int adresID, IList<int> gelieerdAan)
        {
            Adres resultaat = null;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // query opbouwen

                var query
                    = db.PersoonsAdres.Include("Adres.Straat")
                    .Include("Adres.Subgemeente")
                    .Include("GelieerdePersoon.Persoon")
                    .Where(pera => pera.Adres.ID == adresID);

                if (gelieerdAan != null)
                {
                    query = query
                        .Where(Utility.BuildContainsExpression<PersoonsAdres, int>(pera => pera.GelieerdePersoon.Groep.ID, gelieerdAan));
                }

                query = query.Select(pera => pera);

                // genereer lijst met alle persoonsadressen.

                IList<PersoonsAdres> persoonsAdressen = query.ToList();


                // deze zijn allemaal gekoppeld aan hetzelfde adres
                // (met gegeven adresid).  Het eerste adres is dus al
                // ok.  (Ik ga er voor het gemak even van uit dat
                // er altijd een adres met een gekoppelde persoon
                // gevonden wordt.)

                Debug.Assert(persoonsAdressen.Count > 0);

                resultaat = persoonsAdressen[0].Adres;
            }

            return resultaat;
        }

        public Adres Ophalen(string straatNaam, int? huisNr, string bus, int postNr, string postCode, string gemeenteNaam)
        {
            Adres resultaat;
            Straat s;
            Subgemeente sg;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                
                resultaat = (
                    from Adres a in db.Adres
                    where (a.Straat.Naam == straatNaam && a.Subgemeente.Naam == gemeenteNaam
                    && a.Straat.PostNr == postNr
                    && a.HuisNr == huisNr && a.Bus == bus && a.PostCode == postCode)
                    select a).FirstOrDefault();

                // Alweer een rariteit met entity framework:
                //
                // Als ik de eerste lijn van de query vervang door
                //   from Adres a in db.Adres.Include("Straat").Include("Subgemeente")
                //
                // dan zou je verwachten dat resultaat.Straat en resultaat.Subgemeente
                // meteen mee opgehaald worden.  (Net zoals het bijvoorbeeld
                // gebeurt in GelieerdePersonenDau.DetailsOphalen.
                //
                // Niet dus.  Vandaar dat ik de 'eager loading' maar op deze
                // manier doe:

                resultaat.StraatReference.Load();
                resultaat.SubgemeenteReference.Load();

                // Als ik nu resultaat zou detachen, zijn straat en subgemeente
                // opnieuw null.  Begrijpe wie kan.  Maar via deze rare
                // constructie lukt het dan weer wel:

                s = resultaat.Straat;
                sg = resultaat.Subgemeente;

                db.Detach(s);
                db.Detach(sg);
                db.Detach(resultaat);
            }
            resultaat.Straat = s;
            resultaat.Subgemeente = sg;

            return resultaat;
        }
    }
}
