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
    public class ContactgegevenController : EntitySetController<ContactgegevenModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GebruikersRecht _recht;
        private readonly GroepsWerkJaar _groepsWerkJaar;

        public ContactgegevenController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));
            _groepsWerkJaar = _recht.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<ContactgegevenModel> Get()
        {
            return new List<ContactgegevenModel>().AsQueryable();
        }

        protected override ContactgegevenModel GetEntityByKey([FromODataUri] int key)
        {
            var communicatieVorm = _context.CommunicatieVorm.Find(key);
            if (communicatieVorm == null)
            {
                return null;
            }
            return !MagLezen(communicatieVorm) ? null : new ContactgegevenModel(communicatieVorm);
        }

        public PersoonModel GetPersoon([FromODataUri] int key)
        {
            var communicatieVorm = _context.CommunicatieVorm.Find(key);
            if (communicatieVorm == null)
            {
                return null;
            }
            return !MagLezen(communicatieVorm) ? null : new PersoonModel(communicatieVorm.GelieerdePersoon, _groepsWerkJaar);
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(CommunicatieVorm communicatieVorm)
        {
            return Equals(communicatieVorm.GelieerdePersoon.Groep, _recht.Groep);
        }
    }
}