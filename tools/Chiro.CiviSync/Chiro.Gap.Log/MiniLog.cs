/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Chiro.Gap.Log
{
	/// <summary>
	/// Basic logging. Logt naar een databasetabel; Gebruikt de connection string met naam 'LogConnection'
	/// uit de config file.
	/// </summary>
	public class MiniLog : IMiniLog
	{
	    /// <summary>
	    /// Log een bericht.
	    /// </summary>
	    /// <param name="niveau">Niveau van het bericht</param>
	    /// <param name="boodschap">Te loggen boodschap</param>
	    /// <param name="stamNummer">Een stamnummer, indien van toepassing</param>
	    /// <param name="adNummer">Een AD-nummer, indien van toepassing</param>
	    /// <param name="persoonId">Een PersoonID, indien van toepassing</param>
	    public void Loggen(Niveau niveau, string boodschap, string stamNummer, int? adNummer, int? persoonId)
	    {
	        string connectionString = ConfigurationManager.ConnectionStrings["LogConnection"].ConnectionString;
	        using (var connection = new SqlConnection(connectionString))
	        {
	            const string commandText =
	                "INSERT INTO logging.Bericht(Niveau, Boodschap, StamNummer, AdNummer, PersoonID) "
	                + "VALUES (@niveau, @boodschap, @stamNummer, @adNummer, @persoonId)";

	            var command = new SqlCommand(commandText, connection);
	            command.Parameters.AddWithValue("@niveau", (int) niveau);
	            command.Parameters.AddWithValue("@boodschap", boodschap);
	            command.Parameters.AddWithValue("@stamNummer", (Object)stamNummer ?? DBNull.Value);
	            command.Parameters.AddWithValue("@adNummer", adNummer);
	            command.Parameters.AddWithValue("@persoonId", persoonId);

                connection.Open();
	            command.ExecuteNonQuery();
                connection.Close();
	        }

	        // Log to console as well.
	        Console.WriteLine("{0} {1} AD{2} ID{3}\n{4}", niveau, stamNummer, adNummer, persoonId, boodschap);
	    } 
	}
}
