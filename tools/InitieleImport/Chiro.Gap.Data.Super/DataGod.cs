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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Data.Super
{
	/// <summary>
	/// Deze fijne klasse roept 'goddelijke' stored procedures aan ;-)
	/// </summary>
	public class DataGod
	{
		private string ConnectionString { get { return ConfigurationManager.ConnectionStrings["gapdb"].ConnectionString; } }

		/// <summary>
		/// Verwijdert een groep met alwat daaraan gekoppeld is uit de database
		/// </summary>
		/// <param name="stamNr">stamnummer van de te verwijderen groep</param>
		public void GroepVolledigVerwijderen(string stamNr)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("data.spGroepVerwijderen", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@stamnr", stamNr));

				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Maakt een nieuwe groep, op basis van de gegevens in Kipadmin.  Er wordt ook een standaardafdelingsindeling
		/// gemaakt voor werkjaar <paramref name="werkjaar"/>
		/// </summary>
		/// <param name="stamNr">Stamnummer van de gevraagde groep</param>
		/// <param name="werkjaar">Werkjaar waarvoor de afdelingsjaren gemaakt moeten worden</param>
		/// <remarks>Als de groep al bestaat, dan worden hoogstens de groepsgegevens geupdatet</remarks>
		public void GroepUitKipadmin(string stamNr, int werkjaar)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("data.spNieuweGroepUitKipadmin", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@stamnr", stamNr));
				cmd.Parameters.Add(new SqlParameter("@werkjaar", werkjaar));

				cmd.ExecuteNonQuery();
			}
			
		}

		/// <summary>
		/// Zet de huidige leden uit Kipadmin over naar gelieerde personen in GAP.  Adressen komen mee in de 
		/// mate van het mogelijke, communicatie in het geheel niet.
		/// </summary>
		/// <param name="stamNr">Stamnummer van de groep met over te zetten personen</param>
		public void GroepsPersonenUitKipadmin(string stamNr)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("data.spGroepsPersonenUitKipadmin", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@stamnr", stamNr));

				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (SqlException)
				{
					// Dit durft blijkbaar wel eens te crashen op een 
					// conversie-error.  Spijtig dan.
				}
			}
		}

		/// <summary>
		/// Geef gebruker met login <paramref name="userName"/> GAV-rechten op groep met gegeven <paramref name="stamNr"/>.
		/// </summary>
		/// <param name="stamNr">stamnummer van groep</param>
		/// <param name="userName">login van user</param>
		public void RechtenToekennen(string stamNr, string userName)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("auth.spGebruikersRechtToekennen", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@stamnr", stamNr));
				cmd.Parameters.Add(new SqlParameter("@login", userName));

				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					// GVD: er is gepoterd met gebruikersrechten toekennen, zodanig
					// dat die niet meer werkt als de gebruikersrechten er al zijn.
				}
				
			}
		}

		/// <summary>
		/// Ontneem gebruker met login <paramref name="userName"/> GAV-rechten op groep met gegeven 
		/// <paramref name="stamNr"/>.
		/// </summary>
		/// <param name="stamNr">stamnummer van groep</param>
		/// <param name="userName">login van user</param>
		public void RechtenAfnemen(string stamNr, string userName)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("auth.spGebruikersRechtOntnemen", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@stamnr", stamNr));
				cmd.Parameters.Add(new SqlParameter("@login", userName));

				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Verwijdert groepwswerkjaar van werkjaar <paramref name="p"/> van groep met stamnummer
		/// <paramref name="stamNr"/>.
		/// </summary>
		/// <param name="stamNr">Stamnummer van groep met te verwijderen groepswerkjaar</param>
		/// <param name="p">Werkjaar</param>
		public void GroepsWerkJaarVerwijderen(string stamNr, int p)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("data.spGroepsWerkJaarVerwijderen", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@stamnr", stamNr));
				cmd.Parameters.Add(new SqlParameter("@werkjaar", p));

				cmd.ExecuteNonQuery();
			}
		}
	}
}
