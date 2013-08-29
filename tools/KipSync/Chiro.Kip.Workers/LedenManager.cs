/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
﻿using System.Diagnostics;
using System.Linq;
using System.Text;
﻿using Chiro.Kip.Data;
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
            int? adNieuweFinVer = null;

            var feedback = new StringBuilder();

            var teVerwijderen = lid.HeeftFunctie.Where(hf => !functies.Cast<int>().Contains(hf.Functie.id)).ToArray();

            foreach (var hf in teVerwijderen)
            {
                if (lid.Persoon != null)
                {
                    feedback.AppendLine(String.Format(
                        "Functie {4} verwijderen van ID{0} {1} {2} AD{3}",
                        lid.Persoon.GapID,
                        lid.Persoon.VoorNaam,
                        lid.Persoon.Naam,
                        lid.Persoon.AdNummer, hf.Functie.CODE));
                }
                if (hf.Functie.id == (int)FunctieEnum.FinancieelVerantwoordelijke)
                {
                    if (lid.Groep == null)
                    {
                        lid.GroepReference.Load();
                    }

                    // zoek andere financieel verantwoordelijke
                    // bij gebrek: neem contactpersoon
                    // en anders blijft deze het toch :-)

                    var andereFv = (from l in db.Lid.Include("HeeftFunctie")
                        where l.Groep.GroepID == lid.Groep.GroepID && l.werkjaar == lid.werkjaar && l.Persoon.AdNummer != lid.Persoon.AdNummer
                              &&
                              l.HeeftFunctie.Any(
                                  hft =>
                                      hft.Functie.id == (int) FunctieEnum.FinancieelVerantwoordelijke ||
                                      hft.Functie.id == (int) FunctieEnum.ContactPersoon)
                        select
                            new
                            {
                                AdNr = l.Persoon.AdNummer,
                                IsFv =
                                    l.HeeftFunctie.Any(
                                        hft => hft.Functie.id == (int) FunctieEnum.FinancieelVerantwoordelijke)
                            }
                        ).OrderByDescending(res => res.IsFv).FirstOrDefault();

                    adNieuweFinVer = andereFv == null ? null : (int?)andereFv.AdNr;
                };

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

                if (lid.Persoon != null)
                {
                    feedback.AppendLine(String.Format(
                        "Functie toegekend aan ID{0} {1} {2} AD{3}: {4}",
                        lid.Persoon.GapID,
                        lid.Persoon.VoorNaam,
                        lid.Persoon.Naam,
                        lid.Persoon.AdNummer,
                        functie.CODE));
                }

                // Als functie fin. ver. is, pas dan ook betaler in groepsrecord
                // aan.

                if (functieID == (int)FunctieEnum.FinancieelVerantwoordelijke)
                {
                    if (lid.Persoon == null)
                    {
                        lid.PersoonReference.Load();
                    }

                    Debug.Assert(lid.Persoon != null);  // een lid zonder persoon kan niet in Kipadmin, en we hebben net de persoon geladen.

                    adNieuweFinVer = lid.Persoon.AdNummer;
                }
            }

            if (adNieuweFinVer != null)
            {
                if (lid.Groep == null)
                {
                    lid.GroepReference.Load();
                }

                // FIXME (#555): oud-leidingsploegen! 

                var cg = lid.Groep as ChiroGroep;

                if (cg != null)
                {
                    // OH NEE, dat is geen foreign key :-(

                    cg.BET_ADNR = adNieuweFinVer;
                    cg.STEMPEL = DateTime.Now;
                }

                feedback.AppendLine(String.Format("'BET_ADNR' bijgewerkt: {0}", adNieuweFinVer));
            }
            return feedback.ToString();
        }

        /// <summary>
        /// Verandert het lidtype van <paramref name="lid"/>
        /// </summary>
        /// <param name="lid">Lid in Kipadmin</param>
        /// <param name="lidType">LidType in GAP</param>
        /// <remarks>Opgelet: in het kader hebben de medewerkers gewoon type 'leiding', terwijl
        /// er in Kipadmin een onderscheid is tussen leiding en kader.  Vandaar de hacky code.</remarks>
        public void LidTypeInstellen(Lid lid, LidTypeEnum lidType)
        {
            lid.SOORT = (lid.Groep is ChiroGroep) && (lid.Groep as ChiroGroep).IsGewestVerbond
                            ? "KA"
                            : lidType == LidTypeEnum.Kind ? "LI" : "LE";
        }
    }
}
