﻿/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
    public class GroepModule: NancyModule, IDisposable
    {
        private readonly IGapUpdater _gapUpdater;

        public GroepModule(IGapUpdater gapUpdater)
        {
            // gapUpdater, aangeleverd door de dependency injection container, is disposable,
            // en moet achteraf vrijgegeven worden. Dat doen we in Dispose() van deze module.

            _gapUpdater = gapUpdater;

            // curl -X PUT -d StamNummer=AG\ /0711 -d StopDatum=2012-08-21 localhost:50673/groep
            // (laat -d StopDatum=... weg om stopdatum te clearen.)
            Put["/groep"] = _ =>
            {
                GroepModel model = this.Bind();
                try
                {
                    _gapUpdater.GroepDesactiveren(model.StamNummer, model.StopDatum);
                }
                catch (FoutNummerException ex)
                {
                    if (ex.FoutNummer == FoutNummer.GroepNietGevonden)
                    {
                        return HttpStatusCode.NotFound;
                    }
                    throw;
                }
                return HttpStatusCode.OK;
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
                _gapUpdater.Dispose();
            }
            _disposed = true;
        }

        ~GroepModule()
        {
            Dispose(false);
        }

        #endregion
    }
}