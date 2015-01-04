/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Web.Mvc;

using Chiro.Gap.WebApp.Controllers;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.ActionFilters
{
	/// <summary>
	/// Zorgt ervoor dat controllers de gegevens van het MasterViewModel doorgeven aan
	/// het overervend model
	/// </summary>
	/// <remarks>
	/// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc
	/// Meer info: http://www.davidhayden.com/blog/dave/archive/2008/03/21/ActionFilterAttributeExamplesASPNETMVCFrameworkPreview2.aspx
	/// </remarks>
	public class MasterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);

			// Controleer of we iets moeten tonen of dat we geredirect worden. Bij Redirect mogen we verder doen,
			// als we iets moeten tonen, moeten we nog iets doorgeven van de MasterViewModel.
			if (filterContext.Result as ViewResultBase != null)
			{
				var viewModel = (IMasterViewModel)((ViewResultBase)filterContext.Result).ViewData.Model;
				var controller = (BaseController)filterContext.Controller;
				// controller.SetModel(viewModel);
				// Dat de regel hiervoor in comments staat, doet vermoeden dat dit hele bestand niet meer nodig is.
				// Of toch alleszins deze if-constructie niet.
			}
		}
	}
}