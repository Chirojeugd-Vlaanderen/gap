using System;
using System.Data.OleDb;
using System.Text;

using Chiro.Gap.ServiceContracts;

namespace Chiro.Pdox.Data
{
	/// <summary>
	/// Klasse om data te lezen uit een e-mailbestand van het oude chirogroepprogramma
	/// </summary>
	public class Lezer
	{
		private string _connectionString;

		/// <summary>
		/// Creeert een nieuwe lezer
		/// </summary>
		/// <param name="path">pad van de directory met daarin de paradoxtabellen</param>
		public Lezer(string path)
		{
			var builder = new StringBuilder("");

			builder.Append(@"Provider=Microsoft.Jet.OLEDB.4.0;");
			builder.Append(@"Extended Properties=Paradox 5.x;");
			builder.Append(String.Format("Data Source={0};", path));

			_connectionString = builder.ToString();
		}

		/// <summary>
		/// Haalt groep op uit de paradoxdatabase
		/// </summary>
		/// <returns>GroepInfo van de gevraagde groep</returns>
		public GroepInfo GroepOphalen()
		{
			using (var connectie = new OleDbConnection(_connectionString))
			{
				string plaats;

				connectie.Open();

				var query = new OleDbCommand(
					"SELECT STAMNR, NAAM, GEMEENTE, PAROCHIE FROM GROEP",
					connectie);
				var reader = query.ExecuteReader();

				reader.Read();

				var parochie = reader["PAROCHIE"].ToString();
				var gemeente = reader["GEMEENTE"].ToString();

				if (parochie.Trim() != String.Empty
					&& String.Compare(parochie, gemeente, true) != 0)
				{
					plaats = String.Format("{0} - {1}", parochie, gemeente);
				}
				else
				{
					plaats = gemeente;
				}

				return new GroepInfo
				       	{
						ID = 0,
				       		StamNummer = reader["STAMNR"].ToString(),
				       		Naam = reader["NAAM"].ToString(),
						Plaats = plaats
				       	};

			}
		}
	}
}
