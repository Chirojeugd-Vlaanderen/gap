using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public abstract class PersonenEnLedenController : BaseController
	{
		/// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
		protected PersonenEnLedenController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

		[HandleError]
		public abstract override ActionResult Index(int groepID);

		[HandleError]
		protected ActionResult GelieerdePersonenInschrijven(IEnumerable<int> gelieerdepersoonIDs)
		{
			TempData["list"] = gelieerdepersoonIDs;
			return RedirectToAction("LedenMaken", "Leden"); // TODO naar waar willen we terug?
		}

		public void GelieerdePersonenUitschrijven(IEnumerable<int> gelieerdepersoonIDs, int groepID, string succesboodschap)
		{
			var fouten = String.Empty; // TODO (#1035): fouten opvangen

			ServiceHelper.CallService<ILedenService>(l => l.Uitschrijven(gelieerdepersoonIDs, out fouten));

			// TODO (#1035): beter manier om problemen op te vangen dan via een string

			if (fouten == String.Empty)
			{
				TempData["succes"] = succesboodschap;

				VeelGebruikt.FunctieProblemenResetten(groepID);
			}
			else
			{
				// TODO (#1035): vermijden dat output van de back-end rechtstreeks zichtbaar wordt voor de user.
				TempData["fout"] = fouten;
			}
		}

		/// <summary>
		/// BELANGRIJK: in Tempdata "list" moet een lijst zitten van welke gelieerdepersonen moeten worden ingeschreven
		/// </summary>
		/// <param name="groepID"></param>
		/// <returns></returns>
		[HandleError]
		public ActionResult LedenMaken(int groepID)
		{
			// TODO exceptions
			var model = new GeselecteerdePersonenEnLedenModel();
			BaseModelInit(model, groepID);
			model.Titel = "Personen inschrijven in het huidige werkjaar";

			//TODO model laden, gegeven:
			object value;
			TempData.TryGetValue("list", out value);
			var gekozengelieerdepersoonsids = (List<int>)value;
			var foutBerichten = String.Empty;
			var personenOverzicht = ServiceHelper.CallService<ILedenService, IEnumerable<InTeSchrijvenLid>>(e => e.VoorstelTotInschrijvenGenereren(gekozengelieerdepersoonsids, out foutBerichten));
			if (!String.IsNullOrEmpty(foutBerichten))
			{
				TempData["fout"] = string.Concat(Properties.Resources.InschrijvenMisluktFout, Environment.NewLine, foutBerichten);
				return TerugNaarVorigeLijst();
			}

			foreach (var p in personenOverzicht)
			{
				model.PersoonEnLidInfos.Add(p);
			}

			// TODO "Geen" ook toevoegen en met ajax verwijderen uit de lijst als je lid selecteert
			model.BeschikbareAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<ActieveAfdelingInfo>>(svc => svc.HuidigeAfdelingsJarenOphalen(groepID));

			return View("LedenMaken", model);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="groepID"></param>
		/// <returns></returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult LedenMaken(GeselecteerdePersonenEnLedenModel model, int groepID)
		{
			var foutBerichten = String.Empty;
			var lijst = new List<InTeSchrijvenLid>();
			for(var i=0; i<model.GelieerdePersoonIDs.Count; i++)
			{
				var gelieerdePersoonID = model.GelieerdePersoonIDs[i];
				if(model.InTeSchrijvenGelieerdePersoonIDs.Contains(gelieerdePersoonID))
				{
					lijst.Add(new InTeSchrijvenLid() { GelieerdePersoonID = gelieerdePersoonID, AfdelingsJaarIDs = new []{model.ToegekendeAfdelingsJaarIDs[i]}, LeidingMaken = model.LeidingMakenGelieerdePersoonIDs.Contains(gelieerdePersoonID), AfdelingsJaarIrrelevant = false});
				}
			}

			ServiceHelper.CallService<ILedenService, IEnumerable<int>>(l => l.Inschrijven(lijst, out foutBerichten));
			if (String.IsNullOrEmpty(foutBerichten))
			{
				TempData["succes"] = Properties.Resources.IngeschrevenFeedback;

				// Als een persoon wordt aangemaakt, worden best de ledenproblemen gereset (want die persoon zal bvb geen adres hebben).
				VeelGebruikt.LedenProblemenResetten(groepID);
			}
			else
			{
				TempData["fout"] = string.Concat(Properties.Resources.InschrijvenMisluktFout, Environment.NewLine, foutBerichten);
			}

			return TerugNaarVorigeLijst();
		}
	}
}