using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Mvc;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Models;
using Chiro.Gap.Workers;

namespace Chiro.Gap.WebApi.Controllers
{
    [System.Web.Http.Authorize]
    public class GroepController : EntitySetController<GroepModel, int>

    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly IQueryable<Groep> _groepen;

        public GroepController()
        {
            _groepen = new Repository<Groep>(_context).Select();
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<GroepModel> Get()
        {
            var user = HttpContext.Current.User;
            var gav = _context.Gav.FirstOrDefault(g => g.Login == user.Identity.Name);
            if (gav == null)
            {
                return null;
            }

            return gav.GebruikersRecht.Select(gr => new GroepModel (gr.Groep)).AsQueryable();
        }

        protected override GroepModel GetEntityByKey(int key)
        {
            var groep = _groepen.FirstOrDefault(g => g.ID == key);
            if (! MagLezen(groep))
            {
                Request.CreateErrorResponse(HttpStatusCode.Forbidden, new HttpError());
            }
            return new GroepModel(groep);

        }

        [Queryable(PageSize = 50)]
        public IQueryable<PersoonModel> GetPersonen(int key)
        {
            var groep = _groepen.FirstOrDefault(g => g.ID == key);
            if (! MagLezen(groep))
            {
                Request.CreateErrorResponse(HttpStatusCode.Forbidden, new HttpError());
            }
            return groep.GelieerdePersoon.Select(p => new PersoonModel(p)).AsQueryable();
        }

        [Queryable(PageSize = 10)]
        public IQueryable<WerkjaarModel> GetWerkjaren(int key )
        {
            var groep = _groepen.FirstOrDefault(g => g.ID == key);
            if (!MagLezen(groep))
            {
                Request.CreateErrorResponse(HttpStatusCode.Forbidden, new HttpError());
            }
            return groep.GroepsWerkJaar.Select(gwj => new WerkjaarModel(gwj)).AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(Groep groep)
        {
            if (groep == null)
            {
                return false;
            }
            var autorisatieManager = new AutorisatieManager(new AuthenticatieManager());
            return autorisatieManager.IsGav(groep);


        }
    }
}