using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Models;

namespace Chiro.Gap.WebApi.Controllers
{
    [Authorize]
    public class PersoonController : EntitySetController<PersoonModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GebruikersRecht _recht;

        public PersoonController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<PersoonModel> Get()
        {
            return _recht.Groep.GelieerdePersoon.Select(gp => new PersoonModel(gp)).AsQueryable();
        }

        protected override PersoonModel GetEntityByKey([FromODataUri] int key)
        {
            GelieerdePersoon gelieerdePersoon = _context.GelieerdePersoon.Find(key);
            if (gelieerdePersoon == null)
            {
                return null;
            }
            return !MagLezen(gelieerdePersoon) ? null : new PersoonModel(gelieerdePersoon);
        }

        public GroepModel GetGroep([FromODataUri] int key)
        {
            GelieerdePersoon gelieerdePersoon = _context.GelieerdePersoon.Find(key);
            if (gelieerdePersoon == null)
            {
                return null;
            }
            return !MagLezen(gelieerdePersoon) ? null : new GroepModel(gelieerdePersoon.Groep);
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(GelieerdePersoon gelieerdePersoon)
        {
            return Equals(gelieerdePersoon.Groep, _recht.Groep);
        }
    }
}