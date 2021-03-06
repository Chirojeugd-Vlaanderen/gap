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

using System;

namespace Chiro.Cdf.Poco
{
    /// <summary>
    /// Een repository provider levert eender welke repository af, en de
    /// (gedeelde) context van al die repository's. 
    /// </summary>
    /// <remarks>
    /// Een repository wordt typisch
    /// door de IOC-container gemaakt op het niveau van de service, en liefst
    /// nergens anders. Op die manier vermijden we dat er verschillende contexten
    /// in elkaars weg gaan lopen.
    /// </remarks>
    public class RepositoryProvider : IRepositoryProvider
    {
        private readonly IContext _context;

        public RepositoryProvider(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creeert een repository voor entiteiten van type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">type van entiteit waarvoor repository gevraagd</typeparam>
        /// <returns>Een repository voor entiteiten van type <typeparamref name="TEntity"/></returns>
        public IRepository<TEntity> RepositoryGet<TEntity>() where TEntity : BasisEntiteit
        {
            return new Repository<TEntity>(_context);
        }

        #region Disposable administratie

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
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        ~RepositoryProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
