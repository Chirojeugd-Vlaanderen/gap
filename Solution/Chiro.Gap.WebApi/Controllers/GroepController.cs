using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Models;

namespace Chiro.Gap.WebApi.Controllers
{
    public class GroepController : EntitySetController<GroepModel, int>

    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly IQueryable<GroepModel> groepen;

        public GroepController()
        {
            groepen = new Repository<Groep>(_context).Select().Select(
                grp => new GroepModel
                    {
                        Id = grp.ID,
                        Naam = grp.Naam,
                        StamNummer = grp.Code
                    });
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<GroepModel> Get()
        {
            return groepen.AsQueryable();
        }

        protected override GroepModel GetEntityByKey(int key)
        {
            return groepen.FirstOrDefault(g => g.Id == key);
        }

        [Queryable(PageSize = 50)]
        public IQueryable<PersoonModel> GetPersonen(int key)
        {
            var personen = new Repository<GelieerdePersoon>(_context).Select().Select(
                p => new PersoonModel
                {
                    Id = p.ID,
                    GeboorteDatum = p.Persoon.GeboorteDatum,
                    GroepId = p.Groep.ID,
                    Naam = p.Persoon.Naam,
                    Voornaam = p.Persoon.VoorNaam
                });
            return personen.Where(p => p.GroepId == key).AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}