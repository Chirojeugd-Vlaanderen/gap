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

using System.ComponentModel;
using System.ServiceProcess;

namespace Chiro.Gap.UpdateSvc.ServiceHost
{
    /// <summary>
    /// Class die Updateservice helpt installeren
    /// </summary>
    [RunInstaller(true)]
	public partial class Installer1 : System.Configuration.Install.Installer
	{
        /// <summary>
        /// Constructor. Zorgt voor de nodige configuratie.
        /// </summary>
        public Installer1()
		{
			InitializeComponent();

			var processInstaller = new ServiceProcessInstaller();
			var serviceInstaller = new ServiceInstaller();

			processInstaller.Account = ServiceAccount.User;
			processInstaller.Username = @"KIPDORP\GapService";
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			serviceInstaller.ServiceName = "GapUpdate";

			Installers.Add(serviceInstaller);
			Installers.Add(processInstaller);
		}
	}
}
