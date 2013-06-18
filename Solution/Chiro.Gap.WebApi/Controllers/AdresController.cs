/*
 * Copyright 2013 Ben Bridts.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Helpers;
using Chiro.Gap.WebApi.Models;

namespace Chiro.Gap.WebApi.Controllers
{
    [Authorize]
    public class AdresController : EntitySetController<AdresModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GroepsWerkJaar _groepsWerkJaar;
        private readonly int _groepsWerkJaarId;
        private readonly GebruikersRecht _recht;

        public AdresController()
        {
            _recht = ApiHelper.getGebruikersRecht(_context);
            _groepsWerkJaarId = ApiHelper.GetGroepsWerkJaarId(_recht);
            _groepsWerkJaar = _context.GroepsWerkJaar.Find(_groepsWerkJaarId);
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<AdresModel> Get()
        {
            // In plaats van eerst alle personen op te halen en dan van elke persoon de
            // communicatievorm, kunnen we dit met SelectMany in 1 expressie schrijven
            Func<GelieerdePersoon, IEnumerable<AdresModel>> manySelector =
                gp => gp.Persoon.PersoonsAdres.Select(pa => new AdresModel(pa.Adres));
            return _recht.Groep.GelieerdePersoon.SelectMany(manySelector).AsQueryable();
        }

        protected override AdresModel GetEntityByKey([FromODataUri] int key)
        {
            Adres Adres = _context.Adres.Find(key);
            if (Adres == null || !MagLezen(Adres))
            {
                return null;
            }
            return AdresModel(Adres);
        }

        [Queryable(PageSize = 10)]
        public IQueryable<PersoonModel> GetPersonen([FromODataUri] int key)
        {
            Adres adres = _context.Adres.Find(key);
            if (adres == null || !MagLezen(adres))
            {
                return null;
            }
            
            Func<PersoonsAdres, IEnumerable<PersoonModel>> manySelector =
                pa =>
                pa.GelieerdePersoon.Where(gp => gp.Groep.ID == _recht.Groep.ID)
                  .Select(gp => new PersoonModel(gp, _groepsWerkJaar));
            return adres.PersoonsAdres.SelectMany(manySelector).AsQueryable();
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