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

using System;

using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;
using Chiro.Gap.Sync;
using Chiro.Gap.UpdateSvc.Service;

namespace Chiro.Gap.UpdateSvc.ConsoleServiceHost
{
	class Program
	{
		static void Main(string[] args)
		{
			Factory.ContainerInit();
		    MappingHelper.MappingsDefinieren();

			// ServiceHost<T> is een type dat we zelf declareren in Chiro.Cdf.ServiceModel, maar
			// eigenlijk is onderstaande lijn min of meer gelijkaardig aan
			// var host = new ServiceHost(typeof(UpdateService));
			//
            // TODO (#1059): Nakijken of die custom ServiceHost<T> wel de moeite is om te behouden.

			var host = new ServiceHost<UpdateService>();

			host.Open();
			Console.WriteLine("Hit enter to stop the service.");
			Console.ReadKey();

			host.Close();
		}
	}
}
