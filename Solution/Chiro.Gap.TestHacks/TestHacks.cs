﻿/*
 * Copyright 2013-2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Data;
using System.Data.SqlClient;
using Chiro.Gap.TestHacks.Properties;

namespace Chiro.Gap.TestHacks
{
    public class TestHacks
    {
        /// <summary>
        /// Erg lelijke hack die direct in de database schrijft om een gebruiker met naam 
        /// <paramref name="userName"/> toegang te geven tot een testgroep.
        /// </summary>
        /// <param name="userName">naam van de gebruiker die toegang moet krijgen</param>
        public static int TestGroepToevoegen(string userName)
        {
            int result;

            using (var connection = new SqlConnection(Settings.Default.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("auth.spWillekeurigeGroepToekennen", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@login", userName));
                result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
            return result;
        }
    }
}