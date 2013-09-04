using System;
using System.Linq;
using System.Web;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApi.Helpers
{
    public static class ApiHelper
    {
        public static GroepsWerkJaar GetGroepsWerkJaar(ChiroGroepEntities context)
        {
            // Selector voor de geldige gebruikersrechten
            Func<GebruikersRecht, bool> geldigeRechten =
                g =>
                String.Equals(g.Gav.Login, HttpContext.Current.User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) &&
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