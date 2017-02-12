using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chiro.Gap.Api.Controllers
{
    [RoutePrefix("api/Leden")]
    public class LedenController : ApiController
    {
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