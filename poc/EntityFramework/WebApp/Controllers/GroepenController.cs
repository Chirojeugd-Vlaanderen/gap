using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Cg2.Core.Domain;

namespace WebApp.Controllers
{
    public class GroepenController : Controller
    {
        public ActionResult Tonen(int groepID) 
        {
            Groep g;
            using (var client = new CgServiceReference.GroepenServiceClient())
            {
                g = client.Ophalen(groepID);
            }
            return View(g);
        }
        public void Updaten() { }
    }
}
