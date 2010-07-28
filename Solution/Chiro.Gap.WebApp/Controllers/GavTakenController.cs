// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	[HandleError]
	public class GavTakenController : BaseController
	{
		public GavTakenController(IServiceHelper serviceHelper) : base(serviceHelper) { }

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