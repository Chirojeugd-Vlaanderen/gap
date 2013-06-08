using System.Linq;
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
            var user = HttpContext.Current.User;
            var gav = _context.Gav.FirstOrDefault(g => g.Login == user.Identity.Name);
            if (gav == null)
            {
                return null;
            }

            return
                gav.GebruikersRecht.Select(
                    gr => new GroepModel {
                            Id = gr.Groep.ID, 
                            Naam = gr.Groep.Naam, 
                            StamNummer = gr.Groep.Code
                    })
                   .AsQueryable();


        }

        protected override GroepModel GetEntityByKey(int key)
        {
            var groep = groepen.FirstOrDefault(g => g.Id == key);
            
            return groep;

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