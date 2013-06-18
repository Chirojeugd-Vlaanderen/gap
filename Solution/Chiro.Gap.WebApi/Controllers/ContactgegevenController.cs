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
    public class ContactgegevenController : EntitySetController<ContactgegevenModel, int>
    {
        private readonly ChiroGroepEntities _context = new ChiroGroepEntities();
        private readonly GroepsWerkJaar _groepsWerkJaar;
        private readonly GebruikersRecht _recht;

        public ContactgegevenController()
        {
            _recht = _context.GebruikersRecht.First(g => g.Gav.Login == HttpContext.Current.User.Identity.Name &&
                                                         (g.VervalDatum == null || g.VervalDatum > DateTime.Now));
            _groepsWerkJaar = _recht.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();
        }

        [Queryable(PageSize = 10)]
        public override IQueryable<ContactgegevenModel> Get()
        {
            // In plaats van eerst alle personen op te halen en dan van elke persoon de
            // communicatievorm, kunnen we dit met SelectMany in 1 expressie schrijven
            return
                _recht.Groep.GelieerdePersoon.SelectMany(gp => gp.Communicatie.Select(cv => new ContactgegevenModel(cv)))
                      .AsQueryable();
        }

        protected override ContactgegevenModel GetEntityByKey([FromODataUri] int key)
        {
            CommunicatieVorm communicatieVorm = _context.CommunicatieVorm.Find(key);
            if (communicatieVorm == null)
            {
                return null;
            }
            return !MagLezen(communicatieVorm) ? null : new ContactgegevenModel(communicatieVorm);
        }

        public PersoonModel GetPersoon([FromODataUri] int key)
        {
            CommunicatieVorm communicatieVorm = _context.CommunicatieVorm.Find(key);
            if (communicatieVorm == null)
            {
                return null;
            }
            return !MagLezen(communicatieVorm)
                       ? null
                       : new PersoonModel(communicatieVorm.GelieerdePersoon, _groepsWerkJaar);
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