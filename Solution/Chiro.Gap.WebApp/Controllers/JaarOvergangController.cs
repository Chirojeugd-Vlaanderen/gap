using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public class JaarOvergangController : BaseController
    {
        //
        // GET: /JaarOvergang/

    	public JaarOvergangController(IServiceHelper serviceHelper) : base(serviceHelper)
    	{
    	}

		public ActionResult AfdelingenGemaakt()
		{
			return null;
		}

    	public override ActionResult Index(int groepID)
    	{
			var model = new JaarOvergangAfdelingsModel();
			BaseModelInit(model, groepID);

			IEnumerable<AfdelingInfo> lijst = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingInfo>>(g => g.BeschikbareAfdelingenOphalen(groepID));

			model.Afdelingen = lijst;

			return View("AfdelingenAanmaken", model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AfdelingenGemaakt(JaarOvergangAfdelingsModel model1, int groepID)
		{
			var model2 = new JaarOvergangAfdelingsJaarModel();
			BaseModelInit(model2, groepID);

			model2.OfficieleAfdelingen =
				ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(
					e => e.OfficieleAfdelingenOphalen(groepID));

			IEnumerable<AfdelingInfo> lijst = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingInfo>>(g => g.BeschikbareAfdelingenOphalen(groepID));

			int werkjaarID = ServiceHelper.CallService<IGroepenService, int>(g => g.RecentsteGroepsWerkJaarIDGet(groepID));

			IEnumerable<AfdelingDetail> actievelijst = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingDetail>>(g => g.ActieveAfdelingenOphalen(werkjaarID));

			//actieve info inladen
			var volledigelijst = new List<AfdelingDetail>();
			foreach(var afd in lijst)
			{
				if(!model1.GekozenAfdelingsIDs.Contains(afd.ID))
				{
					continue;
				}

				var act = (from actafd in actievelijst
							where actafd.AfdelingID == afd.ID
							select actafd).FirstOrDefault();

				if(act!=null)
				{
					//TODO geboortejaren aanpassen aan nieuwe jaar (vergelijken met huidige werkjaar)
					volledigelijst.Add(new AfdelingDetail
					                   	{
					                   		AfdelingAfkorting = afd.Afkorting,
					                   		AfdelingID = afd.ID,
					                   		AfdelingNaam = afd.Naam,
											OfficieleAfdelingNaam = act.OfficieleAfdelingNaam,
					                   		GeboorteJaarTot = act.GeboorteJaarTot,
					                   		GeboorteJaarVan = act.GeboorteJaarVan,
					                   		Geslacht = act.Geslacht
					                   	});
				}else
				{
					volledigelijst.Add(new AfdelingDetail
					                   	{
					                   		AfdelingAfkorting = afd.Afkorting,
					                   		AfdelingID = afd.ID,
					                   		AfdelingNaam = afd.Naam,
					                   		GeboorteJaarTot = 0,
					                   		GeboorteJaarVan = 0,
					                   		Geslacht = GeslachtsType.Onbekend
					                   	});
				}
			}

			model2.Afdelingen = volledigelijst;

			//TODO leden verwijderen voor 15 okt moet het lid-object VERWIJDEREN, niet non-actief maken!
			//TODO extra info paginas voor en confirmatiepagina na deze
			//TODO andere MASTER pagina, zodat ze niet weg kunnen klikken
			//TODO volgende - vorige links
			//TODO kan validatie in de listhelper worden bijgecodeerd?
			//TODO foutmeldingen

			//Temp om eens te testen
			//model2.Afdelingen.Add(new AfdelingInfo { ID = 10, Afkorting = "BC", Naam = "Broes", OfficieleAfdelingNaam = "Broesen" });
			//model2.Afdelingen.Add(new AfdelingInfo { ID = 5, Afkorting = "EV", Naam = "Ellen", OfficieleAfdelingNaam = "Ellens" });

			return View("AfdelingenVerdelen", model2);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void VerdelingGemaakt(JaarOvergangAfdelingsJaarModel model, int groepID)
		{
			var teactiveren = new List<TeActiverenAfdeling>();

			if(model.TotLijst.Count != model.GeslLijst.Count || model.TotLijst.Count != model.VanLijst.Count)
			{
				//TODO
				throw new NotImplementedException();
			}

			//Probleem: is er de garantie dat de volgorde bewaard blijft? (want we zijn per record de ID kwijt natuurlijk

			for (int i = 0; i < model.VanLijst.Count; i++)
			{
				var x = new TeActiverenAfdeling
				        	{
				        		AfdelingID = Int32.Parse(model.AfdelingsIDs[i]),
								OfficieleAfdelingID = Int32.Parse(model.OfficieleAfdelingsIDs[i]),
				        		GeboorteJaarTot = Int32.Parse(model.TotLijst[i]),
				        		GeboorteJaarVan = Int32.Parse(model.VanLijst[i])
				        	};

				teactiveren.Add(x);
			}

			ServiceHelper.CallService<IGroepenService>(s => s.JaarovergangUitvoeren(teactiveren, groepID));
		}

		public ActionResult AfdelingAanpassen(int afdelingID)
		{
			throw new NotImplementedException();
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

			if(!ModelState.IsValid)
			{
				// Modelstate bevat ongeldige waarden; toon de pagina opnieuw
				return View(model);
			}

			try
			{
				ServiceHelper.CallService<IGroepenService>(e => e.AfdelingAanmaken(groepID, model.Info.Naam, model.Info.Afkorting));

				TempData["feedback"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

				return RedirectToAction("Index");
			}
			catch (FaultException<BestaatAlFault<AfdelingInfo>> ex)
			{
				if (string.Compare(ex.Detail.Bestaande.Afkorting,model.Info.Afkorting,true) == 0)
				{
					ModelState.AddModelError(
						"Info.Afkorting",
						string.Format(Properties.Resources.AfdelingsCodeBestaatAl,ex.Detail.Bestaande.Afkorting,ex.Detail.Bestaande.Naam));
				}
				else if (string.Compare(ex.Detail.Bestaande.Naam,model.Info.Naam,true) == 0)
				{
					ModelState.AddModelError(
						"Info.Naam",
						string.Format(
							Properties.Resources.AfdelingsNaamBestaatAl,ex.Detail.Bestaande.Afkorting,ex.Detail.Bestaande.Naam));
				}
				else
				{
					//Mag niet gebeuren
					ModelState.AddModelError("Er heeft zich een fout voorgedaan, gelieve contact op te nemen met het secretariaat.", ex);
					Debug.Assert(false);
				}

				return View(model);
			}
		}

		public ActionResult Bewerken(int groepID, int id)
		{
			var model = new AfdelingInfoModel();
			BaseModelInit(model, groepID);

			model.Info = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(
				svc => svc.AfdelingOphalen(id));

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

				TempData["feedback"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				TempData["feedback"] = ex.Message;

				model.Info = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(model.Info.ID));

				model.Titel = "Afdeling bewerken";
				return View("AfdelingMaken", model);
			}
		}
    }
}
