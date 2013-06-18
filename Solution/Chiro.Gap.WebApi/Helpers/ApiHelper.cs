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
        public static GebruikersRecht getGebruikersRecht(ChiroGroepEntities context)
        {
            Func<GebruikersRecht, bool> selector =
                g =>
                g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                (g.VervalDatum == null || g.VervalDatum > DateTime.Now);
            return context.GebruikersRecht.First(selector);
        }

        public static int GetGroepsWerkJaarId(GebruikersRecht gebruikersRecht)
        {
            // Code gestolen uit JaarOvergangController. Kunnen we dit zonder de service?
            return
                ServiceHelper.CallService<IGroepenService, int>(
                    svc => svc.RecentsteGroepsWerkJaarIDGet(gebruikersRecht.Groep.ID));
        }
    }
}