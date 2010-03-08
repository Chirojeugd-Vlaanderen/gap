// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public class GroepController : BaseController
	{
		public GroepController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		/// <summary>
		/// Genereert een view met algemene gegevens over de groep
		/// </summary>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>View met algemene gegevens over de groep</returns>
		public ActionResult Index(int groepID)
		{
			GroepsInstellingenModel model = new GroepsInstellingenModel();

			model.Titel = Properties.Resources.GroepsInstellingenTitel;
			model.Info = ServiceHelper.CallService<IGroepenService, GroepInfo>(svc => svc.Ophalen(
				groepID,
				GroepsExtras.AfdelingenHuidigWerkJaar | GroepsExtras.Categorieen));

			return View(model);
		}

		/// <summary>
		/// Actie voor als de gebruiker gegevens heeft ingevuld voor een nieuwe categorie
		/// </summary>
		/// <param name="model">Model voor Groepsinstellingen</param>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>Opnieuw de view Groep/Index</returns>
		[AcceptVerbs(HttpVerbs.Post)]
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

					return RedirectToAction("Index", new { groepID = groepID });
				}
				catch (FaultException<BestaatAlFault> ex)
				{
					// Geef feedback aan de gebruiker: de naam of de code worden al gebruikt
					if (ex.Detail.FoutCode == BestaatAlFaultCode.CategorieCodeBestaatAl)
					{
						ModelState.AddModelError(
							"NieuweCategorie.Code",
							String.Format(
								Properties.Resources.CategorieCodeBestaatAl,
								model.NieuweCategorie.Code));
					}
					else if (ex.Detail.FoutCode == BestaatAlFaultCode.CategorieNaamBestaatAl)
					{
						ModelState.AddModelError(
							"NieuweCategorie.Naam",
							String.Format(
								Properties.Resources.CategorieNaamBestaatAl,
								model.NieuweCategorie.Naam));
					}
					else
					{
						throw new NotImplementedException();
					}

					model.Titel = Properties.Resources.GroepsInstellingenTitel;
					model.Info = ServiceHelper.CallService<IGroepenService, GroepInfo>(svc => svc.Ophalen(
						groepID,
						GroepsExtras.AfdelingenHuidigWerkJaar | GroepsExtras.Categorieen));

					return View(model);
				}
				catch (Exception)
				{
					throw;
				}
			}
			else
			{
				// ModelState bevat ongeldige waarden, dus toon de pagina opnieuw
				model.Titel = Properties.Resources.GroepsInstellingenTitel;
				model.Info = ServiceHelper.CallService<IGroepenService, GroepInfo>(svc => svc.Ophalen(
					groepID,
					GroepsExtras.AfdelingenHuidigWerkJaar | GroepsExtras.Categorieen));

				return View(model);
			}
		}

		/// <summary>
		/// Verwijdert een categorie.  Indien de categorie niet leeg was, lukt dat direct, en krijg
		/// je opnieuw de view 'Index'.  In het andere geval krijg je de view 'CategorieVerwijderen'.
		/// </summary>
		/// <param name="groepID">ID van de groep waarin de gebruiker momenteel aan het werken is</param>
		/// <param name="id">CategorieID van te verwijderen categorie</param>
		/// <returns>Opnieuw de Index als de categorie leeg was, en anders de view CategorieVerwijderen.
		/// </returns>
		public ActionResult CategorieVerwijderen(int groepID, int id)
		{
			try
			{
				ServiceHelper.CallService<IGroepenService>(svc => svc.CategorieVerwijderen(id, false));
			}
			catch (FaultException<GekoppeldeObjectenFault<PersoonInfo>> ex)
			{
				// Categorie was niet leeg

				var model = new Models.PersonenLinksModel();
				BaseModelInit(model, groepID);

				model.Personen = ex.Detail.Objecten;
				model.CategorieID = id;
				model.VolledigeLijstUrl = Url.Action("List", "Personen", new RouteValueDictionary(new { id = id, groepID = groepID }));

				// Vis categorienaam op uit de gekoppelde categorieen van de personen
				// niet elegant, maar werkt wel.

				string categorieNaam = (from cat in model.Personen.First().CategorieLijst
										where cat.ID == id
										select cat).First().Naam;

				model.Titel = String.Format("Categorie '{0}' verwijderen", categorieNaam);

				return View("CategorieVerwijderen", model);
			}
			catch (Exception)
			{
				// Onverwachte exception gewoon verder throwen
				throw;
			}
			return RedirectToAction("Index", new { groepID = groepID });
		}

		/// <summary>
		/// Forceert het verwijderen van een categorie
		/// </summary>
		/// <param name="model">PersonenLinkModel, waarvan enkel GroepID en CategorieID van belang zijn</param>
		/// <returns>Een redirect naar de actie 'Index'</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CategorieVerwijderen(PersonenLinksModel model)
		{
			ServiceHelper.CallService<IGroepenService>(svc => svc.CategorieVerwijderen(
				model.CategorieID,
				true));	// forceer; ook als categorie niet leeg.

			return RedirectToAction("Index", new { groepID = model.GroepID });
		}
	}
}
