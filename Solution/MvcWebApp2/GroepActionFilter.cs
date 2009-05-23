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

        public GroepActionFilterAttribute(string sessionVariable)
            : base()
        {
            this.SessionVariable = sessionVariable;
        }

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            object groepIdSessionItem = filterContext.HttpContext.Session[SessionVariable];

            if (groepIdSessionItem == null)
            {
                // Er is nog geen groep gekozen. Kijk of er toevallig maar één groep is.

                var groepInfoLijst = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
                    (g => g.OphalenMijnGroepen());
                if (groepInfoLijst.Count() == 1)
                {
                    filterContext.HttpContext.Session[SessionVariable] = groepInfoLijst.First().ID;
                }
                else
                {
                    // Redirect naar de Kies Groep pagina
                    // TODO Hier proberen de return url in een parameter te duwen
                    filterContext.Result = new RedirectResult("/Gav/Index");
                }

            }

        }

        #endregion

        #region properties

        public string SessionVariable { get; set; }

        #endregion
    }
}
