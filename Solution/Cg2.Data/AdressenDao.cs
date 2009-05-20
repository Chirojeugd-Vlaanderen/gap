using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

using Cg2.EfWrapper;
using Cg2.EfWrapper.Entity;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;



namespace Cg2.Data.Ef
{
    /// <summary>
    /// Data access voor adressen; zie IAdressenDao voor documentatie.
    /// </summary>
    public class AdressenDao: Dao<Adres>, IAdressenDao
    {
        /// <summary>
        /// Creeert nieuw adres, en bewaart gekoppelde
        /// persoonsadressen.
        /// </summary>
        /// <param name="adr">Te creeren adres</param>
        /// <returns>referentie naar gecreerde adres</returns>
        public override Adres Creeren(Adres adr)
        {
            return Bewaren(adr);
        }

        /// <summary>
        /// Bewaart adres en eventuele gekoppelde persoonsadressen.
        /// </summary>
        /// <param name="nieuweEntiteit">Te bewaren adres</param>
        /// <returns>referentie naar het bewaarde adres</returns>
        /// <opmerking>ID en Versie worden aangepast in Adres, eventuele
        /// gewijzigde velden in gerelateerde entity's niet!</opmerking>
        public override Adres Bewaren(Adres adr)
        {
            Adres bewaardAdres;

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
                bewaardAdres = db.AttachObjectGraph(adr, dink=>dink.Straat.WithoutUpdate(), dink=>dink.Subgemeente.WithoutUpdate(), dink => dink.PersoonsAdres.First());

                // bewaardAdres is het geattachte adres.  Hiervan neem
                // ik ID en versie over; de rest laat ik ongemoeid.
                //
                // (waardoor rowversies van gekoppelde entity's niet
                // meer zullen kloppen :(.  Maar ik kan bewaardadres
                // ook niet detachen zonder de navigation property's
                // te verliezen :(.)

                db.SaveChanges();

                adr.ID = bewaardAdres.ID;
                adr.Versie = bewaardAdres.Versie;
            }
            return adr;
        }

        public Adres BewonersOphalen(int adresID, string user)
        {
            // DAT IS HIER ZWAAR AAN TE PASSEN :-)

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
                    

                if (user != "")
                {
                    // Controleer op niet-vervallen gebruikersrecht

                    query = query
                        .Where(pera => pera.GelieerdePersoon.Groep.GebruikersRecht.Any(
                            gr => gr.Gav.Login == user && (gr.VervalDatum == null 
                                || gr.VervalDatum < DateTime.Now)));
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

        /// <summary>
        /// Haalt adres op op basis van criteria
        /// </summary>
        /// <param name="straatNaam"></param>
        /// <param name="huisNr"></param>
        /// <param name="bus"></param>
        /// <param name="postNr"></param>
        /// <param name="postCode"></param>
        /// <param name="gemeenteNaam"></param>
        /// <param name="metBewoners">indien true, worden ook de
        /// persoonsadressen opgehaald (ALLEMAAL, dus niet zomaar
        /// over de lijn sturen!)</param>
        /// <returns>Een adres als gevonden, anders null</returns>
        public Adres Ophalen(string straatNaam, int? huisNr, string bus, int postNr, string postCode, string gemeenteNaam, bool metBewoners)
        {
            Adres resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Adres.MergeOption = MergeOption.NoTracking;
                // NoTracking, zodat we straks niet moeten detachen
                // (op die manier spelen we geen navigation properties
                // kwijt)

                var adressentabel = db.Adres.Include("Straat").Include("Subgemeente");

                if (metBewoners)
                {
                    adressentabel = adressentabel.Include("PersoonsAdres");
                }
                
                resultaat = (
                    from Adres a in adressentabel
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

                    if (metBewoners)
                    {
                        resultaat.PersoonsAdres.Load();
                    }

                }
            }

            return resultaat;
        }
    }
}
