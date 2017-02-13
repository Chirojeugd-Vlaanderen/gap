// Source: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/

using System.Collections.Generic;
using System.Web.Http;
using Chiro.Cdf.ServiceHelper;

namespace Chiro.Gap.Api.Controllers
{
    [RoutePrefix("api/Leden")]
    public class LedenController : BaseController
    {
        public LedenController(ServiceHelper serviceHelper) : base(serviceHelper)
        {
        }

        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            // Return dummy data.
            return Ok(Lid.CreateLeden());
        }

    }

    #region Helpers

    public class Lid
    {
        public int AdNummer { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public string[] Afdelingen { get; set; }
        public bool IsLeiding { get; set; }

        public static List<Lid> CreateLeden()
        {
            List<Lid> Ledenlijst = new List<Lid>
            {
                new Lid {AdNummer = 1, Afdelingen = new[] {"SP"}, Naam = "Mallezie", Voornaam = "Tim", IsLeiding = true},
                new Lid
                {
                    AdNummer = 2,
                    Afdelingen = new[] {"RAK"},
                    Naam = "Severs",
                    Voornaam = "Paul",
                    IsLeiding = false
                }
            };

            return Ledenlijst;
        }
    }

    #endregion
}