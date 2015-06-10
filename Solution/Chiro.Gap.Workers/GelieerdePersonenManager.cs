/*
 * Copyright 2008-2013, 2015 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. gelieerde personen bevat
    /// </summary>
    public class GelieerdePersonenManager : IGelieerdePersonenManager
    {
        /// <summary>
        /// Voegt een <paramref name="nieuwePersoon"/> toe aan de gegegeven <paramref name="groep"/>. Als
        /// <paramref name="forceer"/> niet is gezet, wordt een exception opgegooid als er al een gelijkaardige
        /// persoon aan de groep gekoppeld is.
        /// </summary>
        /// <param name="nieuwePersoon">Nieuwe toe te voegen persoon</param>
        /// <param name="groep">Groep waaraan de persoon gelieerd/gekoppeld moet worden</param>
        /// <param name="chiroLeeftijd">Chiroleeftijd van de persoon</param>
        /// <param name="forceer">Als <c>false</c>, dan wordt een exception opgegooid als er al een gelijkaardige
        /// persoon aan de groep gekoppeld is.</param>
        /// <returns>De gelieerde persoon na het koppelen van <paramref name="nieuwePersoon"/> aan <paramref name="groep"/>.</returns>
        public GelieerdePersoon Toevoegen(Persoon nieuwePersoon, Groep groep, int chiroLeeftijd, bool forceer)
        {
            if (!forceer)
            {
                var bestaande = ZoekGelijkaardig(nieuwePersoon, groep);
                if (bestaande != null && bestaande.Any())
                {
                    throw new BlokkerendeObjectenException<GelieerdePersoon>(bestaande, bestaande.Count,
                                                                             Resources.GelijkaardigePersoon);
                }
            }

            var resultaat = new GelieerdePersoon {Persoon = nieuwePersoon, Groep = groep, ChiroLeefTijd = chiroLeeftijd};

            nieuwePersoon.GelieerdePersoon.Add(resultaat);
            groep.GelieerdePersoon.Add(resultaat);

            return resultaat;
        }

        /// <summary>
        /// Levert het contact-e-mailadres op van een gelieerde persoon
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvan contact-e-mailadres opgeleverd moet worden</param>
        /// <returns>Het betreffende e-mailadres</returns>
        /// <remarks>Als er geen e-mailadres de voorkeur heeft, wordt gewoon een willekeurig opgeleverd. Of 
        /// <c>null</c>, als er geen e-mailadressen zijn</remarks>
        public string ContactEmail(GelieerdePersoon gelieerdePersoon)
        {
            return (from cv in gelieerdePersoon.Communicatie
                    where cv.CommunicatieType.ID == (int) (CommunicatieTypeEnum.Email)
                    orderby cv.Voorkeur
                    select cv.Nummer).LastOrDefault();
        }

        /// <summary>
        /// Levert alle gelieerde personen op uit dezelfde persoon als <paramref name="gelieerdePersoon"/>, die minstens
        /// een adres gemeenschappelijk hebben met die <paramref name="gelieerdePersoon"/>.
        /// </summary>
        /// <param name="gelieerdePersoon">Eem gelieerde persoon</param>
        /// <returns>Alle gelieerde personen uit dezelfde persoon als <paramref name="gelieerdePersoon"/>, die minstens
        /// een adres gemeenschappelijk hebben met die <paramref name="gelieerdePersoon"/>.</returns>
        public GelieerdePersoon[] AdresGenotenUitZelfdeGroep(GelieerdePersoon gelieerdePersoon)
        {
            // We trekken het wat uit elkaar om makkelijker te kunnen debuggen.
            var persoonsAdressen = gelieerdePersoon.Persoon.PersoonsAdres.SelectMany(pa => pa.Adres.PersoonsAdres);
            var gelieerdePersonen = persoonsAdressen.SelectMany(pa => pa.Persoon.GelieerdePersoon);
            return gelieerdePersonen.Where(gp => Equals(gp.Groep, gelieerdePersoon.Groep)).ToArray();
        }

        /// <summary>
        /// Zoekt naar gelieerde personen die gelijkaardig zijn aan een gegeven
        /// <paramref name="persoon"/>.
        /// </summary>
        /// <param name="persoon">
        /// Persoon waarmee vergeleken moet worden
        /// </param>
        /// <param name="groep">
        /// groep waarin te zoeken
        /// </param>
        /// <returns>
        /// Lijstje met gelijkaardige personen
        /// </returns>
        public List<GelieerdePersoon> ZoekGelijkaardig(Persoon persoon, Groep groep)
        {
            var seNaam = ZoekHelper.Soundex(persoon.Naam);
            var seVoornaam = ZoekHelper.Soundex(persoon.VoorNaam);

            var result = from gp in groep.GelieerdePersoon
                         where
                             (seNaam == gp.Persoon.SeNaam && seVoornaam == gp.Persoon.SeVoornaam) ||
                             (seNaam == gp.Persoon.SeVoornaam && seVoornaam == gp.Persoon.SeNaam)
                         select gp;

            return result.ToList();
        }


        /// <summary>
        /// Koppelt het gegeven Adres via nieuwe PersoonsAdresObjecten
        /// aan de Personen gekoppeld aan de gelieerde personen <paramref name="gelieerdePersonen"/>.  
        /// Persisteert niet.
        /// </summary>
        /// <param name="gelieerdePersonen">
        ///     Gelieerde  die er een adres bij krijgen, met daaraan gekoppeld hun huidige
        ///     adressen, en de gelieerde personen waarop de gebruiker GAV-rechten heeft.
        /// </param>
        /// <param name="adres">
        ///     Toe te voegen adres
        /// </param>
        /// <param name="adrestype">
        ///     Het adrestype (thuis, kot, enz.)
        /// </param>
        /// <param name="voorkeur">
        ///     Indien true, wordt het nieuwe adres voorkeursadres van de gegeven gelieerde personen
        /// </param>
        /// <returns>
        /// De nieuwe koppelingen tussen de <paramref name="gelieerdePersonen"/> en 
        /// <paramref name="adres"/>.
        /// </returns>
        public List<PersoonsAdres> AdresToevoegen(IList<GelieerdePersoon> gelieerdePersonen, Adres adres, AdresTypeEnum adrestype, bool voorkeur)
        {
            var resultaat = new List<PersoonsAdres>();
            // Vind personen waaraan het adres al gekoppeld is.

            var bestaand =
                gelieerdePersonen.Select(gp => gp.Persoon).SelectMany(
                    p => p.PersoonsAdres.Where(pa => pa.Adres.ID == adres.ID)).ToList();

            if (bestaand.FirstOrDefault() != null)
            {
                // Sommige personen hebben het adres al.  Geef een exception met daarin de
                // betreffende persoonsadres-objecten.
                throw new BlokkerendeObjectenException<PersoonsAdres>(
                    bestaand,
                    bestaand.Count(),
                    Resources.WonenDaarAl);
            }

            // En dan nu het echte werk:
            foreach (var gelieerdePersoon in gelieerdePersonen)
            {
                // Maak PersoonsAdres dat het adres aan de persoon koppelt.
                var pa = new PersoonsAdres { Adres = adres, Persoon = gelieerdePersoon.Persoon, AdresType = adrestype };
                gelieerdePersoon.Persoon.PersoonsAdres.Add(pa);
                adres.PersoonsAdres.Add(pa);

                // Maak zo nodig voorkeursadres
                if (gelieerdePersoon.Persoon.PersoonsAdres.Count() == 1)
                {
                    // Eerste adres van de persoon.  Dit moet bij elke gelieerde persoon het voorkeursadres
                    // worden.

                    foreach (var gp2 in gelieerdePersoon.Persoon.GelieerdePersoon)
                    {
                        gp2.PersoonsAdres = pa;
                        pa.GelieerdePersoon.Add(gp2);
                    }
                }
                else if (voorkeur)
                {
                    gelieerdePersoon.PersoonsAdres = pa;
                    pa.GelieerdePersoon.Add(gelieerdePersoon);
                }
                resultaat.Add(pa);
            }
            return resultaat;
        }
    }
}