// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

// met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc
// meer info: http://www.davidhayden.com/blog/dave/archive/2008/03/21/ActionFilterAttributeExamplesASPNETMVCFrameworkPreview2.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chiro.Gap.WebApp.Controllers;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Zorgt ervoor dat controllers de gegevens van het MasterViewModel doorgeven aan
	/// het overervend model
	/// </summary>
	public class MasterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);

			// Controleer of we iets moeten tonen of dat we geredirect worden. Bij Redirect mogen we verder doen,
			// als we iets moeten tonen, moeten we nog iets doorgeven van de MasterViewModel.
			if (filterContext.Result as ViewResultBase != null)
			{
				IMasterViewModel viewModel = (IMasterViewModel)((ViewResultBase)filterContext.Result).ViewData.Model;

				BaseController controller = (BaseController)filterContext.Controller;
				// Controller.SetModel(viewModel);
			}
		}
	}
}