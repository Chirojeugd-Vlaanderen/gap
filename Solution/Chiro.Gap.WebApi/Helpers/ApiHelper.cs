using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApi.Helpers
{
    public class ApiHelper
    {
        private readonly ServiceHelper _serviceHelper;
        protected ServiceHelper ServiceHelper { get { return _serviceHelper; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceHelper">De servicehelper die het groepwerkjaar zal
        /// opleveren.</param>
        public ApiHelper(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        public GroepsWerkJaar GetGroepsWerkJaar(ChiroGroepEntities context)
        {
            Debug.Assert(HttpContext.Current.Request.LogonUserIdentity != null);

            // Selector voor de geldige gebruikersrechten
            Func<GebruikersRecht, bool> geldigeRechten =
                g =>
                String.Equals(g.Gav.Login, HttpContext.Current.Request.LogonUserIdentity.Name, StringComparison.CurrentCultureIgnoreCase) &&
                (g.VervalDatum == null || g.VervalDatum > DateTime.Now);

            // Haal het eerste geldige recht op
            var recht = context.GebruikersRecht.First(geldigeRechten);

            // Code gestolen uit JaarOvergangController. Kunnen we dit zonder de service?
            // Haal de ID van het huidige groepswerkjaar op
            var id = ServiceHelper.CallService<IGroepenService, int>(
                svc => svc.RecentsteGroepsWerkJaarIDGet(recht.Groep.ID));

            // Haal het huidige groepswerkjaar uit de DB
            return context.GroepsWerkJaar.Find(id);
        }
    }
}