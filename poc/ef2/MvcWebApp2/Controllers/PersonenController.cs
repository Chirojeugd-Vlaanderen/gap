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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using Cg2.Orm;

namespace MvcWebApp2.Controllers
{
    public class PersonenController : Controller
    {
        //
        // GET: /Personen/

        public ActionResult Index()
        {
            IList<GelieerdePersoon> personen;

            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                int aantal;
                personen = service.PaginaOphalenMetLidInfo(out aantal, int.Parse(ConfigurationSettings.AppSettings["TestGroepID"]), 1, 12);
            }
            return View("Index", personen);
        }

        //
        // GET: /Personen/Details/5

        public ActionResult Details(int id)
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                GelieerdePersoon p = service.PersoonOphalenMetDetails(id);
                return View("Details", p);
            }
        }

        //
        // GET: /Personen/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Personen/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Personen/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                GelieerdePersoon p = service.PersoonOphalenMetDetails(id);
                return View("Edit", p);
            }
        }

        //
        // POST: /Personen/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(GelieerdePersoon p)
        {
            try
            {
                using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
                {
                    service.PersoonBewaren(p);
                }
 
                return RedirectToAction("Details", new { id = p.ID });
            }
            catch
            {
                return View("Edit" ,p);
            }
        }

        public ActionResult LidMaken(int id)
        {
            using (LedenServiceReference.LedenServiceClient service = new MvcWebApp2.LedenServiceReference.LedenServiceClient())
            {
                // Beter zou zijn:
                //  via de service de definitie van een lid ophalen
                //  een view tonen met die lidgegevens, zodat ze aangepast kunnen worden
                //  pas als de gebruiker dan bevestigt: bewaren

                service.LidMakenEnBewaren(id);
            }

            return RedirectToAction("Index");

        }

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
