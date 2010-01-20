using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Chiro.Gap.WebApp.Models;
using Chiro.Gap.Orm;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Controllers
{
	public class GroepController : BaseController
	{
		/// <summary>
		/// Genereert een view met algemene gegevens over de groep
		/// </summary>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>view met algemene gegevens over de groep</returns>
		public ActionResult Index(int groepID)
		{
			GroepsInfoModel model = new GroepsInfoModel();

			model.Info = ServiceHelper.CallService<IGroepenService, GroepInfo>(svc => svc.Ophalen(
				groepID, 
				GroepsExtras.AfdelingenHuidigWerkJaar | GroepsExtras.Categorieen));

			return View(model);
		}

	}
}
