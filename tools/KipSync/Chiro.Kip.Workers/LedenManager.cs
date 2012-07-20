using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data.Entity;
using Chiro.Kip.Data;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.Workers
{
    public class LedenManager
    {
        /// <summary>
        /// Vervangt de afdelingen van <paramref name="lid"/> door de gegeven <paramref name="afdelingen"/>.
        /// </summary>
        /// <param name="lid">Lid met te vervangen afdelingen</param>
        /// <param name="afdelingen">Nieuwe afdelingen voor <paramref name="lid"/></param>
        /// <param name="db">Objectcontext</param>
        /// <remarks>Kipadmin ondersteunt op dit moment hoogstens 2 afdelingen per leid(st)er.  De rest wordt
        /// genegeerd.</remarks>
        public void AfdelingenZetten(Lid lid, AfdelingEnum[] afdelingen, kipadminEntities db)
        {
            if (afdelingen.Any())
            {
                int afdid = (int)afdelingen.First();
                lid.AFDELING1 = (from a in db.AfdelingSet
                                 where a.AFD_ID == afdid
                                 select a.AFD_NAAM).FirstOrDefault();
            }
            else
            {
                lid.AFDELING1 = null;
            }

            if (afdelingen.Count() >= 2)
            {
                int afdid = (int)afdelingen.Skip(1).First();
                lid.AFDELING2 = (from a in db.AfdelingSet
                                 where a.AFD_ID == afdid
                                 select a.AFD_NAAM).FirstOrDefault();
            }
            else
            {
                lid.AFDELING2 = null;
            }
        }

        /// <summary>
        /// Vervangt de huidige functies van <paramref name="lid"/> door de gegeven <paramref name="functies"/>
        /// </summary>
        /// <param name="lid">Lid met te vervangen functies</param>
        /// <param name="functies">Lijst met nieuwe functies voor het lid</param>
        /// <param name="db">Objectcontext</param>
        /// <returns>Een string met feedback. #ugly</returns>
        public string FunctiesVervangen(Lid lid, FunctieEnum[] functies, kipadminEntities db)
        {
            var feedback = new StringBuilder();

            var teVerwijderen = lid.HeeftFunctie.Where(hf => !functies.Cast<int>().Contains(hf.Functie.id));

            foreach (var hf in teVerwijderen)
            {
                feedback.AppendLine(String.Format(
                    "Functie {4} verwijderen van ID{0} {1} {2} AD{3}",
                    lid.Persoon.GapID,
                    lid.Persoon.VoorNaam,
                    lid.Persoon.Naam,
                    lid.Persoon.AdNummer, hf.Functie.CODE));
                db.DeleteObject(hf);
            }

            var overblijvend = (from hf in lid.HeeftFunctie
                                select hf.Functie.id);

            var toeTeKennenIDs = functies.Where(f => !overblijvend.Contains((int) f)).Select(f => (int) f);

            foreach (var functieID in toeTeKennenIDs)
            {
                var functie = (from f in db.FunctieSet
                               where f.id == functieID
                               select f).FirstOrDefault();

                Debug.Assert(functie != null);

                var hf = new HeeftFunctie
                {
                    Lid = lid,
                    Functie = functie
                };
                db.AddToHeeftFunctieSet(hf);

                feedback.AppendLine(String.Format(
                    "Functie toegekend aan ID{0} {1} {2} AD{3}: {4}",
                    lid.Persoon.GapID,
                    lid.Persoon.VoorNaam,
                    lid.Persoon.Naam,
                    lid.Persoon.AdNummer,
                    functie.CODE));

                // Als functie fin. ver. is, pas dan ook betaler in groepsrecord
                // aan.

                if (functieID == (int)FunctieEnum.FinancieelVerantwoordelijke)
                {
                    lid.GroepReference.Load();

                    // FIXME (#555): oud-leidingsploegen! 

                    var cg = lid.Groep as ChiroGroep;

                    if (cg != null)
                    {
                        // OH NEE, dat is geen foreign key :-(

                        cg.BET_ADNR = lid.Persoon.AdNummer;
                        cg.STEMPEL = DateTime.Now;
                    }

                    feedback.AppendLine("'BET_ADNR' bijgewerkt");
                }
            }
            return feedback.ToString();
        }
    }
}
