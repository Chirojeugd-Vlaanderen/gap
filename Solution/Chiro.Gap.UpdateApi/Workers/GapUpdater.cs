/*
 * Copyright 2014-2015 the GAP developers. See the NOTICE file at the 
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

using System;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.UpdateApi.Models;
using System.Text;
using System.Diagnostics;

namespace Chiro.Gap.UpdateApi.Workers
{
    public class GapUpdater : IGapUpdater
    {
        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Persoon> _personenRepo;
        private readonly IRepository<Lid> _ledenRepo;
        private readonly IRepository<CommunicatieVorm> _communicatieVormenRepo;
        private readonly IRepository<Deelnemer> _deelnemersRepo;
        private readonly IRepository<Abonnement> _abonnementenRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<PersoonsAdres> _persoonsAdressenRepo;
        private readonly IRepository<PersoonsVerzekering> _persoonsVerzekeringenRepo;
        private readonly IRepository<GebruikersRechtV2> _gebruikersRechtenRepo;

        public GapUpdater(IRepositoryProvider repositoryProvider)
        {
            _repositoryProvider = repositoryProvider;
            // De repositoryprovider maakt een context, en die is disposable. De context
            // wordt gedeeld door alle repositories die de repositoryprovider oplevert.
            // Als een repository wordt gedisposet, dan disposet die ook de context, als
            // dat nog niet gebeurd moest zijn.

            // Enfin. In praktijk wil dat zeggen dat de context nog ergens
            // gedisposet moet worden. In Dispose() lijkt een goed idee.

            _personenRepo = repositoryProvider.RepositoryGet<Persoon>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _ledenRepo = repositoryProvider.RepositoryGet<Lid>();
            _communicatieVormenRepo = repositoryProvider.RepositoryGet<CommunicatieVorm>();
            _deelnemersRepo = repositoryProvider.RepositoryGet<Deelnemer>();
            _abonnementenRepo = repositoryProvider.RepositoryGet<Abonnement>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _persoonsAdressenRepo = repositoryProvider.RepositoryGet<PersoonsAdres>();
            _persoonsVerzekeringenRepo = repositoryProvider.RepositoryGet<PersoonsVerzekering>();
            _gebruikersRechtenRepo = repositoryProvider.RepositoryGet<GebruikersRechtV2>();
        }

        #region Disposable etc

        // Ik heb die constructie met 'disposed' en 'disposing' nooit begrepen.
        // Maar ze zeggen dat dat zo moet :-)

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    _repositoryProvider.Dispose();

                }
                disposed = true;
            }
        }

        ~GapUpdater()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Stelt het AD-nummer van de persoon met Id <paramref name="persoonId"/> in. 
        /// Als er al een persoon bestaat met hetzelfde AD-nummer, dan wordt die persoon
        /// gemerged met de bestaande persoon.
        /// Als <paramref name="adNummer"/> <c>null</c> is, dan wordt het AD-nummer van
        /// de persoon gewoon verwijderd.
        /// </summary>
        /// <param name="persoonId">
        /// Id van de persoon
        /// </param>
        /// <param name="adNummer">
        /// Nieuw AD-nummer
        /// </param>
        public void AdNummerToekennen(int persoonId, int? adNummer)
        {
            var persoon = _personenRepo.ByID(persoonId);
            if (persoon == null)
            {
                throw new FoutNummerException(
                    FoutNummer.PersoonNietGevonden,
                    string.Format("Onbekend persoon (ID {0}) voor AD-nummer {1} genegeerd.", persoonId, adNummer));
            }

            AdNummerToekennen(persoon, adNummer); // PERSISTEERT

            Console.WriteLine("Ad-nummer {0} toegekend aan {1}. (ID {2})", adNummer, persoon.VolledigeNaam, persoon.ID);
        }

        /// <summary>
        /// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
        /// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
        /// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
        /// </summary>
        /// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
        /// <param name="nieuwAd">Nieuw AD-nummer</param>
        public void AdNummerVervangen(int oudAd, int nieuwAd)
        {
            var personen = (from g in _personenRepo.Select() where g.AdNummer == oudAd select g);
            foreach (var p in personen)
            {
                AdNummerToekennen(p, nieuwAd); // PERSISTEERT!
                Console.WriteLine("Ad-nummer {0} vervangen door {1}. ({2}, ID {3})", oudAd, nieuwAd, p.VolledigeNaam, p.ID);
            }
        }

        /// <summary>
        /// Kent gegeven <paramref name="adNummer"/> toe aan de gegeven
        /// <paramref name="persoon"/>. Als er al iemand bestaat met hetzelfde
        /// AD-nr, dan worden de personen gemerged.
        /// Als <paramref name="adNummer"/> <c>null</c> is, dan wordt het gewoon
        /// verwijderd.
        /// </summary>
        /// <param name="persoon"></param>
        /// <param name="adNummer"></param>
        /// <remarks>PERSISTEERT!</remarks>
        private void AdNummerToekennen(Persoon persoon, int? adNummer)
        {
            // Voor een niet-null AD-nummer, moeten we mogelijk mergen.

            if (adNummer.HasValue)
            {
                // Wie heeft het gegeven AD-nummer al?
                var personenAlBestaand = (from g in _personenRepo.Select() where g.AdNummer == adNummer select g);

                foreach (var p in personenAlBestaand.Where(prs => prs.ID != persoon.ID).ToList())
                {
                    DubbelVerwijderen(persoon, p);
                }
            }

            persoon.AdNummer = adNummer;
            _personenRepo.SaveChanges();

        }

        /// <summary>
        /// Merget personen <paramref name="origineel"/> en <paramref name="dubbel"/>.
        /// </summary>
        /// <param name="origineel"></param>
        /// <param name="dubbel"></param>
        /// <remarks>Persisteert enkel om #1693 te voorkomen. Wat erg lelijk is. Maar
        /// ik kan er voorlopig niet aan doen.</remarks>
        public void DubbelVerwijderen(Persoon origineel, Persoon dubbel)
        {
            // TODO: Dit kan nog wel wat unit tests gebruiken...

            // Voor de groepen die niet zowel origineel als dubbel bevatten, verleggen we
            // het gelieerde-persoonobject van dubbel naar origineel

            var teVerleggenGPs = (from gp in dubbel.GelieerdePersoon
                where !origineel.GelieerdePersoon.Any(gp2 => Equals(gp2.Groep, gp.Groep))
                select gp).ToList();

            foreach (var gp in teVerleggenGPs)
            {
                dubbel.GelieerdePersoon.Remove(gp);
                gp.Persoon = origineel;
                    origineel.GelieerdePersoon.Add(gp);
            }

            // De gelieerde personen die nu nog aan dubbel hangen, moeten weg. We zetten zo veel 
            // mogelijk relevante informatie over naar de originele gelieerde personen.

            // Om straks problemen te vermijden, verwijderen we eerst de inactieve leden van de
            // te behouden persoon, waarvoor de te verwijderen persoon een actief lid heeft in 
            // hetzelfde werkjaar. (Zie #1693)

            var teVerwijderenLeden = (from l in origineel.GelieerdePersoon.SelectMany(gp => gp.Lid)
                where
                    l.NonActief &&
                    l.GroepsWerkJaar.Lid.Any(l2 => Equals(l2.GelieerdePersoon.Persoon, dubbel) && !l2.NonActief)
                select l).ToList();

            if (teVerwijderenLeden.Any())
            {
                foreach (var tv in teVerwijderenLeden)
                {
                    LidVerwijderen(tv);
                }
                // ik wou dat ik onderstaande savechanges kon vermijden, maar als ik dat niet doe
                // dan herkoppelt entity framework straks een lid alvorens de verwijdering uit te voeren,
                // met een key exception tot gevolg. (zie #1693)

                _ledenRepo.SaveChanges();
            }

            foreach (var dubbeleGp in dubbel.GelieerdePersoon.ToList())
            {
                var origineleGp = (from gp in origineel.GelieerdePersoon
                                   where Equals(gp.Groep, dubbeleGp.Groep)
                                   select gp).Single();

                foreach (var dubbelLid in dubbeleGp.Lid.ToList())
                {
                    var origineelLid = (from l in origineleGp.Lid
                                        where Equals(l.GroepsWerkJaar, dubbelLid.GroepsWerkJaar)
                                        select l).SingleOrDefault();

                    if (origineelLid != null)
                    {
                        // Zowel originele als dubbele gelieerde persoon waren lid. We behouden
                        // het originele lidobject.

                        LidVerwijderen(dubbelLid);
                        // eventuele functies en afdelingen van het dubbel lid worden
                        // zonder boe of ba weggegooid.
                    }
                    else
                    {
                        dubbelLid.GelieerdePersoon = origineleGp;
                        origineleGp.Lid.Add(dubbelLid);
                    }
                }

                foreach (var dubbeleCommunicatie in dubbeleGp.Communicatie.ToList())
                {
                    var origineleCommunicatie = (from c in origineleGp.Communicatie
                                                 where
                                                     Equals(c.CommunicatieType, dubbeleCommunicatie.CommunicatieType) &&
                                                     string.Compare(c.Nummer, dubbeleCommunicatie.Nummer,
                                                                    StringComparison.OrdinalIgnoreCase) == 0
                                                 select c).FirstOrDefault();
                    if (origineleCommunicatie != null)
                    {
                        // Als zowel origineel als dubbel de communicatievorm hebben, dan
                        // verwijderen we de dubbele.
                        _communicatieVormenRepo.Delete(dubbeleCommunicatie);
                    }
                    else
                    {
                        // Anders kennen we de dubbele toe aan de originele.
                        // TODO: problemen met meerdere voorkeuren fixen
                        dubbeleCommunicatie.GelieerdePersoon = origineleGp;
                    }
                }

                foreach (var dubbeleCategorie in dubbeleGp.Categorie.ToList())
                {
                    var origineleCategorie = (from c in origineleGp.Categorie
                                              where
                                                  Equals(c, dubbeleCategorie)
                                              select c).FirstOrDefault();
                    if (origineleCategorie == null)
                    {
                        // Als de originele niet in de categorie zit, fixen
                        // we dat hier.
                        dubbeleCategorie.GelieerdePersoon.Add(origineleGp);
                    }
                    dubbeleCategorie.GelieerdePersoon.Remove(dubbeleGp);
                }

                foreach (var dubbeleDeelnemer in dubbeleGp.Deelnemer.ToList())
                {
                    var origineleDeelnemer = (from d in origineleGp.Deelnemer
                                              where Equals(d.Uitstap, dubbeleDeelnemer.Uitstap)
                                              select d).FirstOrDefault();

                    if (origineleDeelnemer != null)
                    {
                        _deelnemersRepo.Delete(dubbeleDeelnemer);
                    }
                    else
                    {
                        dubbeleDeelnemer.GelieerdePersoon = origineleGp;
                    }
                }

                foreach (var dubbelAbonnement in dubbeleGp.Abonnement.ToList())
                {
                    // Dubbelpuntabonnementen lopen niet meer via het GAP. Maar omdat die er nog inzitten van
                    // vroeger, moeten we ze wel verleggen.

                    var origineelAbonnement = (from d in origineleGp.Abonnement
                                               where Equals(d.Publicatie, dubbelAbonnement.Publicatie)
                                               select d).FirstOrDefault();

                    if (origineelAbonnement != null)
                    {
                        _abonnementenRepo.Delete(origineelAbonnement);
                    }
                    else
                    {
                        dubbelAbonnement.GelieerdePersoon = origineleGp;
                    }
                }

                _gelieerdePersonenRepo.Delete(dubbeleGp);
            }

            // Verleg persoonsAdressen waar mogelijk

            var teVerleggenPAs = (from pa in dubbel.PersoonsAdres
                                  where !pa.Adres.PersoonsAdres.Any(pa2 => Equals(pa2.Persoon, origineel))
                                  select pa).ToList();

            foreach (var pa in teVerleggenPAs.ToList())
            {
                dubbel.PersoonsAdres.Remove(pa);
                pa.Persoon = origineel;
            }

            // De persoonsadressen die nu nog aan de dubbele hangen, hangen ook aan het origineel.
            // Verwijder.

            foreach (var dubbelPa in dubbel.PersoonsAdres.ToList())
            {
                if (dubbelPa.GelieerdePersoon.Any())
                {
                    // Oeps. Dit is nog ergens een voorkeursadres. verleg.
                    var origineelPa = (from pa in origineel.PersoonsAdres
                                       where Equals(pa.Adres, dubbelPa.Adres)
                                       select pa).Single();
                    foreach (var gp in dubbelPa.GelieerdePersoon.ToList())
                    {
                        gp.PersoonsAdres = origineelPa;
                    }
                }
                _persoonsAdressenRepo.Delete(dubbelPa);
            }

            // Verleg verzekeringen waar mogelijk

            var teVerleggenPvs = (from pv in dubbel.PersoonsVerzekering
                                  where
                                      !pv.VerzekeringsType.PersoonsVerzekering.Any(pv2 => Equals(pv2.Persoon, origineel))
                                  select pv).ToList();

            foreach (var pv in teVerleggenPvs.ToList())
            {
                dubbel.PersoonsVerzekering.Remove(pv);
                pv.Persoon = origineel;
            }

            foreach (var pv in dubbel.PersoonsVerzekering.ToList())
            {
                // Nog niet verlegde verzekeringen zijn dubbel, en mogen verwijderd worden
                _persoonsVerzekeringenRepo.Delete(pv);
            }

            // Gebruikersrechten nog

            var teVerleggenGebruikersrechten = (from gr in dubbel.GebruikersRechtV2
                                   where !gr.Groep.GebruikersRechtV2.Any(gr2 => Equals(gr2.Persoon, origineel))
                                   select gr).ToList();

            foreach (var g in teVerleggenGebruikersrechten.ToList())
            {
                dubbel.GebruikersRechtV2.Remove(g);
                origineel.GebruikersRechtV2.Add(g);
            }

            foreach (var dubbelGebruikersRecht in dubbel.GebruikersRechtV2.ToList())
            {
                // De dubbele gebruikersrechten zullen verwijderd worden, maar eerst moeten we eens
                // nakijken of die dubbele niet later vervallen als wat we behouden. Zo ja, moeten
                // we de datum bijwerken.
                // Iets gelijkaardigs geldt voor de rollen.

                var origineelGebruikersRecht = (from gr in origineel.GebruikersRechtV2
                                                where Equals(gr.Groep, dubbelGebruikersRecht.Groep)
                                                select gr).SingleOrDefault();

                Debug.Assert(origineelGebruikersRecht != null);

                if (dubbelGebruikersRecht.VervalDatum > origineelGebruikersRecht.VervalDatum)
                {
                    origineelGebruikersRecht.VervalDatum = dubbelGebruikersRecht.VervalDatum;
                }
                origineelGebruikersRecht.PersoonsPermissies |= dubbelGebruikersRecht.PersoonsPermissies;
                origineelGebruikersRecht.GroepsPermissies |= dubbelGebruikersRecht.GroepsPermissies;
                origineelGebruikersRecht.AfdelingsPermissies |= dubbelGebruikersRecht.AfdelingsPermissies;
                origineelGebruikersRecht.IedereenPermissies |= dubbelGebruikersRecht.IedereenPermissies;
                _gebruikersRechtenRepo.Delete(dubbelGebruikersRecht);
            }

            foreach (var b in dubbel.BerichtGebruiker)
            {
                b.Gebruiker = null;
            }

            foreach (var b in dubbel.BerichtPersoon)
            {
                b.Persoon = null;
            }

            _personenRepo.Delete(dubbel);
        }

        /// <summary>
        /// Verwijdert het geven <paramref name="lid"/>, ZONDER TE PERSISTEREN
        /// </summary>
        /// <param name="lid"></param>
        private void LidVerwijderen(Lid lid)
        {
            var leiding = lid as Leiding;
            if (leiding != null)
            {
                leiding.AfdelingsJaar.Clear();
            }
            lid.Functie.Clear();
            lid.GelieerdePersoon.Lid.Remove(lid);
            lid.GroepsWerkJaar.Lid.Remove(lid);
            _ledenRepo.Delete(lid);
        }

        /// <summary>
        /// Markeert een groep in GAP als gestopt. Of als terug actief.
        /// </summary>
        /// <param name="stamNr">Stamnummer te stoppen groep</param>
        /// <param name="stopDatum">Datum vanaf wanneer gestopt, <c>null</c> om de groep opnieuw te activeren.</param>
        /// <remarks>Als <paramref name="stopDatum"/> <c>null</c> is, wordt de groep opnieuw actief.</remarks>
        public void GroepDesactiveren(string stamNr, DateTime? stopDatum)
        {
            var groep = (from g in _groepenRepo.Select()
                         where string.Compare(g.Code, stamNr, StringComparison.OrdinalIgnoreCase) == 0
                         select g).FirstOrDefault();

            if (groep != null)
            {
                groep.StopDatum = stopDatum;
                _groepenRepo.SaveChanges();
                Console.WriteLine(stopDatum == null ? "Groep opnieuw geactiveerd: {0}" : "Groep gedesactiveerd: {0}", stamNr);
            }
            else
            {
                throw new FoutNummerException(
                    FoutNummer.GroepNietGevonden,
                    string.Format("Kan groep met stamnummer {0} niet vinden. Stopdatum niet aangepast.", stamNr));
            }
        }

        /// <summary>
        /// Verwijdert het ad-nummer van de persoon met gegeven <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer dat verwijderd moet worden</param>
        /// <returns>PersoonId van de persoon met het verwijderde AD-nummer.</returns>
        public int AdNummerVerwijderen(int adNummer)
        {
            int persoonId = 0;
            var personen = (from p in _personenRepo.Select() where p.AdNummer == adNummer select p).ToArray();

            if (!personen.Any())
            {
                throw new FoutNummerException(
                    FoutNummer.PersoonNietGevonden,
                    string.Format("Kan AD-nummer {0} niet verwijderen; persoon niet gevonden.", adNummer));                
            }

            // In principe is er hoogstens 1 persoon met gegeven AD-nummer.
            // Maar voor de zekerheid overlopen we alle resultaten.
            foreach (var p in personen)
            {
                p.AdNummer = null;
                persoonId = p.ID;
                _personenRepo.SaveChanges();
                Console.WriteLine("Ad-nummer {0} van {1} verwijderd. (ID {2})", adNummer, p.VolledigeNaam, p.ID);
            }
            return persoonId;
        }

        /// <summary>
        /// Werkt een persoon bij op basis van gegevens in <paramref name="model"/>.
        /// Het PersoonID bepaalt welke persoon bijgwerkt moet worden.
        /// </summary>
        /// <param name="model">Gegevens bij te werken persoon.</param>
        public void Bijwerken(PersoonModel model)
        {
            var persoon = _personenRepo.ByID(model.PersoonId);

            if (persoon == null)
            {
                throw new FoutNummerException(
                    FoutNummer.PersoonNietGevonden,
                    string.Format("Onbekend persoon (ID {0}) genegeerd (AD {1})", model.PersoonId, model.AdNummer));
            }

            if (persoon.AdNummer != model.AdNummer)
            {
                // AD-nummer aanpassen enkel indien nodig.
                // Onderstaande method persisteert.
                AdNummerToekennen(persoon, model.AdNummer);

                Console.WriteLine("Ad-nummer {0} toegekend aan {1}. (ID {2})", model.AdNummer, persoon.VolledigeNaam, persoon.ID);
            }
        }

        /// <summary>
        /// Werkt het werkjaar van laatste aansluiting bij van de persoon met
        /// AD-nummer gegeven in het model.
        /// </summary>
        /// <param name="model">Gegevens over bij te werken werkjaar.</param>
        public void Bijwerken(AansluitingModel model)
        {
            var persoon = _personenRepo.Select().FirstOrDefault(p => p.AdNummer == model.AdNummer);

            if (persoon == null)
            {
                throw new FoutNummerException(
                    FoutNummer.PersoonNietGevonden,
                    string.Format("Onbekend persoon (AD {0}) genegeerd.", model.AdNummer));
            }

            if (persoon.LaatsteMembership == model.RecentsteWerkJaar) return;

            persoon.LaatsteMembership = model.RecentsteWerkJaar;
            _personenRepo.SaveChanges();
            Console.WriteLine("LaatsteMembership {0} toegekend aan {1}. (ID {2})", model.RecentsteWerkJaar, persoon.VolledigeNaam, persoon.ID);
        }

        /// <summary>
        /// Levert een lijst op van alle stamnummer-adnummer-combinaties van het huidige
        /// werkjaar. Zal gebruikt worden voor monitoring. (#4326, #4268)
        /// </summary>
        /// <returns>Lijst van alle stamnummer-adnummer-combinaties van het huidige
        /// werkjaar.</returns>
        /// <remarks>
        /// Deze functie hoort niet echt thuis in iets dat 'GapUpdater' heet. Misschien
        /// is dit eerder een GapWorker. Of misschien moet deze klasse opgesplitst worden.
        /// </remarks>
        public string AlleLedenRaw(int werkjaar)
        {
            var alles = _ledenRepo.Select().Where(ld => ld.GroepsWerkJaar.WerkJaar == werkjaar && !ld.NonActief && ld.GroepsWerkJaar.Groep.StopDatum == null).OrderBy(l => l.GroepsWerkJaar.Groep.Code).ThenBy(l => l.GelieerdePersoon.Persoon.AdNummer).Select(l => new { StamNr = l.GroepsWerkJaar.Groep.Code, AdNr = l.GelieerdePersoon.Persoon.AdNummer });
            var builder = new StringBuilder();
            
            foreach (var l in alles)
            {
                builder.AppendLine(String.Format("{0};{1}", l.StamNr.Trim(), l.AdNr));
            }

            return builder.ToString();
        }
    }
}