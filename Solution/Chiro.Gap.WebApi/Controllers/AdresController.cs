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
using Chiro.Cdf.Ioc;
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

        private readonly ApiHelper _apiHelper;
        protected ApiHelper ApiHelper { get { return _apiHelper; } }


        public AdresController(): base()
        {
            _apiHelper = Factory.Maak<ApiHelper>();
            _groepsWerkJaar = ApiHelper.GetGroepsWerkJaar(_context);
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<AdresModel> Get()
        {
            // In plaats van eerst alle personen op te halen en dan van elke persoon de
            // communicatievorm, kunnen we dit met SelectMany in 1 expressie schrijven
            Func<GelieerdePersoon, IEnumerable<AdresModel>> manySelector =
                gp => gp.Persoon.PersoonsAdres.Select(pa => new AdresModel(pa.Adres));
            return _groepsWerkJaar.Groep.GelieerdePersoon.SelectMany(manySelector).AsQueryable();
        }

        protected override AdresModel GetEntityByKey([FromODataUri] int key)
        {
            var adres = _context.Adres.Find(key);
            if (adres == null || !MagLezen(adres))
            {
                return null;
            }
            return new AdresModel(adres);
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
                pa.GelieerdePersoon.Where(gp => gp.Groep.ID == _groepsWerkJaar.Groep.ID)
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
            return _groepsWerkJaar.Groep.GelieerdePersoon.Any(gp => gp.PersoonsAdres.Adres.ID == adres.ID);
        }
    }
}