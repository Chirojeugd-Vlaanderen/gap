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
    public class AfdelingController : EntitySetController<AfdelingModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GebruikersRecht _recht;
        private readonly GroepsWerkJaar _groepsWerkJaar;

        public AfdelingController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));

            _groepsWerkJaar = _recht.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<AfdelingModel> Get()
        {
            return _groepsWerkJaar.AfdelingsJaar.Select(aj => new AfdelingModel(aj.Afdeling)).AsQueryable();
        }

        protected override AfdelingModel GetEntityByKey([FromODataUri] int key)
        {
            var afdeling = _context.Afdeling.Find(key);
            if (afdeling == null)
            {
                return null;
            }
            return !MagLezen(afdeling) ? null : new AfdelingModel(afdeling);
        }

        public GroepModel GetGroep([FromODataUri] int key)
        {
            var afdeling = _context.Afdeling.Find(key);
            if (afdeling == null)
            {
                return null;
            }
            return !MagLezen(afdeling) ? null : new GroepModel(afdeling.ChiroGroep);
        }

        [Queryable]
        public IQueryable<PersoonModel> GetPersonen([FromODataUri] int key)
        {
            var afdeling = _context.Afdeling.Find(key);
            if (afdeling == null)
            {
                return null;
            }
            var afdelingsJaar = _groepsWerkJaar.AfdelingsJaar.FirstOrDefault(aj => aj.Afdeling.ID == afdeling.ID);
            if (afdelingsJaar == null || ! MagLezen(afdelingsJaar))
            {
                return null;
            }
            
            var leden = afdelingsJaar.Kind.Select(k => new PersoonModel(k));
            var leiding = afdelingsJaar.Leiding.Select(l => new PersoonModel(l));
            return leden.Union(leiding).AsQueryable();
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        private bool MagLezen(AfdelingsJaar afdelingsJaar)
        {
            return Equals(afdelingsJaar.GroepsWerkJaar.Groep, _recht.Groep);
        }

        private bool MagLezen(Afdeling afdeling)
        {
            // juiste groep en de afdeling moet in dit groepswerkjaar zitten
            return Equals(afdeling.ChiroGroep, _recht.Groep) &&
                   _groepsWerkJaar.AfdelingsJaar.Any(aj => aj.Afdeling.ID == afdeling.ID);
        }
    }
}