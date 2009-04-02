using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using System.Diagnostics;
using Cg2.Orm.DataInterfaces;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

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
            IList<PersoonsAdres> lijst = null;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

                // NoTracking schakelt object tracking uit.  Op die manier
                // moet het resultaat niet gedetachet worden.  Detachen
                // deed blijkbaar de navigation properties verdwijnen.
                // (Voor entity framework moeten alle objecten in een
                // graaf ofwel aan dezelfde context hangen, ofwel aan
                // geen context hangen.  Het adres detachen, koppelt het
                // los van al de rest.)
                
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

                // Nadeel van de NoTracking is dat aan elk persoonsadres
                // een identieke kopie van hetzelfde adres zal hangen.
                // Dat moet sebiet manueel gefixt worden.

                 lijst = query.ToList();
            }

            // FIXME: Voor het gemak ga ik ervan uit dat er steeds minstens
            // iemand op het gezochte adres woont.  Dat is uiteraard niet 
            // noodzakelijk het geval.

            Debug.Assert(lijst.Count > 0);
            
            // Koppel nu alle PersoonsAdressen aan dezelfde instantie van Adres.
            // (lijst[0].Adres, vandaar dat we lijst[0] zelf niet moeten updaten)

            for (int i = 1; i < lijst.Count; ++i)
            {
                lijst[i].Adres = lijst[0].Adres;
                lijst[0].Adres.PersoonsAdres.Add(lijst[i]);
            };

            resultaat = lijst[0].Adres;

            return resultaat;
        }

        public Adres Ophalen(string straatNaam, int? huisNr, string bus, int postNr, string postCode, string gemeenteNaam)
        {
            Adres resultaat;
            Straat s;
            Subgemeente sg;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Adres.MergeOption = MergeOption.NoTracking;
                // NoTracking, zodat we straks niet moeten detachen
                // (op die manier spelen we geen navigation properties
                // kwijt)
                
                resultaat = (
                    from Adres a in db.Adres.Include("Straat").Include("Subgemeente")
                    where (a.Straat.Naam == straatNaam && a.Subgemeente.Naam == gemeenteNaam
                    && a.Straat.PostNr == postNr
                    && a.HuisNr == huisNr && a.Bus == bus && a.PostCode == postCode)
                    select a).FirstOrDefault();

                // Alweer een rariteit met entity framework:
                // De 'eager loading' van Straat en Subgemeente werkt niet.
                //
                // Dan moet het maar zo:

                if (resultaat != null)
                {
                    // Dit is uiteraard enkel zinvol als er iets gevonden is.

                    resultaat.StraatReference.Load();
                    resultaat.SubgemeenteReference.Load();

                }
            }

            return resultaat;
        }
    }
}
