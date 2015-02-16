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
using System.ServiceProcess;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;
using Chiro.CiviSync.Helpers;
using Chiro.CiviSync.Services;

namespace Chiro.CiviSync.SyncServiceHost
{
	public partial class CiviSyncServiceBase : ServiceBase
	{
		private ServiceHost<SyncService> _host = null;

		public CiviSyncServiceBase()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
		    Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();
			Trace.WriteLine("CiviSync wordt gestart");

            _host = new ServiceHost<SyncService>();

			_host.Open();
		}

		protected override void OnStop()
		{
			if (_host != null)
			{
				Trace.WriteLine("CiviSync wordt gestopt");

				_host.Close();
				_host = null;
			}
		}
	}
}
