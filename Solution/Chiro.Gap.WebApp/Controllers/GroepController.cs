// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web.Mvc;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
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
								Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
									svc => svc.DetailOphalen(groepID))
							};

			// Ook hier nakijken of we live zijn.
			model.IsLive = VeelGebruikt.IsLive();

			return View(model);
		}
	}
}
