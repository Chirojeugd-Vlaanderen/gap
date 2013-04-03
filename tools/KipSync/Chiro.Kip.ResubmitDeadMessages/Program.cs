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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Chiro.Kip.ResubmitDeadMessages
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("<ENTER> to start...");
			Console.ReadLine();

			using (var serviceHost = new ServiceHost(typeof(DlqSyncPersoonService)))
			{
				serviceHost.Open();

				Console.WriteLine("The dead letter service is ready.");
				Console.WriteLine("Press <ENTER> to terminate service.");

				Console.ReadLine();

				serviceHost.Close();
			}
		}
	}
}
