using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.OData;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Models;

namespace Chiro.Gap.WebApi.Controllers
{
    public class PersoonController : EntitySetController<PersoonModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly IQueryable<PersoonModel> personen;

        public PersoonController()
        {
            var repo = new Repository<GelieerdePersoon>(_context);
            personen = repo.Select().Select(
                p => new PersoonModel {
                    Id = p.ID,
                    GeboorteDatum = p.Persoon.GeboorteDatum,
                    GroepId = p.Groep.ID,
                    Naam = p.Persoon.Naam,
                    Voornaam = p.Persoon.VoorNaam
                });
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<PersoonModel> Get()
        {
            return personen.AsQueryable();
        }

        protected override PersoonModel GetEntityByKey(int key)
        {
            return personen.FirstOrDefault(p => p.Id == key);
        }
     

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}