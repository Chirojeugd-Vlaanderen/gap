using System;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	/// <summary>
	/// Controller die voorlopig voor niet veel meer dan Dubbelpunt gebruikt zal worden.
	/// </summary>
	public class AbonnementenController : BaseController
	{
		public AbonnementenController(IServiceHelper serviceHelper) : base(serviceHelper)
		{
		}

		public override ActionResult Index(int groepID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Vraag de gebruiker bevestiging voor het bestellen van een dubbelpuntabonnement voor de persoon
		/// gekoppeld aan de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>
		/// </summary>
		/// <param name="groepID">Groep waarn de gebruiker aan het werken is</param>
		/// <param name="id">ID van een gelieerde persoon</param>
		/// <returns>Een view die de gebruiker toelaat te bevestigen</returns>
		public ActionResult DubbelPuntAanvragen(int groepID, int id)
		{
			var model = new BevestigingsModel();
			BaseModelInit(model, groepID);

			// TODO: DetailsOphalen is overkill, aangezien ik enkel de volledige naam nodig heb.
			// OPM: als je de gebruiker wilt waarschuwen dat de kandidaat-abonnee eigenlijk te jong is om in aanmerking te komen,
			//		heb je wel de geboortedatum of de leeftijd nodig

			var info = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(svc => svc.DetailsOphalen(id));

			model.LidID = 0;	// LidID is hier niet relevant

			model.GelieerdePersoonID = info.GelieerdePersoonID;
			model.VolledigeNaam = info.VolledigeNaam;
			model.Prijs = Properties.Settings.Default.PrijsDubbelPunt;
			model.Titel = String.Format("Dubbelpunt bestellen voor {0}", model.VolledigeNaam);

			return View(model);			
		}

		/// <summary>
		/// Bestelt een Dubbelpuntabonnement voor de persoon met GelieerdePersoonID <paramref name="id"/>.
		/// </summary>
		/// <param name="model">Een BevestigingsModel, puur pro forma, want alle relevante info zit in de url</param>
		/// <param name="groepID">ID van de groep waarin wordt gewerkt</param>
		/// <param name="id">GelieerdePersoonID van persoon die Dubbelpunt wil</param>
		/// <returns>Redirect naar detailfiche van de betreffende persoon</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult DubbelPuntAanvragen(BevestigingsModel model, int groepID, int id)
		{
			ServiceHelper.CallService<IGelieerdePersonenService>(svc => svc.DubbelPuntBestellen(id));
			return RedirectToAction("EditRest", "Personen", new { id });
		}

	}
}