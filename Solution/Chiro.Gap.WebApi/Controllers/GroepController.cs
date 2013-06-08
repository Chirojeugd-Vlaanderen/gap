using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Models;

namespace Chiro.Gap.WebApi.Controllers
{
    public class GroepController : EntitySetController<GroepModel,int>

    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();

        [Queryable(PageSize = 10)]
        public override IQueryable<GroepModel> Get()
        {
            var groepenRepository = new Repository<Groep>(_context);

            var groepen = groepenRepository.Select().Select(grp => new GroepModel {Id = grp.ID, Naam = grp.Naam, StamNummer = grp.Code});

            return groepen.AsQueryable();
        }

        protected override GroepModel GetEntityByKey(int key)
        {
            var groepenRepository = new Repository<Groep>(_context);

            var groep = groepenRepository.Select().Select(grp => new GroepModel {Id = grp.ID, Naam = grp.Naam, StamNummer = grp.Code}).FirstOrDefault(g => g.Id == key);
            return groep;
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }


    }
}