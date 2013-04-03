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
﻿using System;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;

namespace Chiro.Kip.ConsoleServiceHost
{
	class Program
	{
		static void Main(string[] args)
		{
			Factory.ContainerInit();

			var host = new ServiceHost<Chiro.Kip.Services.SyncPersoonService>();

			// Als je hieronder een foutmelding krijgt, dan moet je je gebruiker de toestemming geven 
			// om iets open te zetten op poort 8000 (voor metadata exchange)
			// Voor Windows 7 en aanverwanten gaat dat als volgt:
			// netsh http add urlacl url=http://+:8000/ user=DOMAIN\user
			// zie http://go.microsoft.com/fwlink/?LinkId=70353
			host.Open();

			Console.WriteLine("Hit Enter to stop the service");
			Console.ReadKey();

			host.Close();

		}
	}
}
