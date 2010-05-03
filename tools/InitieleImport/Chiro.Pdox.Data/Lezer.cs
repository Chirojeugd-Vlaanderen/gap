using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

using Chiro.Gap.Domain;
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

		/// <summary>
		/// Haalt alle relevante informatie op over de personen in de paradoxtabellen, in een rij
		/// 'PersoonLidInfo'.  Dit is een voorlopig zeer mottig datacontract, maar ik gebruik het bij
		/// gebrek aan beter.  De koppeling van LidInfo naar PersoonDetail zal wel null blijven.
		/// </summary>
		/// <returns>Lijst met info van alle personen in het bestand</returns>
		public IEnumerable<PersoonLidInfo> PersonenOphalen()
		{
			using (var connectie = new OleDbConnection(_connectionString))
			{
				// codes gebruikt in e-mailbestanden voor man en vrouw.

				const string PDOXMAN = "M";
				const string PDOXVROUW = "V";

				var resultaat = new List<PersoonLidInfo>();

				connectie.Open();

				var query = new OleDbCommand(
					"SELECT NAAM, VOORNAAM, GEBDATUM, GESLACHT FROM PERSOON",
					connectie);
				var reader = query.ExecuteReader();

				while (reader.Read())
				{
					GeslachtsType geslacht;

					if (String.Compare(reader["GESLACHT"].ToString(), PDOXMAN, true) == 0)
					{
						geslacht = GeslachtsType.Man;
					}
					else if (String.Compare(reader["GESLACHT"].ToString(), PDOXVROUW, true) == 0)
					{
						geslacht = GeslachtsType.Vrouw;
					}
					else
					{
						geslacht = GeslachtsType.Onbekend;
					}

					resultaat.Add(new PersoonLidInfo()
					              	{	
					              		PersoonDetail = new PersoonDetail()
					              			{
					              				Naam = reader["NAAM"].ToString(),
					              				VoorNaam = reader["VOORNAAM"].ToString(),
					              				GeboorteDatum = DateTime.Parse(reader["GEBDATUM"].ToString()),
					              				Geslacht = geslacht
					              			}
					              	});
				}

				return resultaat;
			}			
		}
	}
}
