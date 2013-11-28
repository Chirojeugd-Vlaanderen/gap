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
using System.IO;
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Packaging;

namespace Chiro.Gap.ExcelManip
{
    /// <summary>
    /// GAP-specifieke Excelmanipulaties
    /// </summary>
    public class GapExcelManip: Cdf.ExcelManip.ExcelManip
    {
        /// <summary>
        /// Genereer een Exceldocument op basis van een rij PersoonLidInfo-objecten.
        /// </summary>
        /// <param name="rows">Objecten die in een rij terecht moeten komen</param>
        /// <param name="alleAfdelingen">Lijstje van (minstens) alle gebruikte afdelingen</param>
        /// <returns>Een memorystream met daarin het Exceldocument</returns>
        public MemoryStream LidExcelTabel(IList<PersoonLidInfo> rows, IList<AfdelingDetail> alleAfdelingen)
        {
            var result = new MemoryStream();

            // Creëer een nieuw document in de stream
            CreateSpreadsheetWorkbook(result);

            var aantallen = (from r in rows
                select new
                       {
                           AantalAdressen = r.PersoonsAdresInfo.Count(),
                           AantalEmail =
                               r.CommunicatieInfo.Count(ci => ci.CommunicatieTypeID == (int) CommunicatieTypeEnum.Email),
                           AantalTel =
                               r.CommunicatieInfo.Count(
                                   ci => ci.CommunicatieTypeID == (int) CommunicatieTypeEnum.TelefoonNummer)
                       }).ToList();

            int maxAantalAdressen = aantallen.Max(a => a.AantalAdressen);
            int maxAantalEmail = aantallen.Max(a => a.AantalEmail);
            int maxAantalTel = aantallen.Max(a => a.AantalTel);

            // Dit is vrij omslachtig. Maar voorlopig heb ik niets beter.

            using (var spreadSheet = SpreadsheetDocument.Open(result, true))
            {
                // Bouw koppen op

                InsertText(spreadSheet, "Type", 1, 1);
                InsertText(spreadSheet, "AD-nr.", 2, 1);
                InsertText(spreadSheet, "Voornaam", 3, 1);
                InsertText(spreadSheet, "Naam", 4, 1);
                InsertText(spreadSheet, "Afdelingen", 5, 1);
                InsertText(spreadSheet, "Functies", 6, 1);
                InsertText(spreadSheet, "Geboortedatum", 7, 1);
                InsertText(spreadSheet, "Betaald", 8, 1);

                for (int i = 0; i < maxAantalAdressen; ++i)
                {
                    InsertText(spreadSheet, String.Format("Straat {0}", i + 1), i * 7 + 9, 1);
                    InsertText(spreadSheet, String.Format("Nr. {0}", i + 1), i * 7 + 10, 1);
                    InsertText(spreadSheet, String.Format("Bus {0}", i + 1), i * 7 + 11, 1);
                    InsertText(spreadSheet, String.Format("Postnr. {0}", i + 1), i * 7 + 12, 1);
                    InsertText(spreadSheet, String.Format("Postcode {0}", i + 1), i * 7 + 13, 1);
                    InsertText(spreadSheet, String.Format("Woonplaats {0}", i + 1), i * 7 + 14, 1);
                    InsertText(spreadSheet, String.Format("Land {0}", i + 1), i * 7 + 15, 1);
                }

                for (int i = 0; i < maxAantalTel; ++i)
                {
                    InsertText(spreadSheet, String.Format("Tel. {0}", i + 1), 7 * maxAantalAdressen + i + 9, 1);
                }

                //for (int i = 0; i < maxAantalEmail; ++i)
                //{
                //    InsertText(spreadSheet, String.Format("E-mail. {0}", i + 1),
                //        7 * maxAantalAdressen + maxAantalTel + i + 9, 1);
                //}

                // de effectieve gegevens

                uint rijNr = 2;

                foreach (var rij in rows)
                {
                    InsertText(spreadSheet, rij.LidInfo.Type.ToString(), 1, rijNr);
                    if (rij.PersoonDetail.AdNummer != null)
                    {
                        InsertNumber(spreadSheet, (double)rij.PersoonDetail.AdNummer, 2, rijNr);
                    }
                    InsertText(spreadSheet, rij.PersoonDetail.VoorNaam, 3, rijNr);
                    InsertText(spreadSheet, rij.PersoonDetail.Naam, 4, rijNr);

                    var afdelingIDs = rij.LidInfo.AfdelingIdLijst;

                    if (afdelingIDs != null)
                    {
                        var afkortingen = from afd in alleAfdelingen
                                          where afdelingIDs.Contains(afd.AfdelingID)
                                          select afd.AfdelingAfkorting + " "; // spatie als separator bij concatenatie
                        InsertText(spreadSheet, String.Concat(afkortingen), 5, rijNr);
                    }

                    if (rij.LidInfo.Functies != null)
                    {
                        InsertText(spreadSheet, String.Concat(rij.LidInfo.Functies.Select(fn => fn.Code + " ")), 6, rijNr);
                    }

                    if (rij.PersoonDetail.GeboorteDatum != null)
                    {
                        InsertDate(spreadSheet, rij.PersoonDetail.GeboorteDatum.Value, 7, rijNr);
                    }
                    InsertText(spreadSheet, rij.LidInfo.LidgeldBetaald ? "Ja" : "Nee", 8, rijNr);

                    int i = 0;
                    foreach (var adres in rij.PersoonsAdresInfo)
                    {
                        InsertText(spreadSheet, adres.StraatNaamNaam, i * 7 + 9, rijNr);
                        if (adres.HuisNr != null)
                        {
                            InsertNumber(spreadSheet, (double)adres.HuisNr, i * 7 + 10, rijNr);
                        }
                        InsertText(spreadSheet, adres.Bus, i * 7 + 11, rijNr);
                        InsertNumber(spreadSheet, adres.PostNr, i * 7 + 12, rijNr);
                        InsertText(spreadSheet, adres.PostCode, i * 7 + 13, rijNr);
                        InsertText(spreadSheet, adres.WoonPlaatsNaam, i * 7 + 14, rijNr);
                        InsertText(spreadSheet, String.Format("Land {0}", i + 1), i * 7 + 15, 1);
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
                        InsertText(spreadSheet, output, 7 * maxAantalAdressen + i + 9, rijNr);
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
                        InsertText(spreadSheet, output, 7 * maxAantalAdressen + maxAantalTel + i + 9, rijNr);
                        ++i;
                    }
                    ++rijNr;
                }
            }
            return result;
        }
    }
}
