// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
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

        /// <summary>
        /// Schrijft via de backend de gelieerde personen met gegeven <paramref name="gelieerdePersoonIDs"/> uit
        /// uit de groep met gegeven <paramref name="groepID"/>.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's uit te schrijven gelieerde personen</param>
        /// <param name="groepID">ID van groep waarvoor uit te schrijven</param>
        /// <param name="succesboodschap">Feedback die gegeven zal worden bij succes</param>
		protected void GelieerdePersonenUitschrijven(IEnumerable<int> gelieerdePersoonIDs, int groepID, string succesboodschap)
		{
            // Ik vind het een beetje vreemd dat het succesbericht hier een parameter is.
            
			var fouten = String.Empty; // TODO (#1035): fouten opvangen

			ServiceHelper.CallService<ILedenService>(l => l.Uitschrijven(gelieerdePersoonIDs, out fouten));

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
		/// Dit is een louche actie.  Ze kijkt in TempData["list"], en hoopt dat daar een List van gelieerdePersoonID's in
		/// zit.  Dan wordt er een suggestie gedaan voor een inschrijving van die gelieerde personen, in die zin
		/// dat er voorgesteld wordt of ze lid of leiding worden, en in welke afdeling ze terechtkomen.
		/// </summary>
		/// <param name="groepID">Groep waar de leden ingeschreven moeten worden</param>
		/// <returns>De view met de voorgestelde afdelingen</returns>
		[HandleError]
		public ActionResult LedenMaken(int groepID)
		{
			// TODO exceptions
			var model = new GeselecteerdePersonenEnLedenModel();
			BaseModelInit(model, groepID);
			model.Titel = "Personen inschrijven in het huidige werkjaar";

			// TODO model laden, gegeven:
			object value;
			TempData.TryGetValue("list", out value);
			var gekozengelieerdepersoonsids = value as List<int>;

			var foutBerichten = String.Empty;

            var personenOverzicht =
		        ServiceHelper.CallService<ILedenService, IEnumerable<InTeSchrijvenLid>>(
		            e => e.VoorstelTotInschrijvenGenereren(gekozengelieerdepersoonsids, out foutBerichten));

			if (!String.IsNullOrEmpty(foutBerichten))
			{
				TempData["fout"] = string.Concat(Properties.Resources.InschrijvenMisluktFout, Environment.NewLine, foutBerichten);
				return TerugNaarVorigeLijst();
			}

		    model.PersoonEnLidInfos = (from p in personenOverzicht
		                               select new InschrijfbaarLid
		                                          {
		                                              AfdelingsJaarIDs = p.AfdelingsJaarIDs,
		                                              AfdelingsJaarIrrelevant = false,
		                                              GelieerdePersoonID = p.GelieerdePersoonID,
		                                              InTeSchrijven = true,
		                                              LeidingMaken = p.LeidingMaken,
		                                              VolledigeNaam = p.VolledigeNaam
		                                          }).ToArray();

			// TODO "Geen" ook toevoegen en met ajax verwijderen uit de lijst als je lid selecteert
			model.BeschikbareAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<ActieveAfdelingInfo>>(svc => svc.HuidigeAfdelingsJarenOphalen(groepID));

			return View("LedenMaken", model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult LedenMaken(GeselecteerdePersonenEnLedenModel model, int groepID)
		{
			var foutBerichten = String.Empty;

            // Bekijk enkel de rijen waar 'in te schrijven' is aangevinkt.
		    var inTeSchrijven = (from rij in model.PersoonEnLidInfos
		                         where rij.InTeSchrijven
		                         select new InTeSchrijvenLid
		                                    {
		                                        AfdelingsJaarIDs = rij.AfdelingsJaarIDs,
                                                AfdelingsJaarIrrelevant = false,
                                                GelieerdePersoonID = rij.GelieerdePersoonID,
                                                LeidingMaken = rij.LeidingMaken
		                                    }).ToArray();
            // model.PersoonEnLidInfo is van het type InschrijfbaarLid[].  Als ik deze cast
            // naar InTeSchrijvenLid, krijg ik een exception ivm datacontracts.  Vandaar dat
            // ik kloon ipv cast.

            // De user interface is zodanig gemaakt dat AfdelingsJaarIDs precies 1 ID bevat, dat
            // 0 kan zijn als het over 'geen' gaat.  Dat is niet logisch.  Maar wel de praktijk.
            // De service verwacht een lijst met AfdelingsJaarIDs.  Ik ga dus alle lijsten die
            // enkel 0 bevatten, vervangen door lege lijsten.

            foreach (var pli in inTeSchrijven.Where(pli => pli.AfdelingsJaarIDs.Count() == 1 && pli.AfdelingsJaarIDs.First() == 0))
            {
                pli.AfdelingsJaarIDs = new int[0];  // lege array van ints
            }

			ServiceHelper.CallService<ILedenService, IEnumerable<int>>(l => l.Inschrijven(inTeSchrijven, out foutBerichten));
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