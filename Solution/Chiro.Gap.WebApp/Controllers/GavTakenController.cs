// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor alles wat te maken heeft met de onafgewerkte taken van de groep
    /// </summary>
	[HandleError]
	public class GavTakenController : BaseController
	{
		/// <summary>
		/// Standaardconstructor.  <paramref name="serviceHelper"/> en <paramref name="veelGebruikt"/> worden
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="serviceHelper">Wordt gebruikt om de webservices van de backend aan te spreken</param>
		/// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
		public GavTakenController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt) : base(serviceHelper, veelGebruikt) { }

		[HandleError]
		public override ActionResult Index(int groepID)
		{
			var model = new GavTakenModel();
			BaseModelInit(model, groepID);
			model.Titel = "Taken voor de groepsadministratieverantwoordelijke (GAV)";
			return View(model);
		}
	}
}