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
using System.Linq;
using Chiro.Gap.UpdateApi.Client;
using PersonenMergen.Properties;

namespace PersonenMergen
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Count() != 2)
			{
				Console.WriteLine(Resources.Gebruik, Environment.CommandLine);
				Console.ReadLine();
			}
			else
			{
			    var client = new GapUpdateClient();
			    client.Configureren(Settings.Default.Server, Settings.Default.Path,
			        Settings.Default.UserName, Settings.Default.Password);

			    throw new NotImplementedException();
                //client.AdNummerVervangen(int.Parse(args[0]), int.Parse(args[1])));
			}
		}
	}
}
