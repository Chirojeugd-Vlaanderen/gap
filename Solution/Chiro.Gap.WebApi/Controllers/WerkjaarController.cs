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
    public class WerkjaarController : EntitySetController<WerkjaarModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GebruikersRecht _recht;

        public WerkjaarController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<WerkjaarModel> Get()
        {
            return _recht.Groep.GroepsWerkJaar.Select(gwj => new WerkjaarModel(gwj)).AsQueryable();
        }

        protected override WerkjaarModel GetEntityByKey(int key)
        {
            GroepsWerkJaar groepsWerkJaar = _context.GroepsWerkJaar.Find(key);
            if (groepsWerkJaar == null)
            {
                return null;
            }
            return !MagLezen(groepsWerkJaar) ? null : new WerkjaarModel(groepsWerkJaar);
        }

        public GroepModel GetGroep([FromODataUri] int key)
        {
            GroepsWerkJaar groepsWerkJaar = _context.GroepsWerkJaar.Find(key);
            if (groepsWerkJaar == null)
            {
                return null;
            }
            return !MagLezen(groepsWerkJaar) ? null : new GroepModel(groepsWerkJaar.Groep);
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(GroepsWerkJaar groepsWerkJaar)
        {
            return Equals(groepsWerkJaar.Groep, _recht.Groep);
        }
    }
}