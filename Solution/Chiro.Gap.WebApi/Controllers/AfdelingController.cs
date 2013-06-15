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
    public class AfdelingController : EntitySetController<AfdelingModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GebruikersRecht _recht;
        private readonly GroepsWerkJaar _groepsWerkJaar;

        public AfdelingController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));

            _groepsWerkJaar = _recht.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<AfdelingModel> Get()
        {
            return _groepsWerkJaar.AfdelingsJaar.Select(aj => new AfdelingModel(aj)).AsQueryable();
        }

        protected override AfdelingModel GetEntityByKey([FromODataUri] int key)
        {
            var afdelingsJaar = _context.AfdelingsJaar.Find(key);
            if (afdelingsJaar == null)
            {
                return null;
            }
            return !MagLezen(afdelingsJaar) ? null : new AfdelingModel(afdelingsJaar);
        }

        public GroepModel GetGroep([FromODataUri] int key)
        {
            var afdelingsJaar = _context.AfdelingsJaar.Find(key);
            if (afdelingsJaar == null)
            {
                return null;
            }
            return !MagLezen(afdelingsJaar) ? null : new GroepModel(afdelingsJaar.GroepsWerkJaar.Groep);
        }

        [Queryable]
        public IQueryable<PersoonModel> GetPersonen([FromODataUri] int key)
        {
            var afdelingsJaar = _context.AfdelingsJaar.Find(key);
            if (afdelingsJaar == null)
            {
                return null;
            }
            if (! MagLezen(afdelingsJaar))
            {
                return null;
            }
            var leden = afdelingsJaar.Kind.Select(k => new PersoonModel(k));
            var leiding = afdelingsJaar.Leiding.Select(l => new PersoonModel(l));
            return leden.Union(leiding).AsQueryable();
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(AfdelingsJaar afdelingsJaar)
        {
            return Equals(afdelingsJaar.GroepsWerkJaar.Groep, _recht.Groep);
        }
    }
}