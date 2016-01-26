/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy repository. Bevat een lijst entiteiten van type <typeparamref name="T"/>, en voert
    /// zoekopdrachten uit op die lijst.
    /// </summary>
    /// <typeparam name="T">Basisentiteit waarvoor dit een repository is</typeparam>
    /// <remarks>Dit wordt gewoon gebruikt voor unit tests</remarks>
    public class DummyRepo<T>: IRepository<T> where T:BasisEntiteit
    {
        private IList<T> _entiteiten;
        private int _saveCount;


        public DummyRepo(IList<T> entiteiten)
        {
            _entiteiten = entiteiten;
            _saveCount = 0;
        }

        public void Dispose()
        {
        }

        public IQueryable<T> Select(params string[] paths)
        {
            return _entiteiten.AsQueryable();
        }

        public IEnumerable<T> GetAll()
        {
            return _entiteiten;
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _entiteiten.Where(predicate);
        }

        public T GetSingle(Func<T, bool> predicate)
        {
            return _entiteiten.Single(predicate);
        }

        public T GetFirst(Func<T, bool> predicate)
        {
            return _entiteiten.First(predicate);
        }

        public T ByID(int id, params string[] paths)
        {
            return _entiteiten.FirstOrDefault(en => en.ID == id);
        }

        public List<T> ByIDs(IEnumerable<int> IDs)
        {
            return _entiteiten.Where(src => IDs.Contains(src.ID)).ToList();
        }

        public void Add(T entity)
        {
            _entiteiten.Add(entity);
        }

        public void Delete(T entity)
        {
            _entiteiten.Remove(entity);
        }

        public void Delete(IList<T> entities)
        {
            foreach (var entity in entities)
            {
                _entiteiten.Remove(entity);
            }
        }

        public void Attach(T entity)
        {
            throw new NotImplementedException();
        }

        public int ContextHash { get; private set; }
        public void SaveChanges()
        {
            ++_saveCount;
        }

        public int SaveCount
        {
            get { return _saveCount; }
        }
    }
}
