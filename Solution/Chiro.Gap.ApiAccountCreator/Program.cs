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
using Chiro.Gap.Api;

namespace Chiro.Gap.ApiAccountCreator
{
    /// <summary>
    /// Dom command line tooltje om API-accounts te maken.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            bool showUsage = false;
            int adnr = 0;

            if (args.Length < 2 || String.IsNullOrEmpty(args[0]))
            {
                showUsage = true;
            }
            else
            {
                showUsage = !int.TryParse(args[1], out adnr);
            }

            if (showUsage)
            {
                Console.WriteLine("USAGE: {0} username adnr [--force]",
                    System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
                );                
            }
            else
            {
                var mgr = new ApiAccountManager();
                string pw = mgr.Register(args[0], adnr, args.Length >= 3 && args[2] == "--force");

                if (pw == null)
                {
                    Console.WriteLine("User already exists. Append --force to delete existing user.");
                }
                else
                {
                    Console.WriteLine("Password: {0}", pw);
                }
            }
        }
    }
}
