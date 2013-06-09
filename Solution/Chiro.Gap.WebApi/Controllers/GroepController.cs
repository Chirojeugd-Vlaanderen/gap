using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Models;
using Chiro.Gap.Workers;

namespace Chiro.Gap.WebApi.Controllers
{
    [Authorize]
    public class GroepController : EntitySetController<GroepModel, int>

    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly IQueryable<GebruikersRecht> _recht;

        public GroepController()
        {
            _recht = _context.GebruikersRecht.Where(g =>
                                                    g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                    (g.VervalDatum == null || g.VervalDatum > DateTime.Now)
                ).Take(1);
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<GroepModel> Get()
        {
            return _recht.Select(r => new GroepModel(r.Groep)).AsQueryable();
        }

        protected override GroepModel GetEntityByKey([FromODataUri] int key)
        {
            Groep groep = _context.Groep.Find(key);
            if (groep == null)
            {
                return null;
            }
            return ! MagLezen(groep) ? null : new GroepModel(groep);
        }

        [Queryable(PageSize = 50)]
        public IQueryable<PersoonModel> GetPersonen(int key)
        {
            Groep groep = _context.Groep.Find(key);
            if (groep == null)
            {
                return null;
            }
            return ! MagLezen(groep) ? null : groep.GelieerdePersoon.Select(p => new PersoonModel(p)).AsQueryable();
        }

        [Queryable(PageSize = 10)]
        public IQueryable<WerkjaarModel> GetWerkjaren(int key)
        {
            Groep groep = _context.Groep.Find(key);
            if (groep == null)
            {
                return null;
            }
            return !MagLezen(groep) ? null : groep.GroepsWerkJaar.Select(gwj => new WerkjaarModel(gwj)).AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(Groep groep)
        {
            var autorisatieManager = new AutorisatieManager(new AuthenticatieManager());
            return autorisatieManager.IsGav(groep);
        }
    }
}