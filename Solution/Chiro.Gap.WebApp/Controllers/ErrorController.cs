// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Net;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.WebApp.Controllers
{
	public class ErrorController : Controller
	{
		//
		// GET: /Error/
		public ActionResult Index()
		{
			Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			return View();
		}

		public ActionResult Index(ExceptionContext exceptionContext)
		{
			if (exceptionContext.Exception.GetType() == typeof(FaultException<FoutNummerFault>))
			{
				var fault = (FaultException<FoutNummerFault>)exceptionContext.Exception;
				if (fault.Detail.FoutNummer == FoutNummer.GeenDatabaseVerbinding)
				{
					return View("GeenVerbinding");
				}
				else
				{
					return View();
				}
			}
			else
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return View();
			}
		}

		//
		// GET: /Error/NietGevonden
		public ActionResult NietGevonden()
		{
			Response.StatusCode = (int)HttpStatusCode.NotFound;
			return View("Index");
		}

		public ActionResult GeenVerbinding()
		{
			Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			return View("GeenVerbinding");
		}
	}
}
