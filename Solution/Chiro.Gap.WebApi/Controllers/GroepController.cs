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
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WebApi.Helpers;
using Chiro.Gap.WebApi.Models;

namespace Chiro.Gap.WebApi.Controllers
{
    [Authorize]
    public class GroepController : EntitySetController<GroepModel, int>

    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly int _groepsWerkJaarId;
        private readonly GroepsWerkJaar _groepsWerkJaar;
        private readonly GebruikersRecht _recht;

        public GroepController()
        {
            _recht = ApiHelper.getGebruikersRecht(_context)
            _groepsWerkJaarId = ApiHelper.GetGroepsWerkJaarId(_recht);
            _groepsWerkJaar = _context.GroepsWerkJaar.Find(_groepsWerkJaarId);
        }

        public override IQueryable<GroepModel> Get()
        {
            var lijst = new List<GroepModel> {new GroepModel(_recht.Groep)};
            return lijst.AsQueryable();
        }

        protected override GroepModel GetEntityByKey([FromODataUri] int key)
        {
            var groep = _context.Groep.Find(key);
            if (groep == null || !MagLezen(groep))
            {
                return null;
            }
            return new GroepModel(groep);
        }

        [Queryable(PageSize = 50)]
        public IQueryable<PersoonModel> GetPersonen(int key)
        {
            var groep = _context.Groep.Find(key);
            if (groep == null || ! MagLezen(groep))
            {
                return null;
            }

            return groep.GelieerdePersoon.Select(p => new PersoonModel(p, _groepsWerkJaar)).AsQueryable();
        }

        [Queryable(PageSize = 12)]
        public IQueryable<AfdelingModel> GetAfdelingen(int key)
        {
            var groep = _context.Groep.Find(key);
            if (groep == null || !MagLezen(groep))
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