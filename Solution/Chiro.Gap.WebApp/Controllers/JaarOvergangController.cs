// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller die de jaarovergangspagina's toont en input verwerkt
    /// </summary>
    public class JaarOvergangController : BaseController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="serviceHelper"/> en <paramref name="veelGebruikt"/> worden
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="serviceHelper">Wordt gebruikt om de webservices van de backend aan te spreken</param>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public JaarOvergangController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt) : base(serviceHelper, veelGebruikt) { }

		public override ActionResult Index(int groepID)
		{
			return Stap1AfdelingenSelecteren(groepID);
		}

		// Geef een lijst weer van alle beschikbare afdelingen, waaruit de groep kan kiezen welke afdelingen ze volgend jaar zullen hebben
		// (ze kunnen er ook nieuwe bijmaken)
		// Als de groep een kadergroep is, dan slaan we de afdelingsstappen over
		private ActionResult Stap1AfdelingenSelecteren(int groepID)
        {
            var model = new JaarOvergangAfdelingsModel();
            BaseModelInit(model, groepID);

            // Als we met een gewest/verbond te doen hebben, dan zijn de afdelingen niet relevant
            if(model.GroepsNiveau == Niveau.KaderGroep)
            {
				return Stap2AfdelingsJarenVerdelen(new JaarOvergangAfdelingsJaarModel(), groepID);
            }

			model.Titel = "Jaarovergang stap 1: welke afdelingen heeft je groep volgend jaar?";
            model.Afdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingInfo>>(g => g.AlleAfdelingenOphalen(groepID));
			return View("Stap1AfdelingenSelecteren", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Stap1AfdelingenSelecteren(JaarOvergangAfdelingsModel model1, int groepID)
        {
			return Stap2AfdelingsJarenVerdelen(model1.GekozenAfdelingsIDs, groepID);
        }

		// Gegeven een lijst van afdelingen die in het volgende werkjaar gelden, 
		// haal de 
		private ActionResult Stap2AfdelingsJarenVerdelen(IEnumerable<int> gekozenAfdelingsIDs, int groepID)
        {
            var model = new JaarOvergangAfdelingsJaarModel();
            BaseModelInit(model, groepID);

			model.Titel = "Jaarovergang stap 2:  instellingen van je afdelingen";
			model.NieuwWerkjaar = ServiceHelper.CallService<IGroepenService, int>(g => g.NieuwWerkJaarOphalen());
            model.OfficieleAfdelingen =
                ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(
                    e => e.OfficieleAfdelingenOphalen(groepID));

            var afdelingsinfos = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingInfo>>(g => g.AlleAfdelingenOphalen(groepID));

            var huidigwerkjaar = ServiceHelper.CallService<IGroepenService, GroepsWerkJaarDetail>(g => g.RecentsteGroepsWerkJaarOphalen(groepID));
			var periodeSindsVorigGwj = DateTime.Today.Year - huidigwerkjaar.WerkJaar;

            // Haal de huidige actieve afdelingen op, om zoveel mogelijk informatie te kunnen overnemen in het scherm
			var actievelijst = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingDetail>>(g => g.ActieveAfdelingenOphalen(huidigwerkjaar.WerkJaarID));

            // laadt de details in van alle afdelingen die geselecteerd zijn voor het nieuwe werkjaar
            var afdelingDetails = new List<AfdelingDetail>();
			foreach (var afd in afdelingsinfos)
            {
                if (!gekozenAfdelingsIDs.Contains(afd.ID))
                {
                    continue;
                }

				var afddetail = new AfdelingDetail
									{
										AfdelingAfkorting = afd.Afkorting,
										AfdelingID = afd.ID,
										AfdelingNaam = afd.Naam,
										GeboorteJaarTot = 0,
										GeboorteJaarVan = 0,
										Geslacht = GeslachtsType.Onbekend
									};

				afdelingDetails.Add(afddetail);

                // Lokale variabele om "Access to modified closure" te vermijden [wiki:VeelVoorkomendeWaarschuwingen#Accesstomodifiedclosure]
                var afd1 = afd;
                var actieveafdeling = (from actafd in actievelijst
                           where actafd.AfdelingID == afd1.ID
                           select actafd).FirstOrDefault();

				if (actieveafdeling != null) // Er is een afdelingsjaar actief op dit moment, dus we kunnen meer info inladen
                {
					afddetail.OfficieleAfdelingID	= actieveafdeling.OfficieleAfdelingID;
					afddetail.Geslacht				= actieveafdeling.Geslacht;
					afddetail.GeboorteJaarTot		= actieveafdeling.GeboorteJaarTot + periodeSindsVorigGwj;
					afddetail.GeboorteJaarVan		= actieveafdeling.GeboorteJaarVan + periodeSindsVorigGwj;
                }
            }

            // Sorteer de afdelingsjaren: eerst die zonder gegevens, dan van ribbels naar aspiranten
			model.Afdelingen = (from a in afdelingDetails
                                 orderby a.GeboorteJaarTot descending
                                 orderby a.GeboorteJaarTot == 0 descending
                                 select a);

            // TODO extra info pagina voor jaarovergang
            // TODO kan validatie in de listhelper worden bijgecodeerd?
            // TODO foutmeldingen
			return View("Stap2AfdelingsJarenVerdelen", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Stap2AfdelingsJarenVerdelen(JaarOvergangAfdelingsJaarModel model, int groepID)
        {
            var tecreerenafdelingsjaren = new List<TeActiverenAfdelingInfo>();

            if (model.AfdelingsIDs.Count != model.TotLijst.Count || model.TotLijst.Count != model.GeslLijst.Count || model.TotLijst.Count != model.VanLijst.Count)
            {
                TempData["fout"] = "Niet alle informatie is ingevuld, gelieve aan te vullen.";
				return View("Stap2AfdelingsJarenVerdelen", model);
            }

            if (model.TotLijst.Any(e => e.Equals(string.Empty)) || model.VanLijst.Any(e => e.Equals(string.Empty)))
            {
                TempData["fout"] = "Niet alle informatie is ingevuld, gelieve aan te vullen.";
				return View("Stap2AfdelingsJarenVerdelen", model);
            }
			
            // ReSharper disable LoopCanBeConvertedToQuery
            for (var i = 0; i < model.VanLijst.Count; i++)
            {
                var x = new TeActiverenAfdelingInfo
                            {
                                AfdelingID = Int32.Parse(model.AfdelingsIDs[i]),
                                OfficieleAfdelingID = Int32.Parse(model.OfficieleAfdelingsIDs[i]),
                                GeboorteJaarTot = Int32.Parse(model.TotLijst[i]),
                                GeboorteJaarVan = Int32.Parse(model.VanLijst[i]),
                                Geslacht = (GeslachtsType)Int32.Parse(model.GeslLijst[i])
                            };

				tecreerenafdelingsjaren.Add(x);
            }
            // ReSharper restore LoopCanBeConvertedToQuery

			// Leden zoeken in het vorige actieve werkjaar, dus opvragen voor we de jaarovergang zelf doen
			var vorigGwjID = ServiceHelper.CallService<IGroepenService, int>(g => g.RecentsteGroepsWerkJaarIDGet(groepID));

            try
            {
				ServiceHelper.CallService<IGroepenService>(s => s.JaarovergangUitvoeren(tecreerenafdelingsjaren, groepID));
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                TempData["fout"] = ex.Message;
				return View("Stap2AfdelingsJarenVerdelen", model);
            }

			VeelGebruikt.JaarOvergangReset(groepID);

        	if (!model.LedenMeteenInschrijven)
        	{
        		return RedirectToAction("Index", "Leden");
        	}

			// Leden uit het oude werkjaar opvragen om lid te maken
			var filter = new LidFilter
        		            {
								GroepsWerkJaarID = vorigGwjID,
        		             	AfdelingID = null,
        		             	FunctieID = null,
        		             	ProbeerPeriodeNa = null,
        		             	HeeftVoorkeurAdres = null,
        		             	HeeftTelefoonNummer = null,
        		             	HeeftEmailAdres = null,
        		             	LidType = LidType.Alles
        		            };

        	// TODO nogal duur om eerst alle leden op te halen, best herschrijven
        	var gelieerdepersoonIDs =
        		ServiceHelper.CallService<ILedenService, IList<LidOverzicht>>(svc => svc.Zoeken(filter, false)).Select(
        			e => e.GelieerdePersoonID).ToList();

        	TempData["list"] = gelieerdepersoonIDs;
        	return RedirectToAction("LedenMaken", "Leden");
        }

        /// <summary>
        /// Toont de view die toelaat een nieuwe afdeling te maken.
        /// </summary>
        /// <param name="groepID">Groep waarvoor de afdeling gemaakt moet worden</param>
        /// <returns>De view die toelaat een nieuwe afdeling te maken.</returns>
        public ActionResult NieuweAfdelingMaken(int groepID)
        {
            var model = new AfdelingInfoModel
                        	{
                        		Titel = Properties.Resources.NieuweAfdelingTitel,
                        		Info = new AfdelingInfo()
                        	};

        	BaseModelInit(model, groepID);

			return View("NieuweAfdelingMaken", model);
        }

        /// <summary>
        /// Maakt een nieuwe afdeling, op basis van <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Bevat naam en code voor de nieuwe afdeling</param>
        /// <param name="groepID">ID van de groep waarvoor de afdeling gemaakt moet worden</param>
        /// <returns>Het overzicht van de afdelingen, indien de nieuwe afdeling goed gemaakt is.
        /// In het andere geval opnieuw de view om een afdeling bij te maken.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult NieuweAfdelingMaken(AfdelingInfoModel model, int groepID)
        {
            model.Titel = Properties.Resources.NieuweAfdelingTitel;
            BaseModelInit(model, groepID);

            if (!ModelState.IsValid)
            {
                // Modelstate bevat ongeldige waarden; toon de pagina opnieuw
                return View(model);
            }

            try
            {
                ServiceHelper.CallService<IGroepenService>(e => e.AfdelingAanmaken(groepID, model.Info.Naam, model.Info.Afkorting));

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

                return RedirectToAction("Index");
            }
            catch (FaultException<BestaatAlFault<AfdelingInfo>> ex)
            {
                if (string.Compare(ex.Detail.Bestaande.Afkorting, model.Info.Afkorting, true) == 0)
                {
                    ModelState.AddModelError(
                        "Info.Afkorting",
                        string.Format(Properties.Resources.AfdelingsCodeBestaatAl, ex.Detail.Bestaande.Afkorting, ex.Detail.Bestaande.Naam));
                }
                else if (string.Compare(ex.Detail.Bestaande.Naam, model.Info.Naam, true) == 0)
                {
                    ModelState.AddModelError(
                        "Info.Naam",
                        string.Format(
                            Properties.Resources.AfdelingsNaamBestaatAl, ex.Detail.Bestaande.Afkorting, ex.Detail.Bestaande.Naam));
                }
                else
                {
                    // Mag niet gebeuren
                    ModelState.AddModelError("Er heeft zich een fout voorgedaan, gelieve contact op te nemen met het secretariaat.", ex);
                    Debug.Assert(false);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Toont de view om een afdeling aan te passen, gegeven een afdelingsID
        /// </summary>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <param name="afdelingID"></param>
        /// <returns></returns>
        public ActionResult Bewerken(int groepID, int afdelingID)
        {
            var model = new AfdelingInfoModel();
            BaseModelInit(model, groepID);

            model.Info = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(afdelingID));

            model.Titel = "Afdeling bewerken";
			return View("Stap1AfdelingenSelecteren", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Bewerken(AfdelingInfoModel model, int groepID)
        {
            BaseModelInit(model, groepID);

            try
            {
                ServiceHelper.CallService<IGroepenService>(e => e.AfdelingBewaren(model.Info));

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // TODO dit kan beter
                TempData["fout"] = ex.Message;

                model.Info = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(model.Info.ID));

                model.Titel = "Afdeling bewerken";
				return View("Stap1AfdelingenSelecteren", model);
            }
        }
    }
}
