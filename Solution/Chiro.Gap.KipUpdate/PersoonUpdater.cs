using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Chiro.Gap.KipUpdate
{
	/// <summary>
	/// Updates die Kipadmin moet kunnen uitvoeren op personen
	/// </summary>
	public class PersoonUpdater : IPersoonUpdater
	{
		private string ConnectionString { get { return ConfigurationManager.ConnectionStrings["kipsync"].ConnectionString; } }

		/// <summary>
		/// Geeft de persoon met ID <paramref name="persoonID"/> het AD-nummer <paramref name="adNummer"/>.
		/// </summary>
		/// <param name="persoonID">ID van persoon die AD-nummer moet krijgen</param>
		/// <param name="adNummer">toe te kennen AD-nummer</param>
		public void AdNummerZetten(int persoonID, int adNummer)
		{
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				var cmd = new SqlCommand("pers.spAdNummerzetten", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				
				cmd.Parameters.Add(new SqlParameter("@PersoonID", persoonID));
				cmd.Parameters.Add(new SqlParameter("@AdNummer", adNummer));

				cmd.ExecuteNonQuery();
			}
		}
	}
}
