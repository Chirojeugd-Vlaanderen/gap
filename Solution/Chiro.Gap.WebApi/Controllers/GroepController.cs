using System;
using System.Collections.Generic;
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
    public class GroepController : EntitySetController<GroepModel, int>

    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GebruikersRecht _recht;
        private readonly GroepsWerkJaar _groepsWerkJaar;

        public GroepController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));
            _groepsWerkJaar = _recht.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();
        }

        public override IQueryable<GroepModel> Get()
        {
            var lijst = new List<GroepModel>();
            lijst.Add(new GroepModel(_recht.Groep));
            return lijst.AsQueryable();
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
            
            return ! MagLezen(groep)
                       ? null
                       : groep.GelieerdePersoon.Select(p => new PersoonModel(p, _groepsWerkJaar)).AsQueryable();
        }

        [Queryable(PageSize = 12)]
        public IQueryable<AfdelingModel> GetAfdelingen(int key)
        {
            Groep groep = _context.Groep.Find(key);
            if (groep == null)
            {
                return null;
            }
            if (!MagLezen(groep))
            {
                return null;
            }
            return _groepsWerkJaar.AfdelingsJaar.Select(aj => new AfdelingModel(aj)).AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(Groep groep)
        {
            return Equals(groep, _recht.Groep);
        }
    }
}