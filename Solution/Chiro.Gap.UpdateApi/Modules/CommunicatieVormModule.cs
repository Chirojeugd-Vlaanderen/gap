/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the
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
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.UpdateApi.Models;
using Chiro.Gap.UpdateApi.Workers;
using Nancy;
using Nancy.ModelBinding;

namespace Chiro.Gap.UpdateApi.Modules
{
    public class CommunicatieVormModule: NancyModule, IDisposable
    {
        private readonly IGapUpdater _gapUpdater;

        public CommunicatieVormModule(IGapUpdater gapUpdater)
        {
            _gapUpdater = gapUpdater;

            // curl -X DELETE -d AdNummer=xx -d CommunicatieTypeId=3 -d Nummer=blabla@example.com localhost:50673/groep
            Delete["/communicatievorm"] = _ =>
            {
                CommunicatieVormModel model = this.Bind();
                try
                {
                    _gapUpdater.CommunicatieVormVerwijderen(model);
                }
                catch (FoutNummerException e)
                {
                    if (e.FoutNummer == FoutNummer.CommunicatieVormNietGevonden)
                    {
                        return HttpStatusCode.NotFound;
                    }
                    throw;
                }
                return HttpStatusCode.OK;
            };
        }

        public void Dispose()
        {
            _gapUpdater?.Dispose();
        }
    }
}