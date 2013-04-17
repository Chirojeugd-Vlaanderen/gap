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

using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Deze controller voorziet de acties om afdelingen en afdelingsjaren te
    /// maken. te wijzigen, te verwijderen.
    /// </summary>
    /// <remarks>Het koppelen van leden aan afdelingen gebeurt hier niet, dat zit in de
    /// <see cref="LedenController" />.</remarks>
	[HandleError]
	public class AfdelingenController : BaseController
	{
		/// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
		public AfdelingenController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

		/// <summary>
        /// Toont het afdelingsoverzicht voor het huidge groepswerkjaar: de actieve afdelingen, 
        /// met links om dien te bekijken/bewerken.  De inactieve afdelingen worden ook getoond, 
        /// met dan de mogelijkheid  om ze te activeren.
        /// </summary>
        /// <param name="groepID">ID van de groep die de pagina oproept, en van dewelke we dus gegevens moeten tonen</param>
        /// <returns>Het afdelingsoverzicht voor het huidige werkJaar</returns>
		[HandleError]
		public override ActionResult Index(int groepID)
		{
			return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), groepID);
		}

        /// <summary>
        /// Toont het afdelingsoverzicht voor het groepswerkjaar met gegeven <paramref name="groepsWerkJaarID"/>:
        /// de actieve afdelingen,  met links om dien te bekijken/bewerken.  De inactieve afdelingen worden ook 
        /// getoond, met dan de mogelijkheid  om ze te activeren.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor de gebruiker de afdelingen wil zien</param>
		/// <param name="groepID">ID van de groep waarin de gebruiker aan het werken is.</param>
		/// <returns>Het afdelingsoverzicht voor het groepswerkjaar met ID <paramref name="groepsWerkJaarID"/></returns>
        [HandleError]
		public ActionResult List(int groepsWerkJaarID, int groepID)
		{
			var model = new AfdelingsOverzichtModel();
			BaseModelInit(model, groepID);

			// AfdelingDetails voor Afdelingen die in het opgegeven werkJaar voorkomen als AfdelingsJaar
			model.Actief =
				ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>
				(groep => groep.ActieveAfdelingenOphalen(groepsWerkJaarID));

			// AfdelingDetails voor Afdelingen die in het opgegeven werkJaar voorkomen als AfdelingsJaar

			model.NietActief
				= ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>(svc => svc.OngebruikteAfdelingenOphalen(groepsWerkJaarID));

			model.Titel = "Afdelingen";
			return View("Index", model);
		}

		/// <summary>
		/// Toont de view die toelaat een nieuwe afdeling te maken.
		/// </summary>
		/// <param name="groepID">Groep waarvoor de afdeling gemaakt moet worden</param>
		/// <returns>De view die toelaat een nieuwe afdeling te maken.</returns>
		/// <!-- GET: /Afdeling/Nieuw/ -->
		[HandleError]
		public ActionResult Nieuw(int groepID)
		{
			var model = new AfdelingInfoModel();

			BaseModelInit(model, groepID);

			model.Info = new AfdelingInfo();

			model.Titel = Properties.Resources.NieuweAfdelingTitel; 
			return View("Nieuw", model);
		}

		/// <summary>
		/// Maakt een nieuwe afdeling, op basis van <paramref name="model"/>.
		/// </summary>
		/// <param name="model">Bevat naam en code voor de nieuwe afdeling</param>
		/// <param name="groepID">ID van de groep waarvoor de afdeling gemaakt moet worden</param>
		/// <returns>Het overzicht van de afdelingen, indien de nieuwe afdeling goed gemaakt is.
		/// In het andere geval opnieuw de view om een afdeling bij te maken.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult Nieuw(AfdelingInfoModel model, int groepID)
		{
			model.Titel = Properties.Resources.NieuweAfdelingTitel;
			BaseModelInit(model, groepID);

			if (ModelState.IsValid)
			{
				try
				{
					ServiceHelper.CallService<IGroepenService>(e => e.AfdelingAanmaken(groepID, model.Info.Naam, model.Info.Afkorting));

					TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

					// (er wordt hier geredirect ipv de view te tonen,
					// zodat je bij een 'refresh' niet de vraag krijgt
					// of je de gegevens opnieuw wil posten.)
					return RedirectToAction("Index");
				}
				catch (FaultException<BestaatAlFault<AfdelingInfo>> ex)
				{
					if (string.Compare(
						ex.Detail.Bestaande.Afkorting,
						model.Info.Afkorting,
						true) == 0)
					{
						ModelState.AddModelError(
							"Info.Afkorting",
							string.Format(
								Properties.Resources.AfdelingsCodeBestaatAl,
								ex.Detail.Bestaande.Afkorting,
								ex.Detail.Bestaande.Naam));
					}
					else if (string.Compare(
						ex.Detail.Bestaande.Naam,
						model.Info.Naam,
						true) == 0)
					{
						ModelState.AddModelError(
							"Info.Naam",
							string.Format(
								Properties.Resources.AfdelingsNaamBestaatAl,
								ex.Detail.Bestaande.Afkorting,
								ex.Detail.Bestaande.Naam));
					}
					else
					{
						// Dit kan niet.
						Debug.Assert(false);
					}

					return View(model);
				}
			}
			else
			{
				// Modelstate bevat ongeldige waarden; toon de pagina opnieuw

				return View(model);
			}
		}

        /// <summary>
        /// Verwijdert een afdeling uit het lijstje van actieve afdelingen in een bepaald werkJaar.
        /// </summary>
        /// <param name="groepID">ID van de groep die we aan het bewerken zijn</param>
        /// <param name="id">ID van het afdelingsjaar dat we willen verwijderen</param>
        /// <returns></returns>
        /// <!-- GET: /Afdeling/VerwijderenVerwijderenVanWerkjaar/afdelingsJaarId -->
		[HandleError]
        public ActionResult VerwijderenVanWerkjaar(int groepID, int id)
		{
			// Afdeling van afdelingsjaar invullen
			try
			{
				ServiceHelper.CallService<IGroepenService>(groep => groep.AfdelingsJaarVerwijderen(id));

				TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
			}
			catch (FaultException)
			{
				TempData["fout"] = Properties.Resources.AfdelingNietLeeg;
                // TODO (#1139): specifieke exceptions catchen en weergeven via de modelstate, en niet via tempdata.
			}

			return RedirectToAction("Index");
		}

        /// <summary>
        /// Verwijdert een afdeling volledig uit de database.
        /// </summary>
        /// <param name="groepID">ID van de groep die we aan het bewerken zijn</param>
        /// <param name="id">ID van de afdeling dat we willen verwijderen</param>
        /// <returns></returns>
        /// <!-- GET: /Afdeling/Verwijderen/afdelingsId -->
        [HandleError]
        public ActionResult Verwijderen(int groepID, int id)
        {
            // Afdeling van afdelingsjaar invullen
            try
            {
                ServiceHelper.CallService<IGroepenService>(groep => groep.AfdelingVerwijderen(id));

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
            }
            catch (FaultException)
            {
                TempData["fout"] = Properties.Resources.AfdelingNietLeeg;
                // TODO (#1139): specifieke exceptions catchen en weergeven via de modelstate, en niet via tempdata.
            }

            return RedirectToAction("Index");
        }

		/// <summary>
		/// Laat de gebruiker een nieuw afdelingsjaar maken voor een niet-actieve afdeling
		/// (met AfdelingID <paramref name="id"/>)
		/// </summary>
		/// <param name="groepID">ID van de geselecteerde groep</param>
		/// <param name="id">AfdelingID van de te activeren afdeling</param>
		/// <returns>De view 'afdelingsjaar'</returns>
		[HandleError]
		public ActionResult Activeren(int groepID, int id)
		{
			var model = new AfdelingsJaarModel();
			BaseModelInit(model, groepID);

			model.OfficieleAfdelingen =
				ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>
				(groep => groep.OfficieleAfdelingenOphalen());

			model.Afdeling = ServiceHelper.CallService<IGroepenService, AfdelingInfo>
				(groep => groep.AfdelingOphalen(id));

			model.AfdelingsJaar = new AfdelingsJaarDetail();
			model.AfdelingsJaar.AfdelingID = model.Afdeling.ID;

			model.Titel = "Afdeling activeren";
			return View("AfdelingsJaar", model);
		}

		/// <summary>
		/// Laat de gebruiker het bestaande afdelingsjaar met afdelingsjaarID <paramref name="id"/>
		/// bewerken.
		/// </summary>
		/// <param name="groepID">ID van de geselecteerde groep</param>
		/// <param name="id">ID van het te bewerken afdelingsjaar</param>
		/// <returns>De view 'afdelingsjaar'</returns>
		[HandleError]
		public ActionResult AfdJaarBewerken(int groepID, int id)
		{
			var model = new AfdelingsJaarModel();
			BaseModelInit(model, groepID);

			AfdelingDetail detail = ServiceHelper.CallService<IGroepenService, AfdelingDetail>(svc => svc.AfdelingDetailOphalen(id));

			model.Afdeling = new AfdelingInfo
			{
				ID = detail.AfdelingID,
				Naam = detail.AfdelingNaam,
				Afkorting = detail.AfdelingAfkorting
			};

			model.AfdelingsJaar = detail; // inheritance :)
			model.OfficieleAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(groep => groep.OfficieleAfdelingenOphalen());

			model.Titel = "Afdeling bewerken";
			return View("AfdelingsJaar", model);
		}

		/// <summary>
		/// Postback voor activeren/bewerken afdelingsjaar
		/// </summary>
		/// <param name="model">De property <c>model.AfdelingsJaar</c> bevat de relevante details over het afdelingsjaar</param>
		/// <param name="groepID">Groep waarin de gebruiker momenteel aan het werken is</param>
		/// <returns>Het afdelingsoverzicht als de wijzigingen bewaard zijn, en anders opnieuw de
		/// 'AfdelingsJaarView'.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult AfdJaarBewerken(AfdelingsJaarModel model, int groepID)
		{
			BaseModelInit(model, groepID);

			// Als de gebruiker een kleiner geboortejaar 'tot' als 'van' ingeeft, wisselen we die stiekem om.  (Ticket #289)

			if (model.AfdelingsJaar.GeboorteJaarTot < model.AfdelingsJaar.GeboorteJaarVan)
			{
				model.AfdelingsJaar.GeboorteJaarVan = model.AfdelingsJaar.GeboorteJaarTot;
                model.AfdelingsJaar.GeboorteJaarTot = model.AfdelingsJaar.GeboorteJaarVan;
			}

			try
			{
                // de view 'AfdJaarBewerken' laat zowel toe de naam en afkorting
                // van de afdeling aan te passen, als de geboortejaren, geslacht en
                // officiele afdeling.
                // Vandaar dat we zowel afdeling als afdelingsjaar moeten aanpassen.
				ServiceHelper.CallService<IGroepenService>(e => e.AfdelingsJaarBewaren(model.AfdelingsJaar));
                ServiceHelper.CallService<IGroepenService>(e => e.AfdelingBewaren(model.Afdeling));

				TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

				return RedirectToAction("Index");
			}
			catch (FaultException<FoutNummerFault> ex)
			{
                switch (ex.Detail.FoutNummer)
                {
                    case FoutNummer.OngeldigeGeboorteJarenVoorAfdeling:
                        ModelState.AddModelError("AfdelingsJaar.GeboorteJaarTot", Properties.Resources.MinimumLeeftijd);
                        break;
                    default:
                        ModelState.AddModelError("Afdeling.Naam", ex.Detail.Bericht);
                        break;
                }
                

				// Vul model aan, en toon de view AfdelingsJaar opnieuw
				model.Afdeling = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(model.AfdelingsJaar.AfdelingID));
				model.OfficieleAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(svc => svc.OfficieleAfdelingenOphalen());

				model.Titel = "Afdeling bewerken";
				return View("AfdelingsJaar", model);
			}
		}

        /// <summary>
        /// Laat de gebruiker een bestaande afdeling met afdelingID <paramref name="id"/>
        /// bewerken.
        /// </summary>
        /// <param name="groepID">ID van de geselecteerde groep</param>
        /// <param name="id">ID van de te bewerken afdeling</param>
        /// <returns>De view 'afdeling'</returns>
        [HandleError]
        public ActionResult AfdBewerken(int groepID, int id)
        {
            var model = new AfdelingInfoModel();
            BaseModelInit(model, groepID);

            AfdelingInfo detail = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(id));

            model.Info = detail;
           
            model.Titel = "Afdeling bewerken";
            return View("Afdeling", model);
        }

        /// <summary>
        /// Postback voor activeren/bewerken afdeling
        /// </summary>
        /// <param name="model">De property <c>model.AfdelingInfo</c> bevat de relevante details over de afdeling</param>
        /// <param name="groepID">Groep waarin de gebruiker momenteel aan het werken is</param>
        /// <returns>Het afdelingsoverzicht als de wijzigingen bewaard zijn, en anders opnieuw de
        /// 'AfdelingView'.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult AfdBewerken(AfdelingInfoModel model, int groepID)
        {
            BaseModelInit(model, groepID);
            
            try
            {
                ServiceHelper.CallService<IGroepenService>(e => e.AfdelingBewaren(model.Info));

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

                return RedirectToAction("Index");
            }
            catch (FaultException<BestaatAlFault<AfdelingInfo>> ex)
            {
                if (string.Compare(
                    ex.Detail.Bestaande.Afkorting,
                    model.Info.Afkorting,
                    true) == 0)
                {
                    ModelState.AddModelError(
                        "Info.Afkorting",
                        string.Format(
                            Properties.Resources.AfdelingsCodeBestaatAl,
                            ex.Detail.Bestaande.Afkorting,
                            ex.Detail.Bestaande.Naam));
                }
                else if (string.Compare(
                    ex.Detail.Bestaande.Naam,
                    model.Info.Naam,
                    true) == 0)
                {
                    ModelState.AddModelError(
                        "Info.Naam",
                        string.Format(
                            Properties.Resources.AfdelingsNaamBestaatAl,
                            ex.Detail.Bestaande.Afkorting,
                            ex.Detail.Bestaande.Naam));
                }
                else
                {
                    // Dit kan niet.
                    Debug.Assert(false);
                }

                return View("Afdeling", model);
            }
        }
	}
}
