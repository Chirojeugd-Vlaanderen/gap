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
    /// 
    /// </summary>
    public class JaarOvergangController : BaseController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="serviceHelper"/> en <paramref name="veelGebruikt"/> worden
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="serviceHelper">wordt gebruikt om de webservices van de backend aan te spreken</param>
        /// <param name="veelGebruikt">haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public JaarOvergangController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt) : base(serviceHelper, veelGebruikt) { }

        public override ActionResult Index(int groepID)
        {
            return AfdelingenMaken(groepID);
        }

        public ActionResult AfdelingenMaken(int groepID)
        {
            var model = new JaarOvergangAfdelingsModel();
            BaseModelInit(model, groepID);

            // Als we met een gewest/verbond te doen hebben, dan zijn de afdelingen niet relevant

            if ((model.GroepsNiveau & Niveau.Groep) == 0)
            {
                return VerdelingMaken(new JaarOvergangAfdelingsJaarModel(), groepID);
            }

            IEnumerable<AfdelingInfo> lijst = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingInfo>>(g => g.AlleAfdelingenOphalen(groepID));

            model.Afdelingen = lijst;
            model.Titel = "Jaarovergang stap 1: welke afdelingen heeft je groep volgend jaar?";
            return View("AfdelingenAanmaken", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AfdelingenMaken(JaarOvergangAfdelingsModel model1, int groepID)
        {
            return VerdelingMaken(model1.GekozenAfdelingsIDs, groepID);
        }

        public ActionResult VerdelingMaken(IEnumerable<int> gekozenAfdelingsIDs, int groepID)
        {
            var model2 = new JaarOvergangAfdelingsJaarModel();
            BaseModelInit(model2, groepID);

            model2.OfficieleAfdelingen =
                ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(
                    e => e.OfficieleAfdelingenOphalen(groepID));

            var lijst = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingInfo>>(g => g.AlleAfdelingenOphalen(groepID));

            var werkjaarID = ServiceHelper.CallService<IGroepenService, int>(g => g.RecentsteGroepsWerkJaarIDGet(groepID));

            // Haal de huidige actieve afdelingen op, om zoveel mogelijk informatie te kunnen overnemen in het scherm
            var actievelijst = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingDetail>>(g => g.ActieveAfdelingenOphalen(werkjaarID));

            // actieve info inladen
            var volledigelijst = new List<AfdelingDetail>();
            foreach (var afd in lijst)
            {
                if (!gekozenAfdelingsIDs.Contains(afd.ID))
                {
                    continue;
                }

                // Lokale variabele om "Access to modified closure" te vermijden [wiki:VeelVoorkomendeWaarschuwingen#Accesstomodifiedclosure]
                var afd1 = afd;
                var act = (from actafd in actievelijst
                           where actafd.AfdelingID == afd1.ID
                           select actafd).FirstOrDefault();

                var afddetail = new AfdelingDetail
                                    {
                                        AfdelingAfkorting = afd.Afkorting,
                                        AfdelingID = afd.ID,
                                        AfdelingNaam = afd.Naam,
                                        GeboorteJaarTot = 0,
                                        GeboorteJaarVan = 0,
                                        Geslacht = GeslachtsType.Onbekend
                                    };

                if (act != null) // Er bestaat nu al een afdelingsjaar voor, dus we kunnen meer info inladen
                {
                    afddetail.OfficieleAfdelingNaam = act.OfficieleAfdelingNaam;
                    afddetail.Geslacht = act.Geslacht;

                    // TODO dit is niet correct als het laatste actieve werkjaar niet een jaar geleden was
                    afddetail.GeboorteJaarTot = act.GeboorteJaarTot + 1;
                    afddetail.GeboorteJaarVan = act.GeboorteJaarVan + 1;
                }

                volledigelijst.Add(afddetail);
            }

            // Sorteer de afdelingsjaren: eerst die zonder gegevens, dan van ribbels naar aspiranten
            model2.Afdelingen = (from a in volledigelijst
                                 orderby a.GeboorteJaarTot descending
                                 orderby a.GeboorteJaarTot == 0 descending
                                 select a);

            // TODO extra info pagina voor jaarovergang
            // TODO kan validatie in de listhelper worden bijgecodeerd?
            // TODO foutmeldingen

            var nieuwewerkjaar = ServiceHelper.CallService<IGroepenService, int>(g => g.NieuwWerkJaarOphalen());

            model2.NieuwWerkjaar = nieuwewerkjaar;
            model2.Titel = "Jaarovergang stap 2:  instellingen van je afdelingen";
            return View("AfdelingenVerdelen", model2);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VerdelingMaken(JaarOvergangAfdelingsJaarModel model, int groepID)
        {
            var teactiveren = new List<TeActiverenAfdelingInfo>();

            if (model.AfdelingsIDs.Count != model.TotLijst.Count || model.TotLijst.Count != model.GeslLijst.Count || model.TotLijst.Count != model.VanLijst.Count)
            {
                TempData["fout"] = "Niet alle informatie is ingevuld, gelieve aan te vullen.";
                return View("AfdelingenVerdelen", model);
            }

            if (model.TotLijst.Any(e => e.Equals(string.Empty)) || model.VanLijst.Any(e => e.Equals(string.Empty)))
            {
                TempData["fout"] = "Niet alle informatie is ingevuld, gelieve aan te vullen.";
                return View("AfdelingenVerdelen", model);
            }

            // Probleem: is er de garantie dat de volgorde bewaard blijft? (want we zijn per record de ID kwijt natuurlijk)

            // ReSharper disable LoopCanBeConvertedToQuery
            for (int i = 0; i < model.VanLijst.Count; i++)
            {
                var x = new TeActiverenAfdelingInfo
                            {
                                AfdelingID = Int32.Parse(model.AfdelingsIDs[i]),
                                OfficieleAfdelingID = Int32.Parse(model.OfficieleAfdelingsIDs[i]),
                                GeboorteJaarTot = Int32.Parse(model.TotLijst[i]),
                                GeboorteJaarVan = Int32.Parse(model.VanLijst[i]),
                                Geslacht = (GeslachtsType)Int32.Parse(model.GeslLijst[i])
                            };

                teactiveren.Add(x);
            }
            // ReSharper restore LoopCanBeConvertedToQuery

            string foutberichten = String.Empty;
            try
            {
                ServiceHelper.CallService<IGroepenService>(s => s.JaarovergangUitvoeren(teactiveren, groepID, out foutberichten));
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                TempData["fout"] = ex.Message;
                /*model.OfficieleAfdelingen =
                ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(
                    e => e.OfficieleAfdelingenOphalen(groepID));*/
                return View("AfdelingenVerdelen", model);
            }

            if (!String.IsNullOrEmpty(foutberichten))
            {
                TempData["fout"] = foutberichten;
            }

            VeelGebruikt.FunctieProblemenResetten(groepID);

            return RedirectToAction("Index", "Leden");
        }

        /// <summary>
        /// Toont de view die toelaat een nieuwe afdeling te maken.
        /// </summary>
        /// <param name="groepID">Groep waarvoor de afdeling gemaakt moet worden</param>
        /// <returns>De view die toelaat een nieuwe afdeling te maken.</returns>
        public ActionResult AfdelingMaken(int groepID)
        {
            var model = new AfdelingInfoModel();

            BaseModelInit(model, groepID);

            model.Info = new AfdelingInfo();

            model.Titel = Properties.Resources.NieuweAfdelingTitel;
            return View("AfdelingMaken", model);
        }

        /// <summary>
        /// Maakt een nieuwe afdeling, op basis van <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Bevat naam en code voor de nieuwe afdeling</param>
        /// <param name="groepID">ID van de groep waarvoor de afdeling gemaakt moet worden</param>
        /// <returns>Het overzicht van de afdelingen, indien de nieuwe afdeling goed gemaakt is.
        /// In het andere geval opnieuw de view om een afdeling bij te maken.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AfdelingMaken(AfdelingInfoModel model, int groepID)
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
        /// <param name="groepID"></param>
        /// <param name="afdelingID"></param>
        /// <returns></returns>
        public ActionResult Bewerken(int groepID, int afdelingID)
        {
            var model = new AfdelingInfoModel();
            BaseModelInit(model, groepID);

            model.Info = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(afdelingID));

            model.Titel = "Afdeling bewerken";
            return View("AfdelingMaken", model);
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
                return View("AfdelingMaken", model);
            }
        }
    }
}
