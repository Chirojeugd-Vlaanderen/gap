using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public abstract class PersonenEnLedenController : BaseController
	{
		/// <summary>
        /// Standaardconstructor.  <paramref name="serviceHelper"/> en <paramref name="veelGebruikt"/> worden
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="serviceHelper">wordt gebruikt om de webservices van de backend aan te spreken</param>
        /// <param name="veelGebruikt">haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
		protected PersonenEnLedenController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt) : base(serviceHelper, veelGebruikt) { }

		[HandleError]
		public abstract override ActionResult Index(int groepID);

		/// <summary>
		/// 
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
				int gelieerdePersoonID = model.GelieerdePersoonIDs[i];
				if(model.InTeSchrijvenGelieerdePersoonIDs.Contains(gelieerdePersoonID))
				{
					lijst.Add(new InTeSchrijvenLid() { GelieerdePersoonID = gelieerdePersoonID, AfdelingsJaarID = model.ToegekendeAfdelingsJaarIDs[i], LeidingMaken = model.LeidingMakenGelieerdePersoonIDs.Contains(gelieerdePersoonID)} );
				}
			}

			ServiceHelper.CallService<ILedenService, IEnumerable<int>>(l => l.Inschrijven(lijst, out foutBerichten));
			if (String.IsNullOrEmpty(foutBerichten))
			{
				TempData["succes"] = Properties.Resources.IngeschrevenFeedback;
			}
			else
			{
				TempData["fout"] = string.Concat(Properties.Resources.InschrijvenMisluktFout, Environment.NewLine, foutBerichten);
			}

			return TerugNaarVorigeLijst();
		}
	}
}