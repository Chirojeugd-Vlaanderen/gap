/*
 * Copyright 2014-2015 the GAP developers. See the NOTICE file at the 
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
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.UpdateApi.Models;
using Chiro.Gap.UpdateApi.Workers;
using Nancy;
using Nancy.ModelBinding;

namespace Chiro.Gap.UpdateApi.Modules
{
    public class FoutAdModule: NancyModule, IDisposable
    {
        // disposable, maar wordt gemaakt door DI-container.
        // moet gedisposet worden in Dispose() van deze module.
        private readonly IPersoonUpdater _persoonUpdater;

        public FoutAdModule(IPersoonUpdater persoonUpdater)
        {
            _persoonUpdater = persoonUpdater;

            Post["/foutad"] = _ =>
            {
                int persoonId;
                FoutAdModel model = this.Bind();
                try
                {
                    persoonId = _persoonUpdater.AdNummerVerwijderen(model.AdNummer);
                }
                catch (FoutNummerException ex)
                {
                    if (ex.FoutNummer == FoutNummer.PersoonNietGevonden)
                    {
                        return HttpStatusCode.UnprocessableEntity;
                    }
                    throw;
                }
                string url = String.Format("{0}/Persoon/{1}", this.Request.Url.SiteBase, persoonId);

                Response r = HttpStatusCode.NoContent;
                r.Headers["Location"] = url;
                return r;
            };
        }


        #region Disposable etc

        // Ik heb die constructie met 'disposed' en 'disposing' nooit begrepen.
        // Maar ze zeggen dat dat zo moet :-)

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;
            if (disposing)
            {
                _persoonUpdater.Dispose();
            }
            _disposed = true;
        }

        ~FoutAdModule()
        {
            Dispose(false);
        }

        #endregion
    }
}