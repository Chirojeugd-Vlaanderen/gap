﻿/*
 * Copyright 2008-2013, 2016, 2017 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;
using Chiro.Cdf.Authentication;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor alles wat te maken heeft met beheer van categorieën
    /// </summary>
	[HandleError]
	public class CategorieenController : BaseController
	{
	    /// <summary>
	    /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
	    /// best toegewezen via inversion of control.
	    /// </summary>
	    /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
	    /// service</param>
	    /// <param name="serviceHelper"></param>
	    /// <param name="authenticator"></param>
	    public CategorieenController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper, IAuthenticator authenticator)
            : base(veelGebruikt, serviceHelper, authenticator)
        {
        }

        /// <summary>
        /// Genereert een view met de categorieën die de groep gebruikt
        /// </summary>
        /// <param name="groepID">ID van de gewenste groep</param>
        /// <returns>View met categorieën van de groep</returns>
        [HandleError]
		public override ActionResult Index(int groepID)
		{
			var model = new GroepsInstellingenModel
			{
				Titel = Properties.Resources.GroepsInstellingenTitel,
				Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
					svc => svc.DetailOphalen(groepID))
			};
		    BaseModelInit(model, groepID);

			return View(model);
		}

		/// <summary>
		/// Verwijdert een categorie.  Indien de categorie niet leeg was, lukt dat direct, en krijg
		/// je opnieuw de view 'Index'.  In het andere geval krijg je de view 'CategorieVerwijderen'.
		/// </summary>
		/// <param name="groepID">ID van de groep waarin de gebruiker momenteel aan het werken is</param>
		/// <param name="id">CategorieID van te verwijderen categorie</param>
		/// <returns>Opnieuw de Index als de categorie leeg was, en anders de view CategorieVerwijderen.
		/// </returns>
		[HandleError]
		public ActionResult CategorieVerwijderen(int groepID, int id)
		{
			try
			{
				ServiceHelper.CallService<IGroepenService>(svc => svc.CategorieVerwijderen(id, false));
				TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
                return RedirectToAction("Categorieen", new { Controller = "Groep", groepID = groepID });
			}
			catch (FaultException<BlokkerendeObjectenFault<PersoonDetail>> ex)
			{
				// Categorie was niet leeg

				var model = new PersonenLinksModel();
				BaseModelInit(model, groepID);

				model.Personen = ex.Detail.Objecten;
				model.CategorieID = id;
				model.VolledigeLijstUrl = Url.Action("List", "Personen", new RouteValueDictionary(new { id, groepID }));
				model.TotaalAantal = ex.Detail.Aantal;

				// Vis categorienaam op uit de gekoppelde categorieen van de personen.
				// Niet elegant, maar werkt wel.

				string categorieNaam = (from cat in model.Personen.First().CategorieLijst
										where cat.ID == id
										select cat).First().Naam;

				model.Titel = String.Format("Categorie '{0}' verwijderen", categorieNaam);

                return View("CategorieVerwijderen", model); // FIXME only reloads part of the previous page
			}
		}

		/// <summary>
		/// Forceert het verwijderen van een categorie
		/// </summary>
		/// <param name="model">PersonenLinkModel, waarvan enkel GroepID en CategorieID van belang zijn</param>
		/// <returns>Een redirect naar de actie 'Index'</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult CategorieVerwijderen(PersonenLinksModel model)
		{
			ServiceHelper.CallService<IGroepenService>(svc => svc.CategorieVerwijderen(
				model.CategorieID,
				true));	// forceer; ook als categorie niet leeg.

            return RedirectToAction("Categorieen", new { Controller = "Groep", groepID = model.GroepID });
		}

		/// <summary>
		/// Actie voor als de gebruiker gegevens heeft ingevuld voor een nieuwe categorie
		/// </summary>
		/// <param name="model">Model voor Groepsinstellingen</param>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>Opnieuw de view Categorieen/Index</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult Index(GroepsInstellingenModel model, int groepID)
		{
			// Ik heb chance, want momenteel staan er enkel constraints op de velden die de
			// gebruiker invulde, en die meegeleverd worden via model binding.
			// Dus het is nog niet nodig om hier het model aan te vullen...
			// Zodra er wel constraints staan op de groepsinfo/afdelingsinfo/... moet
			// het model toch op voorhand aangevuld worden.

			if (ModelState.IsValid)
			{
				// Als de ModelState geldig is: categorie toevoegen
				try
				{
					ServiceHelper.CallService<IGroepenService>(svc => svc.CategorieToevoegen(
						groepID,
						model.NieuweCategorie.Naam,
						model.NieuweCategorie.Code));

                    return RedirectToAction("Categorieen", new { Controller = "Groep", groepID = groepID });
				}
				catch (FaultException<BestaatAlFault<CategorieInfo>> ex)
				{
					if (String.Compare(
						model.NieuweCategorie.Code,
						ex.Detail.Bestaande.Code,
						true) == 0)
					{
						// Geef feedback aan de gebruiker: de naam of de code worden al gebruikt
						ModelState.AddModelError(
							"NieuweCategorie.Code",
							String.Format(
								Properties.Resources.CategorieCodeBestaatAl,
								ex.Detail.Bestaande.Code,
								ex.Detail.Bestaande.Naam));
					}
					else if (String.Compare(
						model.NieuweCategorie.Naam,
						ex.Detail.Bestaande.Naam,
						true) == 0)
					{
						// Geef feedback aan de gebruiker: de naam of de code worden al gebruikt
						ModelState.AddModelError(
							"NieuweCategorie.Naam",
							String.Format(
								Properties.Resources.CategorieNaamBestaatAl,
								ex.Detail.Bestaande.Code,
								ex.Detail.Bestaande.Naam));
					}
					else
					{
						Debug.Assert(false);
					}

					model.Titel = Properties.Resources.GroepsInstellingenTitel;
					model.Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
						svc => svc.DetailOphalen(groepID));

                    return View(model); // FIXME only reloads part of the previous page
				}
			}
			else
			{
				// ModelState bevat ongeldige waarden, dus toon de pagina opnieuw
				model.Titel = Properties.Resources.GroepsInstellingenTitel;
				model.Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
					svc => svc.DetailOphalen(groepID));

                return View(model); // FIXME only reloads part of the previous page
			}
		}
	}
}
