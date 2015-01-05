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
    public class AfdelingController : EntitySetController<AfdelingModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GroepsWerkJaar _groepsWerkJaar;

        private readonly ApiHelper _apiHelper;
        protected ApiHelper ApiHelper { get { return _apiHelper; } }

        public AfdelingController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
            _groepsWerkJaar = ApiHelper.GetGroepsWerkJaar(_context);
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<AfdelingModel> Get()
        {
            return _groepsWerkJaar.AfdelingsJaar.Select(aj => new AfdelingModel(aj.Afdeling)).AsQueryable();
        }

        protected override AfdelingModel GetEntityByKey([FromODataUri] int key)
        {
            Afdeling afdeling = _context.Afdeling.Find(key);
            if (afdeling == null || !MagLezen(afdeling))
            {
                return null;
            }
            return new AfdelingModel(afdeling);
        }

        public GroepModel GetGroep([FromODataUri] int key)
        {
            Afdeling afdeling = _context.Afdeling.Find(key);
            if (afdeling == null || !MagLezen(afdeling))
            {
                return null;
            }
            return new GroepModel(afdeling.ChiroGroep);
        }

        [Queryable(PageSize = 25)]
        public IQueryable<PersoonModel> GetPersonen([FromODataUri] int key)
        {
            Afdeling afdeling = _context.Afdeling.Find(key);
            if (afdeling == null)
            {
                return null;
            }
            AfdelingsJaar afdelingsJaar =
                _groepsWerkJaar.AfdelingsJaar.FirstOrDefault(aj => aj.Afdeling.ID == afdeling.ID);
            if (afdelingsJaar == null || ! MagLezen(afdelingsJaar))
            {
                return null;
            }

            IEnumerable<PersoonModel> leden = afdelingsJaar.Kind.Select(k => new PersoonModel(k));
            IEnumerable<PersoonModel> leiding = afdelingsJaar.Leiding.Select(l => new PersoonModel(l));
            return leden.Union(leiding).AsQueryable();
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(AfdelingsJaar afdelingsJaar)
        {
            return Equals(afdelingsJaar.GroepsWerkJaar, _groepsWerkJaar);
        }

        private bool MagLezen(Afdeling afdeling)
        {
            // juiste groep en de afdeling moet in dit groepswerkjaar zitten
            return Equals(afdeling.ChiroGroep, _groepsWerkJaar.Groep) &&
                   _groepsWerkJaar.AfdelingsJaar.Any(aj => aj.Afdeling.ID == afdeling.ID);
        }
    }
}