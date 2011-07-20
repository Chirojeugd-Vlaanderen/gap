// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Voorziet data access voor uitstappen
    /// </summary>
    public class UitstappenDao : Dao<Uitstap, ChiroGroepEntities>, IUitstappenDao
    {
        /// <summary>
        /// Haalt alle uitstappen van een gegeven groep op.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="inschrijvenMogelijk">Als dit <c>true</c> is, worden enkel de uitstappen van het
        /// huidige werkjaar van de groep opgehaald.</param>
        /// <returns>Details van uitstappen</returns>
        public IEnumerable<Uitstap> OphalenVanGroep(int groepID, bool inschrijvenMogelijk)
        {
            IEnumerable<Uitstap> resultaat;
            using (var db = new ChiroGroepEntities())
            {
                if (!inschrijvenMogelijk)
                {
                    // Alle uitstappen ophalen
                    resultaat = (from u in db.Uitstap
                                 where u.GroepsWerkJaar.Groep.ID == groepID
                                 select u).ToArray();
                }
                else
                {
                    // Enkel uitstappen van recentste groepswerkjaar
                    var groep = db.Groep.Include(g => g.GroepsWerkJaar).Where(grp => grp.ID == groepID).FirstOrDefault();
                    Debug.Assert(groep != null);
                    var groepsWerkJaar = groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();
                    Debug.Assert(groepsWerkJaar != null);
                    groepsWerkJaar.Uitstap.Load();
                    resultaat = groepsWerkJaar.Uitstap.ToArray();
                }
            }

            return Utility.DetachObjectGraph(resultaat);
        }

        /// <summary>
        /// Haalt de deelnemers (incl. lidgegevens van het betreffende groepswerkjaar)
        /// van de gegeven uitstap op.
        /// </summary>
        /// <param name="uitstapID">ID van uitstap waarvan deelnemers op te halen zijn</param>
        /// <returns>De deelnemers van de gevraagde uitstap.</returns>
        public IEnumerable<Deelnemer> DeelnemersOphalen(int uitstapID)
        {
            IEnumerable<Deelnemer> deelnemers;
            IEnumerable<Lid> leden;

            using (var db = new ChiroGroepEntities())
            {
                deelnemers = (from d in db.Deelnemer.Include(dn => dn.GelieerdePersoon.Persoon).Include(dn => dn.UitstapWaarvoorVerantwoordelijk)
                              where d.Uitstap.ID == uitstapID
                              select d).ToArray();

                var gelieerdePersoonIDs = (from d in deelnemers
                                           select d.GelieerdePersoon.ID);

                leden = db.Lid.Include(ld => ld.GelieerdePersoon)
                    .Where(ld => ld.GroepsWerkJaar.Uitstap.Any(u => u.ID == uitstapID))
                    .Where(Utility.BuildContainsExpression<Lid, int>(ld => ld.GelieerdePersoon.ID, gelieerdePersoonIDs)).ToArray();

                LedenDao.AfdelingenKoppelen(db, leden, false);
            }

            //// Here comes the tricky part.
            ////
            //// In eerste instantie had ik: Utility.DetachObjectGraph(deelnemers);
            //// maar in dat geval is deelnemer.GelieerdePersoon.Lid.First() telkens van het type Lid, en niet van 
            //// het type Kind of Leiding, wat we eigenlijk nodig hebben.
            //// Ik probeer daar rond te werken door de leden te detachen, en daarna de deelnemers te returnen

            // var gedetachteLeden = Utility.DetachObjectGraph(leden);
            // var resultaat = (from l in gedetachteLeden select l.GelieerdePersoon.Deelnemer.First()).ToArray();

            var resultaat = Utility.DetachObjectGraph(deelnemers);

            return resultaat;
        }

        /// <summary>
        /// Haalt alle bivakken (geen uitstappen) op van alle groepen, uit gegeven <paramref name="werkjaar"/>,
        /// inclusief bivakplaats (met adres), contactpersoon, groepswerkjaar (met groep)
        /// </summary>
        /// <param name="werkjaar">Werkjaar waarvan de bivakken opgehaald moeten worden.</param>
        /// <returns>Alle bivakken uit het <paramref name="werkjaar"/></returns>
        public IEnumerable<Uitstap> AlleBivakkenOphalen(int werkjaar)
        {
            IEnumerable<Uitstap> bivakken;
            using (var db = new ChiroGroepEntities())
            {
                bivakken =
                    (from u in
                         db.Uitstap.Include(b => b.ContactDeelnemer.GelieerdePersoon.Persoon).Include(
                             b => b.GroepsWerkJaar.Groep).Include(b => b.Plaats.Adres)
                     where u.IsBivak && u.GroepsWerkJaar.WerkJaar == werkjaar
                     select u).ToArray();

                var adressen = (from b in bivakken
                                where b.Plaats != null && b.Plaats.Adres != null
                                select b.Plaats.Adres);

                // Koppel relevante adresgegevens voor Belgische en buitenlandse adressen.

                foreach (var adr in adressen)
                {
                    AdresHelper.AdresGegevensKoppelen(adr);
                }
            }

            return Utility.DetachObjectGraph(bivakken);
        }

        /// <summary>
        /// Haalt een uitstap op
        /// </summary>
        /// <param name="id">ID op te halen uitstap</param>
        /// <param name="paths">Gekoppelde entiteiten</param>
        /// <returns>Opgehaalde uitstap</returns>
        /// <remarks>Als de plaats is gekoppeld, wordt het adres mee opgehaald als Belgisch
        /// of buitenlands adres.</remarks>
        public override Uitstap Ophalen(int id, params System.Linq.Expressions.Expression<Func<Uitstap, object>>[] paths)
        {
            Uitstap resultaat;

            using (var db = new ChiroGroepEntities())
            {
                // Haal uitstap op met gekoppelde objecten

                var query = (from a in db.Uitstap
                             where a.ID == id
                             select a) as ObjectQuery<Uitstap>;

                query = IncludesToepassen(query, paths);
                resultaat = query.FirstOrDefault();

                // Als er een adres is, koppel de relevante 'onderdelen'

                if (resultaat.Plaats != null && resultaat.Plaats.Adres != null)
                {
                    if (resultaat.Plaats.Adres is BelgischAdres)
                    {
                        ((BelgischAdres)resultaat.Plaats.Adres).StraatNaamReference.Load();
                        ((BelgischAdres)resultaat.Plaats.Adres).WoonPlaatsReference.Load();
                    }
                    else if (resultaat.Plaats.Adres is BuitenLandsAdres)
                    {
                        ((BuitenLandsAdres)resultaat.Plaats.Adres).LandReference.Load();
                    }
                }
            }

            return Utility.DetachObjectGraph(resultaat);
        }
    }
}
