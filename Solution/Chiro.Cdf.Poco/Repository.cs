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
﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity.Infrastructure;
﻿using System.Linq;

namespace Chiro.Cdf.Poco
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity: BasisEntiteit
    {
        protected readonly IContext Context;

        public Repository(IContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Geeft een queryable voor alle objecten van type <typeparamref name="TEntity"/>,
        /// die gebruikt kan worden voor data access.
        /// </summary>
        /// <param name="paths">Je kunt aan Select een lijstje met entiteiten meegeven die
        /// 'eager geload' moeten worden. Wat niet meegegeven wordt, wordt later on demand
        /// 'lazy geload'.</param>
        /// <returns>queryable voor alle objecten van type <typeparamref name="TEntity"/></returns>
        /// <remarks>Voorbeeldje voor eager loading:
        /// _ledenRepo.Select("GelieerdePersoon.PersoonsAdres") zorgt ervoor dat bij het evalueren
        /// van de query sowieso bij elk lid de gelieerde persoon en diens voorkeuradres 
        /// (GelieerdePersoon.PersoonsAdres) wordt opgehaald. Op die manier vermijden we dat
        /// wanneer later de adressen van alle personen nodig zijn, ze een voor een een nog lazy
        /// opgehaald moeten worden.</remarks>
        public IQueryable<TEntity> Select(params string[] paths)
        {
            var set = this.Context.Set<TEntity>();
            var x = (DbQuery<TEntity>) (set);

            x = paths.Aggregate(x, (current, param) => current.Include(param));

            var result = x.AsQueryable();
            return result;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.Context.Set<TEntity>().AsEnumerable();
        }

        public TEntity GetSingle(Func<TEntity, bool> predicate)
        {
            return this.Context.Set<TEntity>().Single(predicate);
        }

        public TEntity GetFirst(Func<TEntity, bool> predicate)
        {
            return this.Context.Set<TEntity>().First(predicate);
        }

        /// <summary>
        /// Zoekt de entiteit op met de gegeven ID, samen met de gekoppelde entiteiten bepaald door 
        /// <paramref name="paths"/>
        /// </summary>
        /// <param name="id">ID van op te zoeken entiteit</param>
        /// <param name="paths">namen van de eager te loaden gekoppelde entiteiten</param>
        /// <returns>Entiteit met de gegeven ID. <c>null</c> als ze niet is gevonden.</returns>
        public TEntity ByID(int id, params string[] paths)
        {
            DbQuery<TEntity> set = Context.Set<TEntity>();

            set = paths.Aggregate(set, (current, path) => current.Include(path));

            return set.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Zoekt de entiteiten met gegeven <paramref name="IDs"/>.
        /// </summary>
        /// <param name="IDs">ID's van op te halen entiteiten</param>
        /// <returns>
        /// Entiteiten met gegeven <paramref name="IDs"/>. Onbestaande entiteiten worden
        /// genegeerd.
        /// </returns>
        public List<TEntity> ByIDs(IEnumerable<int> IDs)
        {
            return this.Context.Set<TEntity>().Where(x => IDs.Contains(x.ID)).ToList();
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Cannot add a null entity");

            this.Context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Cannot delete a null entity");

            this.Context.Set<TEntity>().Remove(entity);
        }

        public void Delete(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.Context.Set<TEntity>().Remove(entity);
            }
        }

        public void Attach(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Cannot attach a null entity");

            this.Context.Set<TEntity>().Attach(entity);
        }

        /// <summary>
        /// Hash van de achterliggende context. Enkel voor diagnostic purposes.
        /// </summary>
        public int ContextHash
        {
            get { return this.Context.GetHashCode(); }
        }

        public void SaveChanges()
        {
            this.Context.SaveChanges();
        }

        #region Disposable etc

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    //Debug.WriteLine("Disposing context {0}", _context.GetHashCode());
                   this.Context.Dispose();
                }
                disposed = true;
            }
        }

        ~Repository()
        {
            Dispose(false);
        }

        #endregion

    }
}
