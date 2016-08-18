/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.Diagnostics;
using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Deze controller bevat de method 'BaseModelInit', het BaseModel initialiseert.
    /// Verder ga ik hier proberen de IoC te arrangere voor de ServiceHelper
    /// </summary>
    [HandleError]
    public abstract class BaseController : Controller
    {
        private readonly IVeelGebruikt _veelGebruikt;
        private readonly ServiceHelper _serviceHelper;

        protected IVeelGebruikt VeelGebruikt { get { return _veelGebruikt; } }
        protected ServiceHelper ServiceHelper { get { return _serviceHelper; } }

        /// <summary>
        /// Standaardconstructor.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        /// <param name="serviceHelper">Helper class voor service calls.</param>
        protected BaseController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper)
        {
            _veelGebruikt = veelGebruikt;
            _serviceHelper = serviceHelper;
        }

        /// <summary>
        /// Een standaard indexpagina die door elke controller geimplementeerd moet zijn om de "terug naar 
        /// vorige" te kunnen implementeren.
        /// </summary>
        /// <param name="groepID">ID van de groep die de pagina oproept, en van wie we dus gegevens moeten tonen</param>
        /// <returns></returns>
        [HandleError]
        public abstract ActionResult Index(int groepID);

        /// <summary>
        /// Methode probeert terug te keren naar de vorige (in cookie) opgeslagen lijst. Als dit niet lukt gaat 
        /// hij naar de indexpagina van de controller terug.
        /// Belangrijkste voordeel is het bijhouden van de instellingen zoals sortering, pagina, ..., zonder al die informatie overal in de url of in de post te houden.
        /// Er zijn wel enkele issues als er multitabs gebruikt worden, maar kunnen niet leiden tot echte bugs.
        /// </summary>
        /// <returns></returns>
        [HandleError]
        protected ActionResult TerugNaarVorigeLijst()
        {
            ActionResult r;

            string url = ClientState.VorigeLijst;
            if (url == null)
            {
                // ReSharper disable Asp.NotResolved
                r = RedirectToAction("Index");
                // ReSharper restore Asp.NotResolved
            }
            else
            {
                r = Redirect(url);
            }

            return r;
        }

        /// <summary>
        /// Vult de groepsgegevens in in de base view.
        /// </summary>
        /// <param name="model">Te initialiseren model</param>
        /// <param name="groepID">ID van de gewenste groep</param>
        [HandleError]
        protected void BaseModelInit(MasterViewModel model, int groepID)
        {
            // Werken we op test of live?
            model.IsLive = VeelGebruikt.IsLive();

            if (groepID == 0)
            {
                // De Gekozen groep is nog niet gekend, zet defaults
                model.GroepsNaam = Properties.Resources.GroepsnaamDefault;
                model.Plaats = Properties.Resources.GroepPlaatsDefault;
                model.StamNummer = Properties.Resources.StamNrDefault;
                model.MeerdereGroepen = false;
                // model.GroepsCategorieen = new List<SelectListItem>();
            }
            else
            {
                #region gekozen groep en werkJaar

                var gwjDetail = VeelGebruikt.GroepsWerkJaarOphalen(groepID);

                model.GroepsNaam = gwjDetail.GroepNaam;
                model.GroepsNiveau = gwjDetail.GroepNiveau;
                model.Plaats = gwjDetail.GroepPlaats;
                model.StamNummer = gwjDetail.GroepCode;
                model.GroepID = gwjDetail.GroepID;
                model.HuidigWerkJaar = gwjDetail.WerkJaar;
                model.WerkJaarStatus = gwjDetail.Status;

                #endregion

                #region GAV over meerdere groepen?

                // Als UniekeGroepGav een waarde heeft, is er maar één groep. Bij 0 zijn er meerdere.

                model.MeerdereGroepen = (VeelGebruikt.UniekeGroepGav(User == null ? null : User.Identity.Name) == 0);

                #endregion

                #region Mededelingen

                // Eerst algemene mededelingen

                if (gwjDetail.Status == WerkJaarStatus.InOvergang)
                {
                    var url = Url.Action("Index","JaarOvergang");
                    model.Mededelingen.Add(new Mededeling
                    {
                        Type = MededelingsType.Probleem,
                        Info = String.Format(
                            Properties.Resources.WerkJaarInOvergang,
                            gwjDetail.WerkJaar + 1,
                            gwjDetail.WerkJaar + 2,
                            url)
                    });
                }

                var bivakstatus = VeelGebruikt.BivakStatusHuidigWerkjaarOphalen(groepID);
                VoegBivakStatusMededelingenToe(bivakstatus, model.Mededelingen);

                // Problemen opvragen
                var functieProblemen = VeelGebruikt.FunctieProblemenOphalen(gwjDetail.GroepID);
                var ledenProblemen = VeelGebruikt.LedenProblemenOphalen(gwjDetail.GroepID);

                // Problemen vertalen naar model
                if (functieProblemen != null)
                {
                    foreach (var p in functieProblemen)
                    {
                        var bekijkMetFunctieUrl = Url.Action("Functie", "Leden",
                            new
                            {
                                groepsWerkJaarID = gwjDetail.WerkJaarID,
                                id = p.ID,
                                groepID = groepID,
                            });
                        // Eerst een paar specifieke en veelvoorkomende problemen apart behandelen.
                        if (p.MinAantal > 0 && p.EffectiefAantal == 0)
                        {
                            var statusToekennenUrl = Url.Action("ZelfFunctiesToekennen", "Leden",
                            new
                            {
                                groepID = groepID,
                            });
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = string.Format(Properties.Resources.FunctieOntbreekt, p.Naam, p.Code, bekijkMetFunctieUrl, statusToekennenUrl)
                            });
                        }
                        else if (p.MaxAantal == 1 && p.EffectiefAantal > 1)
                        {
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = string.Format(Properties.Resources.FunctieMeerdereKeren, p.Naam, p.Code, p.EffectiefAantal,bekijkMetFunctieUrl)
                            });
                        }
                        // Dan de algemene foutmeldingen
                        else if (p.MinAantal > p.EffectiefAantal)
                        {
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = string.Format(Properties.Resources.FunctieTeWeinig, p.Naam, p.Code, p.EffectiefAantal, p.MinAantal, bekijkMetFunctieUrl)
                            });
                        }
                        else if (p.EffectiefAantal > p.MaxAantal)
                        {
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = string.Format(Properties.Resources.FunctieTeVeel, p.Naam, p.Code, p.EffectiefAantal, p.MinAantal, bekijkMetFunctieUrl)
                            });
                        }
                    }
                }

                if (ledenProblemen != null)
                {
                    foreach (var p in ledenProblemen)
                    {
                        string boodschap = String.Empty;
                        string url = String.Empty;

                        switch (p.Probleem)
                        {
                            case LidProbleem.AdresOntbreekt:
                                boodschap = Properties.Resources.LedenZonderAdres;
                                url = Url.Action("ZonderAdres", "Leden");
                                break;
                            case LidProbleem.EmailOntbreekt:
                                boodschap = Properties.Resources.LeidingZonderEmail;
                                url = Url.Action("LeidingZonderMail", "Leden");
                                break;
                            case LidProbleem.TelefoonNummerOntbreekt:
                                boodschap = Properties.Resources.LedenZonderTelefoonNummer;
                                url = Url.Action("ZonderTelefoon", "Leden");
                                break;
                            default:
                                Debug.Assert(false);
                                break;
                        }

                        model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = String.Format(boodschap, p.Aantal, url)
                            });
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Gaat na of de probleemmeldingen i.v.m. het bivak van belang zijn, en zo ja,
        /// dan worden ze toegevoegd aan de problemen waarvoor de gebruiker een waarschuwing ziet.
        /// </summary>
        /// <param name="aangiftestatus">Geeft aan of de bivakaangifte al dan niet van belang is (op dit moment)</param>
        /// <param name="mededelingen">De problemen i.v.m. de bivakaangifte die getoond moeten worden</param>
        [HandleError]
        private void VoegBivakStatusMededelingenToe(BivakAangifteLijstInfo aangiftestatus, IList<Mededeling> mededelingen)
        {
            if (aangiftestatus == null)
            {
                return;
            }
            
            if (aangiftestatus.Bivakinfos == null)
            {
                return;
            }
            
            if (aangiftestatus.AlgemeneStatus == BivakAangifteStatus.NogNietVanBelang || aangiftestatus.AlgemeneStatus == BivakAangifteStatus.Ok)
            {
                return;
            }
            
            if (aangiftestatus.AlgemeneStatus == BivakAangifteStatus.Ontbrekend && aangiftestatus.Bivakinfos.Count == 0)
            {
                var url = Url.Action("Nieuw", "Uitstappen");
                mededelingen.Add(new Mededeling
                {
                    Type = MededelingsType.Probleem,
                    Info = String.Format(Properties.Resources.BivakAangifteNogInTeVullen, url)
                });
            }
            else
            {
                foreach (var bivakstatus in aangiftestatus.Bivakinfos)
                {
                    if (bivakstatus.Status == BivakAangifteStatus.PlaatsEnContactOntbreekt)
                    {
                        var url = Url.Action("Bekijken", "Uitstappen", new { id = bivakstatus.ID });
                        mededelingen.Add(new Mededeling
                        {
                            Type = MededelingsType.Probleem,
                            Info = String.Format(Properties.Resources.BeideNogInvullenOpBivakAangifte, bivakstatus.Omschrijving, url)
                        });
                    }
                    if (bivakstatus.Status == BivakAangifteStatus.PlaatsOntbreekt)
                    {
                        var url = Url.Action("PlaatsBewerken", "Uitstappen", new { id = bivakstatus.ID });
                        mededelingen.Add(new Mededeling
                        {
                            Type = MededelingsType.Probleem,
                            Info = String.Format(Properties.Resources.AdresNogInvullenOpBivakAangifte, bivakstatus.Omschrijving, url)
                        });
                    }
                    else if (bivakstatus.Status == BivakAangifteStatus.ContactOntbreekt)
                    {
                        var url = Url.Action("Bekijken", "Uitstappen", new { id = bivakstatus.ID });
                        mededelingen.Add(new Mededeling
                        {
                            Type = MededelingsType.Probleem,
                            Info = String.Format(Properties.Resources.PersoonNogInvullenOpBivakAangifte, bivakstatus.Omschrijving, url)
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Vult de groepsgegevens én de paginatitel in in de base view
        /// </summary>
        /// <param name="model">Te initialiseren model</param>
        /// <param name="groepID">ID van de gewenste groep</param>
        /// <param name="titel">Titel van de pagina</param>
        [HandleError]
        protected void BaseModelInit(MasterViewModel model, int groepID, string titel)
        {
            BaseModelInit(model, groepID);
            model.Titel = titel;
        }

        /*//	Wordt alleen door Bart gebruikt

        protected override void OnException(ExceptionContext filterContext)
        {
            try
            {
                LogSchrijven(Properties.Settings.Default.LogBestandPad,
                             string.Format("Gebruiker '{0}' veroorzaakte de volgende fout tussen {1} en {2}, op controller {3}: {4}",
                           HttpContext.User.Identity.Name,
                           Request.UrlReferrer, Request.Url,
                           filterContext.Controller, filterContext.Exception));
            }
            catch (Exception)
            {
                // Tja, wat doen we in zo'n geval?
            }
        }

        static void LogSchrijven(string pad, string tekst)
        {
            var boodschap = new StringBuilder();
            boodschap.AppendLine(DateTime.Now.ToString());
            boodschap.AppendLine(tekst);
            boodschap.AppendLine("=====================================");

            System.IO.File.AppendAllText(pad, boodschap.ToString());
        }
         */
    }
}