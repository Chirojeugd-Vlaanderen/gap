// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Diagnostics;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.WebApp.ActionFilters;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Deze controller bevat de method 'BaseModelInit', het BaseModel initialiseert.
    /// Verder ga ik hier proberen de IoC te arrangere voor de ServiceHelper
    /// </summary>
    /// <remarks>
    /// MasterAttribute helpt de overerving regelen
    /// Met dank aan 
    /// http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc
    /// </remarks>
    [Master]
    [HandleError]
    public abstract class BaseController : Controller
    {
        private readonly IServiceHelper _serviceHelper;
        private readonly IVeelGebruikt _veelGebruikt;

        protected IServiceHelper ServiceHelper { get { return _serviceHelper; } }
        protected IVeelGebruikt VeelGebruikt { get { return _veelGebruikt; } }

        /// <summary>
        /// Standaardconstructor.  <paramref name="serviceHelper"/> en <paramref name="veelGebruikt"/> worden
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="serviceHelper">wordt gebruikt om de webservices van de backend aan te spreken</param>
        /// <param name="veelGebruikt">haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        protected BaseController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt)
        {
            _serviceHelper = serviceHelper;
            _veelGebruikt = veelGebruikt;
        }

        /// <summary>
        /// Een standaard index pagina die door elke controller geimplementeerd moet zijn om de "terug naar 
        /// vorige" te kunnen implementeren.
        /// </summary>
        /// <param name="groepID"></param>
        /// <returns></returns>
        [HandleError]
        public abstract ActionResult Index(int groepID);

        /// <summary>
        /// Methode probeert terug te keren naar de vorige (in cookie) opgeslagen lijst. Als dit niet lukt gaat 
        /// hij naar de indexpagina van de controller terug.
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
        /// Methode probeert terug te keren naar de vorige (in cookie) opgeslagen fiche. In tweede instantie 
        /// probeert hij de vorige lijst, in laatste instantie gaat hij naar de indexpagina van de controller 
        /// terug.
        /// </summary>
        /// <returns></returns>
        [HandleError]
        protected ActionResult TerugNaarVorigeFiche()
        {
            ActionResult r;

            string url = ClientState.VorigeFiche ?? ClientState.VorigeLijst;
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
                #region gekozen groep en werkjaar

                var gwjDetail = VeelGebruikt.GroepsWerkJaarOphalen(groepID);

                model.GroepsNaam = gwjDetail.GroepNaam;
                model.GroepsNiveau = gwjDetail.GroepNiveau;
                model.Plaats = gwjDetail.GroepPlaats;
                model.StamNummer = gwjDetail.GroepCode;
                model.GroepID = gwjDetail.GroepID;
                model.HuidigWerkJaar = gwjDetail.WerkJaar;
                model.IsInOvergangsPeriode = gwjDetail.Status == WerkJaarStatus.InOvergang;

                #endregion

                #region GAV over meerdere groepen?

                // Als UniekeGroepGav een waarde heeft, is er maar één groep. Bij 0 zijn er meerdere.
                model.MeerdereGroepen = (VeelGebruikt.UniekeGroepGav(User.Identity.Name) == 0);

                #endregion

                #region Mededelingen

                // Eerst algemene mededelingen

                if (gwjDetail.Status == WerkJaarStatus.InOvergang)
                {
                    model.Mededelingen.Add(new Mededeling
                    {
                        Type = MededelingsType.Probleem,
                        Info = String.Format(
                            Properties.Resources.WerkJaarInOvergang,
                            gwjDetail.WerkJaar + 1,
                            gwjDetail.WerkJaar + 2)
                    });
                }

                // Problemen opvragen

                var functieProblemen = VeelGebruikt.FunctieProblemenOphalen(gwjDetail.GroepID);
                var ledenProblemen = VeelGebruikt.LedenProblemenOphalen(gwjDetail.GroepID);

                // Problemen vertalen naar model

                if (functieProblemen != null)
                {
                    foreach (var p in functieProblemen)
                    {
                        // Eerst een paar specifieke en veelvoorkomende problemen apart behandelen.

                        if (p.MinAantal > 0 && p.EffectiefAantal == 0)
                        {
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = String.Format(Properties.Resources.FunctieOntbreekt, p.Naam, p.Code)
                            });
                        }
                        else if (p.MaxAantal == 1 && p.EffectiefAantal > 1)
                        {
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = String.Format(Properties.Resources.FunctieMeerdereKeren, p.Naam, p.Code, p.EffectiefAantal)
                            });
                        }

                        // Dan de algemene foutmeldingen

                        else if (p.MinAantal > p.EffectiefAantal)
                        {
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = String.Format(Properties.Resources.FunctieTeWeinig, p.Naam, p.Code, p.EffectiefAantal, p.MinAantal)
                            });
                        }
                        else if (p.EffectiefAantal > p.MaxAantal)
                        {
                            model.Mededelingen.Add(new Mededeling
                            {
                                Type = MededelingsType.Probleem,
                                Info = String.Format(Properties.Resources.FunctieTeVeel, p.Naam, p.Code, p.EffectiefAantal, p.MinAantal)
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

                // TODO (#637): ook vermelden dat het werkjaar nog geïnitieerd moet worden als dat nog 
                // niet gebeurde!

                #endregion
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