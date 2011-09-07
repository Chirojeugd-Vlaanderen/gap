// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web.Mvc;

using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller die de verschillende pagina's van de handleiding toont
    /// </summary>
    [HandleError]
    public class HandleidingController : BaseController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public HandleidingController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

        [HandleError]
        public override ActionResult Index(int groepID)
        {
            var model = new HandleidingModel();
            BaseModelInit(model, groepID, "Handleiding");
            return View(model);
        }

        [HandleError]
        public ActionResult ViewTonen(int? groepID, string helpBestand)
        {
            var model = new HandleidingModel { Titel = "Handleiding" };

            if (groepID != null && groepID > 0)
            {
                BaseModelInit(model, (int)groepID);
            }
            else
            {
                model.GroepID = 0;
            }
            return View(helpBestand, "Handleiding", model);
        }
    }
}