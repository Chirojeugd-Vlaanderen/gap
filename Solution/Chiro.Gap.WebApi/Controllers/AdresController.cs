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
    public class AdresController : EntitySetController<AdresModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GebruikersRecht _recht;
        private readonly GroepsWerkJaar _groepsWerkJaar;

        public AdresController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));
            _groepsWerkJaar = _recht.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<AdresModel> Get()
        {
            // In plaats van eerst alle personen op te halen en dan van elke persoon de
            // communicatievorm, kunnen we dit met SelectMany in 1 expressie schrijven
            return
                _recht.Groep.GelieerdePersoon.SelectMany(gp => gp.Persoon.PersoonsAdres.Select(pa => new AdresModel(pa.Adres)))
                      .AsQueryable();
        }

        protected override AdresModel GetEntityByKey([FromODataUri] int key)
        {
            var Adres = _context.Adres.Find(key);
            if (Adres == null)
            {
                return null;
            }
            return !MagLezen(Adres) ? null : new AdresModel(Adres);
        }

        public IQueryable<PersoonModel> GetPersonen([FromODataUri] int key)
        {
            var adres = _context.Adres.Find(key);
            if (adres == null)
            {
                return null;
            }
            if (!MagLezen(adres))
            {
                return null;
            }
            return
                adres.PersoonsAdres.SelectMany(
                    pa => pa.GelieerdePersoon.Where(gp => gp.Groep.ID == _recht.Groep.ID).Select(gp => new PersoonModel(gp, _groepsWerkJaar))).AsQueryable();
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(Adres adres)
        {
            return _recht.Groep.GelieerdePersoon.Any(gp => gp.PersoonsAdres.Adres.ID == adres.ID);
        }
    }
}