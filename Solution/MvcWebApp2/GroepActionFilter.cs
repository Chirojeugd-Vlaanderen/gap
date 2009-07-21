using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cg2.Adf.ServiceModel;
using Cg2.ServiceContracts;

namespace MvcWebApp2
{
    public class GroepActionFilterAttribute : FilterAttribute, IAuthorizationFilter
    {

        public GroepActionFilterAttribute()
            : base()
        {
        }

        #region IAuthorizationFilter Members

        /// <summary>
        /// Het authorisatiemechanisme voor de GroepActionFilter wordt
        /// gebruikt om te bepalen welke groep een gebruiker wil beheren.
        /// De gebruiker krijgt de keuze uit de groepen waarvoor hij
        /// GAV is.
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (Sessie.GroepID == 0)
            {
                // Er is nog geen groep gekozen. Kijk of er toevallig maar één groep is.

                var groepInfoLijst = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
                    (g => g.OphalenMijnGroepen());
                if (groepInfoLijst.Count() == 1)
                {
                    // Sla in de sessie op dat de Gav niet moet kunnen wisselen van groep, en over welke groepID het gaat
                    Sessie.IsMultiGav = false;
                    Sessie.GroepID = groepInfoLijst.First().ID;
                }
                else
                {
                    // Sla in de sessie op dat de Gav moet kunnen wisselen van groep
                    MvcWebApp2.Sessie.IsMultiGav = true;

                    // Redirect naar de Kies Groep pagina
                    // TODO Hier proberen de return url in een parameter te duwen
                    filterContext.Result = new RedirectResult("/Gav/Index");
                }

            }

        }

        #endregion
    }
}
