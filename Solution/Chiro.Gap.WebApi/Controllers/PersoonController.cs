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
        private readonly GroepsWerkJaar _groepsWerkJaar;

        public PersoonController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));
            _groepsWerkJaar = _recht.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<PersoonModel> Get()
        {
            return _recht.Groep.GelieerdePersoon.Select(gp => new PersoonModel(gp, _groepsWerkJaar)).AsQueryable();
        }

        protected override PersoonModel GetEntityByKey([FromODataUri] int key)
        {
            GelieerdePersoon gelieerdePersoon = _context.GelieerdePersoon.Find(key);
            if (gelieerdePersoon == null)
            {
                return null;
            }
            return !MagLezen(gelieerdePersoon) ? null : new PersoonModel(gelieerdePersoon, _groepsWerkJaar);
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

        public IQueryable<ContactgegevenModel> GetContactgegevens([FromODataUri] int key)
        {
            GelieerdePersoon gelieerdePersoon = _context.GelieerdePersoon.Find(key);
            if (gelieerdePersoon == null)
            {
                return null;
            }
            return !MagLezen(gelieerdePersoon)
                       ? null
                       : gelieerdePersoon.Communicatie.Select(c => new ContactgegevenModel(c)).AsQueryable();

        }

        public IQueryable<AfdelingModel> GetAfdelingen([FromODataUri] int key)
        {
            var gelieerdePersoon = _context.GelieerdePersoon.Find(key);
            if (gelieerdePersoon == null)
            {
                return null;
            }
            if (! MagLezen(gelieerdePersoon))
            {
                return null;
            }
            var lid = gelieerdePersoon.Lid.FirstOrDefault(
                l => l.GroepsWerkJaar.ID == _groepsWerkJaar.ID);
            var lidVan = _groepsWerkJaar.AfdelingsJaar.Where(aj => aj.Kind.Contains(lid));
            var leidingVan = _groepsWerkJaar.AfdelingsJaar.Where(aj => aj.Leiding.Contains(lid));
            return leidingVan.Union(lidVan).Select(aj => new AfdelingModel(aj)).AsQueryable();
        }

        public IQueryable<AdresModel> GetAdressen([FromODataUri] int key)
        {
            var gelieerdePersoon = _context.GelieerdePersoon.Find(key);
            if (gelieerdePersoon == null)
            {
                return null;
            }
            if (!MagLezen(gelieerdePersoon))
            {
                return null;
            }

            return gelieerdePersoon.Persoon.PersoonsAdres.Select(pa => new AdresModel(pa.Adres)).AsQueryable();
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