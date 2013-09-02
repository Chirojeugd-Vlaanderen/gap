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
using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

using Adres = Chiro.Gap.Poco.Model.Adres;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. adressen bevat
    /// </summary>
    public class AdressenManager : IAdressenManager
    {
        /// <summary>
        /// Maakt een nieuw adres op basis van de info in <paramref name="adresInfo"/>, en persisteert
        /// </summary>
        /// <param name="adresInfo">
        /// Gegevens voor het nieuwe adres
        /// </param>
        /// <param name="straatNamen">beschikbare straatnamen als queryable</param>
        /// <param name="woonPlaatsen">beschikbare woonplaatsen als queryable</param>
        /// <param name="landen">beschikbare landen als queryable</param>
        /// <returns>
        /// Het nieuw gemaakte adres
        /// </returns>
        private Adres Maken(AdresInfo adresInfo, IQueryable<StraatNaam> straatNamen,
                            IQueryable<WoonPlaats> woonPlaatsen, IQueryable<Land> landen)
        {
            var problemen = new Dictionary<string, FoutBericht>();

            // Al maar preventief een collectie fouten verzamelen.  Als daar uiteindelijk
            // geen foutberichten in zitten, dan is er geen probleem.  Anders
            // creëer ik een exception.
            // FIXME: de manier waarop de problemen worden doorgegeven, is niet erg proper.
            // Kan dat niet eleganter?

            if (adresInfo.StraatNaamNaam == string.Empty)
            {
                problemen.Add("StraatNaamNaam",
                              new FoutBericht
                              {
                                  FoutNummer = FoutNummer.StraatOntbreekt,
                                  Bericht = string.Format(
                                      Resources.StraatOntbreekt,
                                      adresInfo.StraatNaamNaam,
                                      adresInfo.PostNr)
                              });
            }

            // Controle formaat postnummer enkel voor Belgische adressen.
            if ((string.IsNullOrEmpty(adresInfo.LandNaam) ||
                 String.Compare(adresInfo.LandNaam, Resources.Belgie, StringComparison.OrdinalIgnoreCase) == 0) &&
                (adresInfo.PostNr < 1000 || adresInfo.PostNr > 9999))
            {
                problemen.Add("PostNr",
                              new FoutBericht
                              {
                                  FoutNummer = FoutNummer.OngeldigPostNummer,
                                  Bericht = string.Format(
                                      Resources.OngeldigPostNummer,
                                      adresInfo.StraatNaamNaam,
                                      adresInfo.PostNr)
                              });
            }

            if (adresInfo.WoonPlaatsNaam == string.Empty)
            {
                problemen.Add("WoonPlaatsNaam",
                              new FoutBericht
                              {
                                  FoutNummer = FoutNummer.WoonPlaatsOntbreekt,
                                  Bericht = string.Format(
                                      Resources.WoonPlaatsOntbreekt,
                                      adresInfo.StraatNaamNaam,
                                      adresInfo.PostNr)
                              });
            }

            // Als er hier al fouten zijn: gewoon throwen.  Me hiel 't stad, mor ni me maa!
            if (problemen.Count != 0)
            {
                throw new OngeldigObjectException(problemen);
            }


            Adres adr;

            if (string.IsNullOrEmpty(adresInfo.LandNaam) ||
                System.String.Compare(adresInfo.LandNaam, Resources.Belgie, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Belgisch adres.  Zoek en koppel straat en gemeente
                adr = new BelgischAdres();

                var s = (from strt in straatNamen
                         where
                             String.Compare(adresInfo.StraatNaamNaam, strt.Naam,
                                            StringComparison.OrdinalIgnoreCase) == 0 &&
                             adresInfo.PostNr == strt.PostNummer
                         select strt).FirstOrDefault();

                if (s != null)
                {
                    // Straat gevonden: aan adres koppelen
                    ((BelgischAdres)adr).StraatNaam = s;
                    s.BelgischAdres.Add((BelgischAdres)adr);
                }
                else
                {
                    // Straat niet gevonden: foutbericht toevoegen
                    problemen.Add("StraatNaamNaam",
                                  new FoutBericht
                                      {
                                          FoutNummer = FoutNummer.StraatNietGevonden,
                                          Bericht = string.Format(
                                              Resources.StraatNietGevonden,
                                              adresInfo.StraatNaamNaam,
                                              adresInfo.PostNr)
                                      });
                }

                var sg = (from wpl in woonPlaatsen
                          where
                              String.Compare(adresInfo.WoonPlaatsNaam, wpl.Naam,
                                             StringComparison.OrdinalIgnoreCase) == 0 &&
                              adresInfo.PostNr == wpl.PostNummer
                          select wpl).FirstOrDefault();

                if (sg != null)
                {
                    // Gemeente gevonden: aan adres koppelen
                    ((BelgischAdres)adr).WoonPlaats = sg;
                    sg.BelgischAdres.Add((BelgischAdres)adr);
                }
                else
                {
                    // Gemeente niet gevonden: foutbericht toevoegen
                    problemen.Add("WoonPlaatsNaam",
                                  new FoutBericht
                                      {
                                          FoutNummer = FoutNummer.WoonPlaatsNietGevonden,
                                          Bericht = Resources.GemeenteNietGevonden
                                      });
                }
            }
            else
            {
                // Buitenlands adres.  Straat en gemeente zijn gewone strings.
                // Zoek en koppel land.
                adr = new BuitenLandsAdres();

                ((BuitenLandsAdres)adr).Straat = adresInfo.StraatNaamNaam;
                ((BuitenLandsAdres)adr).WoonPlaats = adresInfo.WoonPlaatsNaam;
                ((BuitenLandsAdres)adr).PostCode = adresInfo.PostCode;
                ((BuitenLandsAdres)adr).PostNummer = adresInfo.PostNr;

                Land l = (from lnd in landen
                          where String.Compare(lnd.Naam, adresInfo.LandNaam, StringComparison.OrdinalIgnoreCase) == 0
                          select lnd).FirstOrDefault();

                if (l != null)
                {
                    // Gemeente gevonden: aan adres koppelen
                    ((BuitenLandsAdres)adr).Land = l;
                    l.BuitenLandsAdres.Add((BuitenLandsAdres)adr);
                }
                else
                {
                    // Gemeente niet gevonden: foutbericht toevoegen
                    problemen.Add("LandNaam",
                                  new FoutBericht
                                      {
                                          FoutNummer = FoutNummer.LandNietGevonden,
                                          Bericht = Resources.LandNietGevonden
                                      });
                }
            }

            if (problemen.Count != 0)
            {
                throw new OngeldigObjectException(problemen);
            }

            adr.HuisNr = adresInfo.HuisNr;
            adr.Bus = adresInfo.Bus;

            // bewaren brengt Versie en ID automatisch in orde.
            return adr;
        }

        /// <summary>
        /// Zoekt adres op, op basis van de parameters.
        /// Als er zo geen adres bestaat, wordt het aangemaakt, op
        /// voorwaarde dat de straat en subgemeente geidentificeerd
        /// kunnen worden.  Als ook dat laatste niet het geval is,
        /// wordt een exception gethrowd.
        /// </summary>
        /// <param name="adresInfo">
        /// Bevat de gegevens van het te zoeken/maken adres
        /// </param>
        /// <param name="adressen">
        /// Lijst met bestaande adressen om na te kijken of het nieuwe adres al bestaat
        /// </param>
        /// <param name="straatNamen">queryable voor alle beschikbare straatnamen</param>
        /// <param name="woonPlaatsen">queryable voor alle beschikbare woonplaatsen</param>
        /// <param name="landen">queryable voor alle beschikbare landen</param>
        /// <returns>
        /// Gevonden adres
        /// </returns>
        /// <remarks>
        /// Ieder heeft het recht adressen op te zoeken
        /// </remarks>
        public Adres ZoekenOfMaken(AdresInfo adresInfo, IQueryable<Adres> adressen, IQueryable<StraatNaam> straatNamen,
                            IQueryable<WoonPlaats> woonPlaatsen, IQueryable<Land> landen)
        {
            // In volgorde: Belgisch adres, buitenlands adres, nieuw adres

            return (from adr in adressen.OfType<BelgischAdres>()
                    where adr.StraatNaam.Naam == adresInfo.StraatNaamNaam
                          && adr.StraatNaam.PostNummer == adresInfo.PostNr
                          && adr.HuisNr == adresInfo.HuisNr 
                          && (adr.Bus ?? String.Empty) == (adresInfo.Bus ?? String.Empty)
                          // (de truuk met het vraagteken converteert null naar empty voor vergelijken)
                    select adr).FirstOrDefault() ?? ((from adr in adressen.OfType<BuitenLandsAdres>()
                                                      where adr.Straat == adresInfo.StraatNaamNaam
                                                            && adr.PostNummer == adresInfo.PostNr
                                                            && adr.HuisNr == adresInfo.HuisNr
                                                            && (adr.Bus ?? String.Empty) == (adresInfo.Bus ?? String.Empty)
                                                            && adr.Land.Naam == adresInfo.LandNaam
                                                            && adr.PostCode == adresInfo.PostCode
                                                      select adr).FirstOrDefault() ??
                                                     Maken(adresInfo, straatNamen, woonPlaatsen, landen));


        }
    }
}