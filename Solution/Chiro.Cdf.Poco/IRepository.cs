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
using System.Linq;

namespace Chiro.Cdf.Poco
{
    public interface IRepository<TEntity> : IDisposable where TEntity: BasisEntiteit
    {
        //IQueryable<TEntity> Select();
        IQueryable<TEntity> Select(params string[] paths);
        IEnumerable<TEntity> GetAll();
        // Dit is een gevaarlijke functie, want hiermee evalueren we op SQL en worden nadien alle selecties in RAM uitgevoerd
        // terwijl dat dat helemaal niet zo duidelijk is. Voorlopig laten we dit weg dus.
        // IEnumerable<TEntity> Where(Func<TEntity, bool> predicate);
        TEntity GetSingle(Func<TEntity, bool> predicate);
        TEntity GetFirst(Func<TEntity, bool> predicate);

        /// <summary>
        /// Zoekt de entiteit op met de gegeven ID
        /// </summary>
        /// <param name="id">ID van op te zoeken entiteit</param>
        /// <returns>Entiteit met de gegeven ID. <c>null</c> als ze niet is gevonden.</returns>
        TEntity ByID(int id);

        /// <summary>
        /// Zoekt de entiteiten met gegeven <paramref name="IDs"/>.
        /// </summary>
        /// <param name="IDs">ID's van op te halen entiteiten</param>
        /// <returns>
        /// Entiteiten met gegeven <paramref name="IDs"/>. Onbestaande entiteiten worden
        /// genegeerd.
        /// </returns>
        List<TEntity> ByIDs(IEnumerable<int> IDs);

        void Add(TEntity entity);

        /// <summary>
        /// Verwijdert de gegeven <paramref name="entity"/>
        /// </summary>
        /// <param name="entity">Te verwijderen entity</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Verwijdert de gegeven <paramref name="entities"/>
        /// </summary>
        /// <param name="entities">Te verwijderen entititeiten</param>
        void Delete(IList<TEntity> entities);

        void Attach(TEntity entity);

        /// <summary>
        /// Hash code van de achterliggende context. Enkel voor diagnostic purposes.
        /// </summary>
        int ContextHash { get; }
        
        void SaveChanges();
    }
}
