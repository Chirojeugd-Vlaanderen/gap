/*
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

using Chiro.Gap.UpdateApi.Workers;
using Nancy;
using System;

namespace Chiro.Gap.UpdateApi.Modules
{
    /// <summary>
    /// Module die raw data oplevert. Zo weinig mogelijk gebruiken aub.
    /// Op dit moment wordt dit gebruikt voor #4326 en #4268, monitoring
    /// van de sync met civi.
    /// </summary>
    /// <remarks>
    /// Het opleveren van data hoort eigenlijk niet echt thuis in een
    /// 'UpdateApi'. UpdateApi is niet de beste naam ooit. Eigenlijk is dit
    /// de api via dewelke Civi GAP aanspreekt.
    /// 
    /// Als we ooit een echte API hebben, kan Civi het via de officiële
    /// manier doen.
    /// </remarks>
    public class RawModule: NancyModule, IDisposable
    {
        private readonly IGapUpdater _gapUpdater;

        public RawModule(IGapUpdater gapUpdater)
        {
            // gapUpdater, aangeleverd door de dependency injection container, is disposable,
            // en moet achteraf vrijgegeven worden. Dat doen we in Dispose() van deze module.

            _gapUpdater = gapUpdater;

            Get["/raw/leden/{werkjaar}"] = parameters => _gapUpdater.AlleLedenRaw(parameters.werkjaar);
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

        ~RawModule()
        {
            Dispose(false);
        }

        #endregion
    }
}