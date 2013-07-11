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
        /// <summary>
        /// Levert een queryable op voor entiteiten van het type <typeparamref name="TEntity"/>, met eager loading
        /// voor de gekoppelde entiteiten gegeven door <paramref name="paths"/>
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
        IQueryable<TEntity> Select(params string[] paths);

        IEnumerable<TEntity> GetAll();
        TEntity GetSingle(Func<TEntity, bool> predicate);
        TEntity GetFirst(Func<TEntity, bool> predicate);

        /// <summary>
        /// Zoekt de entiteit op met de gegeven ID, samen met de gekoppelde entiteiten bepaald door 
        /// <paramref name="paths"/>
        /// </summary>
        /// <param name="id">ID van op te zoeken entiteit</param>
        /// <param name="paths">namen van de eager te loaden gekoppelde entiteiten</param>
        /// <returns>Entiteit met de gegeven ID. <c>null</c> als ze niet is gevonden.</returns>
        TEntity ByID(int id, params string[] paths);

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
