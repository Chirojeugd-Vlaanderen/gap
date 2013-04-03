/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Algemeen.Data.Entity
{
    public class RepositoryBase : IRepository
    {
        private readonly IDbContext _dbc;

        public RepositoryBase(IDbContext dbContext)
        {
            _dbc = dbContext;
        }

        public void WijzigingenBewaren()
        {
            _dbc.SaveChanges();
        }

        public IEnumerable<TEntiteit> Alles<TEntiteit>() where TEntiteit: class, IBasisEntiteit
        {
            return _dbc.Set<TEntiteit>();
        }

        public TEntiteit Toevoegen<TEntiteit>(TEntiteit entiteit) where TEntiteit: class, IBasisEntiteit
        {
            return _dbc.Set<TEntiteit>().Add(entiteit);
        }

        public TEntiteit Ophalen<TEntiteit>(int ID) where TEntiteit : class, IBasisEntiteit
        {
            return _dbc.Set<TEntiteit>().Where(ent => ent.ID == ID).FirstOrDefault();
        }

        public TEntiteit Ophalen<TEntiteit>(int ID, params Expression<Func<TEntiteit, object>>[] paths) where TEntiteit : class, IBasisEntiteit
        {
            var query = _dbc.Set<TEntiteit>().Where(ent => ent.ID == ID);
            var result = IncludesToepassen(query, paths);
            return result.FirstOrDefault();
        }

        public void Verwijderen<TEntiteit>(TEntiteit entiteit) where TEntiteit : class, IBasisEntiteit
        {
            _dbc.Set<TEntiteit>().Remove(entiteit);
        }

        /// <summary>
        /// Includet gerelateerde entiteiten in een objectquery
        /// </summary>
        /// <param name="query">Betreffende query</param>
        /// <param name="paths">Paden naar mee op te halen gerelateerde entiteiten</param>
        /// <returns>Nieuwe query die de gevraagde entiteiten Includet</returns>
        protected IQueryable<TEntiteit> IncludesToepassen<TEntiteit>(IQueryable<TEntiteit> query, Expression<Func<TEntiteit, object>>[] paths) where TEntiteit : class, IBasisEntiteit
        {
            var resultaat = query;

            if (paths != null)
            {
                // Workaround om de Includes te laten werken
                // overgenomen van 
                // http://blogs.msdn.com/alexj/archive/2009/06/02/tip-22-how-to-make-include-really-include.aspx
                // en daarna via ReSharper omgezet in een Linq-expressie
                resultaat = paths.Aggregate(resultaat, (current, v) => current.Include(v));
            }

            return resultaat;
        }
    }
}