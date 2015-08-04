/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Gap.Sync;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Klasse die op de scope van de service een aantal zaken regelt
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// Acties die uitgevoerd moeten worden wanneer de applicatie start
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected void Application_Start(object sender, EventArgs e)
        {
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren(); // mappings voor sync

            // De mappings voor de servicecontracts worden hier niet meer gedefinieerd,
            // maar expliciet in de constructor van de services. Op die manier konden
            // de mappers gebruik maken van geinjecteerde managers. (Zie #3150)
        }

        /// <summary>
        /// Acties die uitgevoerd moeten worden wanneer een gebruiker de applicatie opent
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected void Session_Start(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Acties die uitgevoerd moeten worden VOOR elke gebruikersrequest 
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Acties die uitgevoerd moeten worden op het moment dat de rechten van de gebruiker
        /// nagekeken worden
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Acties die uitgevoerd moeten worden op het moment dat er een niet-opgevangen fout optreedt
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected void Application_Error(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Acties die uitgevoerd moeten worden wanneer een gebruiker de applicatie afsluit of lang genoeg niet gebruikt
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Acties die uitgevoerd moeten worden wanneer de applicatie gestopt wordt
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}