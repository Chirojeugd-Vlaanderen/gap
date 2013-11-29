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
    public class GapExcelManip
    {
        /// <summary>
        /// Genereer een Exceldocument op basis van een rij PersoonLidInfo-objecten.
        /// </summary>
        /// <param name="rows">Objecten die in een rij terecht moeten komen</param>
        /// <param name="alleAfdelingen">Lijstje van (minstens) alle gebruikte afdelingen</param>
        /// <returns>Een memorystream met daarin het Exceldocument</returns>
        public ExcelPackage LidExcelTabel(IList<PersoonLidInfo> rows, IList<AfdelingDetail> alleAfdelingen)
        {
            var pkg = new ExcelPackage();
            var aantallen = (from r in rows
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

            // Dit is vrij omslachtig. Maar voorlopig heb ik niets beter.

            var worksheet = pkg.Workbook.Worksheets.Add("Ledenlijst");
            // Bouw koppen op

            Insert(worksheet, "Type", 1, 1);
            Insert(worksheet, "AD-nr.", 2, 1);
            Insert(worksheet, "Voornaam", 3, 1);
            Insert(worksheet, "Naam", 4, 1);
            Insert(worksheet, "Afdelingen", 5, 1);
            Insert(worksheet, "Functies", 6, 1);
            Insert(worksheet, "Geboortedatum", 7, 1);
            Insert(worksheet, "Betaald", 8, 1);

            for (int i = 0; i < maxAantalAdressen; ++i)
            {
                Insert(worksheet, String.Format("Straat {0}", i + 1), i*7 + 9, 1);
                Insert(worksheet, String.Format("Nr. {0}", i + 1), i*7 + 10, 1);
                Insert(worksheet, String.Format("Bus {0}", i + 1), i*7 + 11, 1);
                Insert(worksheet, String.Format("Postnr. {0}", i + 1), i*7 + 12, 1);
                Insert(worksheet, String.Format("Postcode {0}", i + 1), i*7 + 13, 1);
                Insert(worksheet, String.Format("Woonplaats {0}", i + 1), i*7 + 14, 1);
                Insert(worksheet, String.Format("Land {0}", i + 1), i*7 + 15, 1);
            }

            for (int i = 0; i < maxAantalTel; ++i)
            {
                Insert(worksheet, String.Format("Tel. {0}", i + 1), 7*maxAantalAdressen + i + 9, 1);
            }

            for (int i = 0; i < maxAantalEmail; ++i)
            {
                Insert(worksheet, String.Format("E-mail. {0}", i + 1),
                    7*maxAantalAdressen + maxAantalTel + i + 9, 1);
            }

            // de effectieve gegevens

            uint rijNr = 2;

            foreach (var rij in rows)
            {
                Insert(worksheet, rij.LidInfo.Type.ToString(), 1, rijNr);
                Insert(worksheet, rij.PersoonDetail.AdNummer, 2, rijNr);
                Insert(worksheet, rij.PersoonDetail.VoorNaam, 3, rijNr);
                Insert(worksheet, rij.PersoonDetail.Naam, 4, rijNr);

                var afdelingIDs = rij.LidInfo.AfdelingIdLijst;

                if (afdelingIDs != null)
                {
                    var afkortingen = from afd in alleAfdelingen
                                      where afdelingIDs.Contains(afd.AfdelingID)
                                      select afd.AfdelingAfkorting + " "; // spatie als separator bij concatenatie
                    Insert(worksheet, String.Concat(afkortingen), 5, rijNr);
                }

                if (rij.LidInfo.Functies != null)
                {
                    Insert(worksheet, String.Concat(rij.LidInfo.Functies.Select(fn => fn.Code + " ")), 6,
                        rijNr);
                }

                Insert(worksheet, rij.PersoonDetail.GeboorteDatum, 7, rijNr);
                Insert(worksheet, rij.LidInfo.LidgeldBetaald ? "Ja" : "Nee", 8, rijNr);

                int i = 0;
                foreach (var adres in rij.PersoonsAdresInfo)
                {
                    Insert(worksheet, adres.StraatNaamNaam, i * 7 + 9, rijNr);
                    Insert(worksheet, adres.HuisNr, i * 7 + 10, rijNr);
                    Insert(worksheet, adres.Bus, i * 7 + 11, rijNr);
                    Insert(worksheet, adres.PostNr, i * 7 + 12, rijNr);
                    Insert(worksheet, adres.PostCode, i * 7 + 13, rijNr);
                    Insert(worksheet, adres.WoonPlaatsNaam, i * 7 + 14, rijNr);
                    Insert(worksheet, String.Format("Land {0}", i + 1), i * 7 + 15, 1);
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
                    Insert(worksheet, output, 7 * maxAantalAdressen + i + 9, rijNr);
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
                    Insert(worksheet, output, 7 * maxAantalAdressen + maxAantalTel + i + 9, rijNr);
                    ++i;
                }
                ++rijNr;
            }
            return pkg;
        }

        /// <summary>
        /// Wrapperfunctie om een object in het exceldocument te plaatsen. Snel gemaakt om de oude code
        /// te kunnen blijven gebruiken.
        /// </summary>
        /// <param name="worksheet">Worksheet waarin de string moet komen</param>
        /// <param name="value">Te inserten object</param>
        /// <param name="kolom">Kolomnummer (te beginnen van 1)</param>
        /// <param name="rij">Rijnumer (te beginnen van 1)</param>
        private void Insert(ExcelWorksheet worksheet, Object value, int kolom, uint rij)
        {
            string celNaam = String.Format("{0}{1}", KolomLetter(kolom), rij);
            worksheet.Cells[celNaam].Value = value;
        }

        private object KolomLetter(int colNum)
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
        /// <param name="koppen">Een (param)array van strings die als kolomkoppen in het Exceldocument moeten komen</param>
        /// <param name="cols">Een (param)array van lambda-expressies, die de kolommen van het document bepaalt</param>
        /// <returns>Een memorystream met daarin het Exceldocument</returns>
        public ExcelPackage ExcelDocument<T>(IEnumerable<T> rows, string[] koppen, params Func<T, object>[] cols)
        {
            var pkg = new ExcelPackage();
            var worksheet = pkg.Workbook.Worksheets.Add("Ledenlijst");
            KolomTitelsInvullen(worksheet, koppen);
            WriteRows(worksheet, rows, cols);

            return pkg;
        }

        protected void KolomTitelsInvullen(ExcelWorksheet spreadSheet, string[] koppen)
        {
            int colIndex = 1;

            foreach (var kop in koppen)
            {
                // Zet de kolomtitels in de eerste rij
                Insert(spreadSheet, kop, colIndex, 1);
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
        public void WriteRows<T>(ExcelWorksheet spreadSheet, IEnumerable<T> rows, params Func<T, object>[] cols)
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
