// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;
using Chiro.Adf.ServiceModel;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor toegang tot groepsinstellingen
    /// </summary>
	[HandleError]
	public class GroepController : BaseController
	{
		/// <summary>
		/// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
		public GroepController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

		/// <summary>
		/// Genereert een view met algemene gegevens over de groep
		/// </summary>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>View met algemene gegevens over de groep</returns>
		[HandleError]
		public override ActionResult Index(int groepID)
		{
			var model = new GroepsInstellingenModel
							{
								Titel = Properties.Resources.GroepsInstellingenTitel,
								Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(svc => svc.DetailOphalen(groepID))
							};

			// Ook hier nakijken of we live zijn.
			model.IsLive = VeelGebruikt.IsLive();

			return View(model);
		}

        /// <summary>
        /// Laat de gebruiker de naam van de groep <paramref name="groepID"/> bewerken.
        /// </summary>
        /// <param name="groepID">ID van de geselecteerde groep</param>
        /// <returns>De view 'afdelingsinstellingen'</returns>
        [HandleError]
        public ActionResult NaamWijzigen(int groepID)
        {
            var model = new GroepsInstellingenModel
            {
                Titel = "Groepsnaam wijzigen",
                Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(svc => svc.DetailOphalen(groepID))
            };

            // Ook hier nakijken of we live zijn.
            model.IsLive = VeelGebruikt.IsLive();

            return View("NaamWijzigen", model);
        }

        /// <summary>
        /// Postback voor bewerken van de groepsnaam
        /// </summary>
        /// <param name="model">De property <c>model.AfdelingsJaar</c> bevat de relevante details over de groep</param>
        /// <param name="groepID">Groep waarin de gebruiker momenteel aan het werken is</param>
        /// <returns>De view 'afdelingsinstellingen'</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult NaamWijzigen(GroepInfoModel model, int groepID)
        {
            BaseModelInit(model, groepID);

            try
            {
                ServiceHelper.CallService<IGroepenService>(e => e.Bewaren(model.Info));

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

                return RedirectToAction("Index");
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                ModelState.AddModelError("fout", ex.Detail.Bericht);

                
                model.Titel = "Groepsnaam wijzigen";
                return View("NaamWijzigen", model);
            }
        }
	}
}
