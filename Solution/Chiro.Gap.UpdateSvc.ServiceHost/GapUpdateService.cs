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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;

using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;
using Chiro.Gap.UpdateSvc.Service;

namespace Chiro.Gap.UpdateSvc.ServiceHost
{
    /// <summary>
    /// De service die updates doorgeeft tussen GAP en KipAdmin
    /// </summary>
    public partial class GapUpdateService : ServiceBase
    {
        private ServiceHost<UpdateService> _host = null;

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        public GapUpdateService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Regelt wat er allemaal moet gebeuren wanneer de service gestart wordt
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("GapUpdate service wordt gestart.");
            Factory.ContainerInit();

            _host = new ServiceHost<UpdateService>();
            _host.Open();
        }

        /// <summary>
        /// Regelt wat er allemaal moet gebeuren wanneer de service gestopt wordt
        /// </summary>
        protected override void OnStop()
        {
            if (_host != null)
            {
                Trace.WriteLine("GapUpdate service wordt gestopt.");

                _host.Close();
                _host = null;
            }
        }
    }
}