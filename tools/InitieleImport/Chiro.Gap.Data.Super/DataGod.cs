using System;
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
		/// gemaakt voor het huidige werkjaar.
		/// </summary>
		/// <param name="stamNr">Stamnummer van de gevraagde groep</param>
		/// <remarks>Als de groep al bestaat, dan worden hoogstens de groepsgegevens geupdatet</remarks>
		public void GroepUitKipadmin(string stamNr)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("data.spNieuweGroepUitKipadmin", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@stamnr", stamNr));

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

				cmd.ExecuteNonQuery();
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

				cmd.ExecuteNonQuery();
			}
		}
	}
}
