/*
 * Copyright 2013 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Text;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;
using OfficeOpenXml;

namespace Chiro.Gap.ExcelManip
{
    /// <summary>
    /// GAP-specifieke Excelmanipulaties
    /// </summary>
    /// <remarks>
    /// Dit kan nog wat opkuis gebruiken. We zijn van iets custom aan het migreren naar EPPlus, en die migratie
    /// is van de rap-rap gebeurd.
    /// </remarks>
    public static class GapExcelManip
    {
        /// <summary>
        /// Genereer een Exceldocument op basis van een rij PersoonLidInfo-objecten.
        /// </summary>
        /// <param name="leden">Objecten die in een rij terecht moeten komen</param>
        /// <param name="alleAfdelingen">Lijstje van (minstens) alle gebruikte afdelingen</param>
        /// <returns>Een memorystream met daarin het Exceldocument</returns>
        public static ExcelPackage LidExcelDocument(IList<PersoonLidInfo> leden, IEnumerable<AfdelingInfo> alleAfdelingen)
        {
            // TODO: Needs cleanup.

            var pkg = new ExcelPackage();
            var aantallen = (from r in leden
                select new
                       {
                           AantalAdressen = r.PersoonsAdresInfo.Count(),
                           AantalEmail =
                               r.CommunicatieInfo.Count(
                                   ci => ci.CommunicatieTypeID == (int) CommunicatieTypeEnum.Email),
                           AantalTel =
                               r.CommunicatieInfo.Count(
                                   ci => ci.CommunicatieTypeID == (int) CommunicatieTypeEnum.TelefoonNummer)
                       }).ToList();

            int maxAantalAdressen = aantallen.Max(a => a.AantalAdressen);
            int maxAantalEmail = aantallen.Max(a => a.AantalEmail);
            int maxAantalTel = aantallen.Max(a => a.AantalTel);

            // In het eerste werkblad zit een grote tabel met erg veel informatie.
            // Opbouw is tamelijk omslachtig, maar daar heb ik niets beters voor.

            var ledenBlad = pkg.Workbook.Worksheets.Add("Ledenlijst");
            
            // Bouw koppen op
            
            Insert(ledenBlad, "Type", 1, 1);
            Insert(ledenBlad, "Civi-ID", 2, 1);
            Insert(ledenBlad, "Voornaam", 3, 1);
            Insert(ledenBlad, "Naam", 4, 1);
            Insert(ledenBlad, "Afdelingen", 5, 1);
            Insert(ledenBlad, "Functies", 6, 1);
            Insert(ledenBlad, "Geboortedatum", 7, 1);
            Insert(ledenBlad, "Betaald", 8, 1);

            for (int i = 0; i < maxAantalAdressen; ++i)
            {
                Insert(ledenBlad, String.Format("Straat {0}", i + 1), i*7 + 9, 1);
                Insert(ledenBlad, String.Format("Nr. {0}", i + 1), i*7 + 10, 1);
                Insert(ledenBlad, String.Format("Bus {0}", i + 1), i*7 + 11, 1);
                Insert(ledenBlad, String.Format("Postnr. {0}", i + 1), i*7 + 12, 1);
                Insert(ledenBlad, String.Format("Postcode {0}", i + 1), i*7 + 13, 1);
                Insert(ledenBlad, String.Format("Woonplaats {0}", i + 1), i*7 + 14, 1);
                Insert(ledenBlad, String.Format("Land {0}", i + 1), i*7 + 15, 1);
            }

            for (int i = 0; i < maxAantalTel; ++i)
            {
                Insert(ledenBlad, String.Format("Tel. {0}", i + 1), 7*maxAantalAdressen + i + 9, 1);
            }

            for (int i = 0; i < maxAantalEmail; ++i)
            {
                Insert(ledenBlad, String.Format("E-mail. {0}", i + 1),
                    7*maxAantalAdressen + maxAantalTel + i + 9, 1);
            }

            // de effectieve gegevens

            uint rijNr = 2;

            foreach (var rij in leden)
            {
                Insert(ledenBlad, rij.PersoonDetail.CiviID, 2, rijNr);
                Insert(ledenBlad, rij.PersoonDetail.VoorNaam, 3, rijNr);
                Insert(ledenBlad, rij.PersoonDetail.Naam, 4, rijNr);

                if (rij.LidInfo != null)
                {
                    Insert(ledenBlad, rij.LidInfo.Type.ToString(), 1, rijNr);
                    Insert(ledenBlad, GeconcateneerdeAfdelingen(rij.LidInfo.AfdelingIdLijst, alleAfdelingen), 5, rijNr);
                    if (rij.LidInfo.Functies != null)
                    {
                        Insert(ledenBlad, String.Concat(rij.LidInfo.Functies.Select(fn => fn.Code + " ")), 6,
                            rijNr);
                    }
                    Insert(ledenBlad, rij.LidInfo.LidgeldBetaald ? "Ja" : "Nee", 8, rijNr);
                }

                Insert(ledenBlad, rij.PersoonDetail.GeboorteDatum, 7, rijNr);

                int voorkeursAdresID = rij.PersoonDetail.VoorkeursAdresID ?? 0;
                int i = 0;

                // die order by hieronder is een nifty hack om het voorkeursadres eerst te krijgen.

                foreach (var adres in rij.PersoonsAdresInfo.OrderBy(pai => Math.Abs(pai.PersoonsAdresID - voorkeursAdresID)))
                {
                    Insert(ledenBlad, adres.StraatNaamNaam, i * 7 + 9, rijNr);
                    Insert(ledenBlad, adres.HuisNr, i * 7 + 10, rijNr);
                    Insert(ledenBlad, adres.Bus, i * 7 + 11, rijNr);
                    Insert(ledenBlad, adres.PostNr, i * 7 + 12, rijNr);
                    Insert(ledenBlad, adres.PostCode, i * 7 + 13, rijNr);
                    Insert(ledenBlad, adres.WoonPlaatsNaam, i * 7 + 14, rijNr);
                    Insert(ledenBlad, adres.LandNaam, i * 7 + 15, rijNr);
                    ++i;
                }

                i = 0;
                foreach (
                    var tel in
                        rij.CommunicatieInfo.Where(
                            ci => ci.CommunicatieTypeID == (int)CommunicatieTypeEnum.TelefoonNummer))
                {
                    string output;
                    if (String.IsNullOrEmpty(tel.Nota))
                    {
                        output = tel.Nummer;
                    }
                    else
                    {
                        output = String.Format("{0} ({1})", tel.Nummer, tel.Nota);
                    }
                    Insert(ledenBlad, output, 7 * maxAantalAdressen + i + 9, rijNr);
                    ++i;
                }

                i = 0;
                foreach (
                    var email in
                        rij.CommunicatieInfo.Where(
                            ci => ci.CommunicatieTypeID == (int)CommunicatieTypeEnum.Email))
                {
                    string output;
                    if (String.IsNullOrEmpty(email.Nota))
                    {
                        output = String.Format("{0} <{1}>", rij.PersoonDetail.VolledigeNaam, email.Nummer);
                    }
                    else
                    {
                        output = String.Format("{0} ({2}) <{1}>", rij.PersoonDetail.VolledigeNaam, email.Nummer,
                            email.Nota);
                    }
                    Insert(ledenBlad, output, 7 * maxAantalAdressen + maxAantalTel + i + 9, rijNr);
                    ++i;
                }
                ++rijNr;
            }

            // tweede werkblad: adressen

            var adressenBlad = pkg.Workbook.Worksheets.Add("Adressen");
            KolomTitelsInvullen(adressenBlad, new string[]
                                              {
                                                  "Type", "Civi-ID", "Voornaam", "Naam", "Afdelingen", "Straat", "Nr.",
                                                  "Bus", "Postnr.", "Postcode",
                                                  "Woonplaats", "Land", "Adrestype", "Voorkeur"
                                              });

            rijNr = 2;
            foreach (var lid in leden)
            {
                int voorkeursAdresID = lid.PersoonDetail.VoorkeursAdresID ?? 0;
                foreach (var adres in lid.PersoonsAdresInfo)
                {
                    if (lid.LidInfo != null)
                    {
                        Insert(adressenBlad, lid.LidInfo.Type.ToString(), 1, rijNr);
                        Insert(adressenBlad, GeconcateneerdeAfdelingen(lid.LidInfo.AfdelingIdLijst, alleAfdelingen), 5, rijNr);
                    }
                    Insert(adressenBlad, lid.PersoonDetail.CiviID, 2, rijNr);
                    Insert(adressenBlad, lid.PersoonDetail.VoorNaam, 3, rijNr);
                    Insert(adressenBlad, lid.PersoonDetail.Naam, 4, rijNr);
                    Insert(adressenBlad, adres.StraatNaamNaam, 6, rijNr);
                    Insert(adressenBlad, adres.HuisNr, 7, rijNr);
                    Insert(adressenBlad, adres.Bus, 8, rijNr);
                    Insert(adressenBlad, adres.PostNr, 9, rijNr);
                    Insert(adressenBlad, adres.PostCode, 10, rijNr);
                    Insert(adressenBlad, adres.WoonPlaatsNaam, 11, rijNr);
                    Insert(adressenBlad, adres.LandNaam, 12, rijNr);
                    Insert(adressenBlad, adres.AdresType.ToString(), 13, rijNr);
                    Insert(adressenBlad, adres.PersoonsAdresID == voorkeursAdresID, 14, rijNr);
                    ++rijNr;
                }
            }

            // derde werkblad: communicatievormen

            var communicatieBlad = pkg.Workbook.Worksheets.Add("Communicatie");
            KolomTitelsInvullen(communicatieBlad, new string[]
                                              {
                                                  "Type", "Civi-ID", "Voornaam", "Naam", "Afdelingen", "Type",
                                                  "Nr./adres", "Snelleberichtenlijst", "Opmerking"
                                              });
            rijNr = 2;
            foreach (var lid in leden)
            {
                foreach (var ci in lid.CommunicatieInfo)
                {
                    if (lid.LidInfo != null)
                    {
                        Insert(communicatieBlad, lid.LidInfo.Type.ToString(), 1, rijNr);
                        Insert(communicatieBlad, GeconcateneerdeAfdelingen(lid.LidInfo.AfdelingIdLijst, alleAfdelingen), 5, rijNr);
                    }
                    Insert(communicatieBlad, lid.PersoonDetail.CiviID, 2, rijNr);
                    Insert(communicatieBlad, lid.PersoonDetail.VoorNaam, 3, rijNr);
                    Insert(communicatieBlad, lid.PersoonDetail.Naam, 4, rijNr);
                    Insert(communicatieBlad, ci.CommunicatieTypeOmschrijving, 6, rijNr);
                    Insert(communicatieBlad, ci.Nummer, 7, rijNr);
                    Insert(communicatieBlad, ci.IsVoorOptIn, 8, rijNr);
                    Insert(communicatieBlad, ci.Nota, 9, rijNr);
                    ++rijNr;
                }
            }

            return pkg;
        }

        /// <summary>
        /// Gegeven een aantal <paramref name="afdelingIDs"/>, lever de concatenatie op van de afkortingen van de
        /// afdelingen horende bij die IDs.
        /// </summary>
        /// <param name="afdelingIDs">ID's van afdelingen waarvan code te concateneren is.</param>
        /// <param name="alleAfdelingen">Lijst van afdelingen, die minstens de afdelingen met de gevraagde ID's 
        /// bevat.</param>
        /// <returns>String met geconcateneerde afdelingsafkortingen.</returns>
        private static string GeconcateneerdeAfdelingen(ICollection<int> afdelingIDs, IEnumerable<AfdelingInfo> alleAfdelingen)
        {
            string geconcateneerdeAfdelingen;
            if (afdelingIDs != null)
            {
                var afkortingen = from afd in alleAfdelingen
                    where afdelingIDs.Contains(afd.ID)
                    select afd.Afkorting + " "; // spatie als separator bij concatenatie
                geconcateneerdeAfdelingen = String.Concat(afkortingen);
            }
            else
            {
                geconcateneerdeAfdelingen = null;
            }
            return geconcateneerdeAfdelingen;
        }

        /// <summary>
        /// Wrapperfunctie om een object in het exceldocument te plaatsen. Snel gemaakt om de oude code
        /// te kunnen blijven gebruiken.
        /// </summary>
        /// <param name="worksheet">Worksheet waarin de string moet komen</param>
        /// <param name="value">Te inserten object</param>
        /// <param name="kolom">Kolomnummer (te beginnen van 1)</param>
        /// <param name="rij">Rijnumer (te beginnen van 1)</param>
        private static void Insert(ExcelWorksheet worksheet, Object value, int kolom, uint rij)
        {
            string celNaam = String.Format("{0}{1}", KolomLetter(kolom), rij);
            worksheet.Cells[celNaam].Value = value;
            if (value is DateTime)
            {
                worksheet.Cells[celNaam].Style.Numberformat.Format = Properties.Resources.DatumFormaat;
            }
        }

        private static object KolomLetter(int colNum)
        {
            var result = new StringBuilder();

            --colNum;

            int cycleNum = colNum / 26;
            int withinNum = colNum - (cycleNum * 26);

            if (cycleNum > 0)
            {
                result.Append((char)((cycleNum - 1) + 'a'));
            }
            result.Append((char)(withinNum + 'a'));
            return (result.ToString());

        }

        /// <summary>
        /// Genereer een Exceldocument op basis van een rij objecten van type <typeparamref name="T"/>,
        /// en gebruikt de waarden in <paramref name="koppen"/> als kolomtitels
        /// </summary>
        /// <typeparam name="T">Type van de objecten</typeparam>
        /// <param name="rows">Objecten die in een rij terecht moeten komen</param>
        /// <param name="koppen">Een array van strings die als kolomkoppen in het Exceldocument moeten komen</param>
        /// <param name="cols">Een (param-)array van lambda-expressies, die de kolommen van het document bepaalt</param>
        /// <returns>Een memorystream met daarin het Exceldocument</returns>
        public static ExcelPackage ExcelDocument<T>(IEnumerable<T> rows, string[] koppen, params Func<T, object>[] cols)
        {
            var pkg = new ExcelPackage();
            WerkBladMaken(pkg, Properties.Resources.StandaardWerkBladNaam, rows, koppen, cols);
            return pkg;
        }

        /// <summary>
        /// Maakt een werkblad bij in het bestaande Exceldocument <paramref name="pkg" />. Gegevens van het type 
        /// <typeparamref name="T"/> worden in kolommen weerkgegeven volgens de functies (lambda-expressies)
        /// <paramref name="cols"/>. De koppen van de kolom zijn gegeven door <paramref name="koppen"/>.
        /// </summary>
        /// <typeparam name="T">Type van de objecten</typeparam>
        /// <param name="pkg">Exceldocument waaraan het nieuwe werkblad toe te voegen is</param>
        /// <param name="rows">Objecten met op te lijsten gegevens</param>
        /// <param name="koppen">Namen van de kolomkoppen</param>
        /// <param name="cols"></param>
        /// <param name="naam"></param>
        /// <returns></returns>
        public static ExcelWorksheet WerkBladMaken<T>(ExcelPackage pkg, string naam, IEnumerable<T> rows, string[] koppen, params Func<T, object>[] cols)
        {
            var worksheet = pkg.Workbook.Worksheets.Add(naam);
            KolomTitelsInvullen(worksheet, koppen);
            WriteRows(worksheet, rows, cols);
            return worksheet;
        }

        private static void KolomTitelsInvullen(ExcelWorksheet werkBlad, IEnumerable<string> koppen)
        {
            int colIndex = 1;

            foreach (var kop in koppen)
            {
                // Zet de kolomtitels in de eerste rij
                Insert(werkBlad, kop, colIndex, 1);
                werkBlad.Row(1).Style.Font.Bold = true;
                ++colIndex;
            }
        }

        /// <summary>
        /// Maak een tabel in de eerste worksheet van een bestaand Exceldocument, op basis van een rij objecten
        /// van het type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type van de objecten in de rij</typeparam>
        /// <param name="spreadSheet">Spreadsheet waarin de tabel moet komen</param>
        /// <param name="rows">Rij objecten; elke rij is gebaseerd op een object</param>
        /// <param name="cols">Een (param)array van lambda-expressies, die de kolommen bepalen</param>
        public static void WriteRows<T>(ExcelWorksheet spreadSheet, IEnumerable<T> rows, params Func<T, object>[] cols)
        {
            uint rowIndex = 2; // In de eerste rij vulden we al kolomtitels in
			
            foreach (var rij in rows)
            {
                int colIndex = 1;

                foreach (var selector in cols)
                {
                    Insert(spreadSheet, selector(rij), colIndex, rowIndex);
                    ++colIndex;
                }
                ++rowIndex;
            }
        }
    }
}
