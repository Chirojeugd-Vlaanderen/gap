using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

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

				var result = new GroepInfo
				       	{
						ID = 0,
				       		StamNummer = reader["STAMNR"].ToString(),
				       		Naam = reader["NAAM"].ToString(),
						Plaats = plaats
				       	};

				connectie.Close();

				return result;

			}
		}

		/// <summary>
		/// Haalt alle relevante informatie op over de personen in de paradoxtabellen, in een rij
		/// 'PersoonLidInfo'.  PersoonLidInfo is een beetje een draak van een datacontract; enkel
		/// de relevante velden worden ingevuld.
		/// </summary>
		/// <returns>Lijst met info van alle personen in het bestand</returns>
		public IEnumerable<PersoonLidInfo> PersonenOphalen()
		{
			var helper = new ImportHelper();

			using (var connectie = new OleDbConnection(_connectionString))
			{
				// codes gebruikt in e-mailbestanden voor man en vrouw.

				const string PDOXMAN = "M";
				const string PDOXVROUW = "V";
				
				// codes gebruikt voor kind en leiding

				const string PDOXKIND = "LD";
				const string PDOXLEIDING = "LE";

				var resultaat = new List<PersoonLidInfo>();

				connectie.Open();

				var query = new OleDbCommand(
					"SELECT NAAM, VOORNAAM, GEBDATUM, GESLACHT, ADNR, AANSL_NR, SOORT, " + 
                                        "	STRAAT_NR, POSTNR, GEMEENTE, LAND, STRAAT_NR2, POSTNR2, GEMEENTE2, LAND2, POST_OP, " +
                                        "	TEL, TEL2, FAX, E_MAIL " +
					"FROM PERSOON LEFT OUTER JOIN LID ON PERSOON.NR = LID.PERS_NR ",
					connectie);
				var reader = query.ExecuteReader();

				while (reader.Read())
				{
					GeslachtsType geslacht;
					LidInfo lid;
					DateTime gebDatum;

					int adInt;
					int? adNr;

					if (int.TryParse(reader["ADNR"].ToString().Trim(), out adInt))
					{
						adNr = adInt;
					}
					else
					{
						adNr = null;
					}

					DateTime.TryParse(reader["GEBDATUM"].ToString(), out gebDatum);

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

					if (reader["AANSL_NR"] == null || (String.Compare(reader["SOORT"].ToString(), PDOXKIND, true) != 0
						&& String.Compare(reader["SOORT"].ToString(), PDOXLEIDING, true) != 0) )
					{
						lid = null;
					}
					else
					{
						LidType lt = String.Compare(reader["SOORT"].ToString(), PDOXKIND, true) == 0 ? LidType.Kind : LidType.Leiding;

						lid = new LidInfo {Type = lt};
					}

					#region adressen
					var adressen = new List<PersoonsAdresInfo>();
					
					bool eersteAdresThuis = (int.Parse(reader["POST_OP"].ToString()) == 1);
					var persoonsAdres = helper.MaakAdresInfo(
						reader["STRAAT_NR"].ToString(),
						reader["POSTNR"].ToString(),
						reader["GEMEENTE"].ToString(),
						reader["LAND"].ToString(),
						eersteAdresThuis ? AdresTypeEnum.Thuis : AdresTypeEnum.Overig);

					if (persoonsAdres != null)
					{
						adressen.Add(persoonsAdres);
					}

					persoonsAdres = helper.MaakAdresInfo(
						reader["STRAAT_NR2"].ToString(),
						reader["POSTNR2"].ToString(),
						reader["GEMEENTE2"].ToString(),
						reader["LAND2"].ToString(),
						!eersteAdresThuis ? AdresTypeEnum.Thuis : AdresTypeEnum.Overig);

					if (persoonsAdres != null)
					{
						adressen.Add(persoonsAdres);
					}
					#endregion

					#region communicatie

					var communicatie = new List<CommunicatieInfo>();
					string tel1 = helper.FormatteerTelefoonNr(reader["TEL"].ToString());
					string tel2 = helper.FormatteerTelefoonNr(reader["TEL2"].ToString());
					string fax = helper.FormatteerTelefoonNr(reader["FAX"].ToString());
					string eMail = reader["E_MAIL"].ToString();

					if (tel1 != null)
					{
						communicatie.Add(new CommunicatieInfo
						                 	{
						                 		CommunicatieTypeID = 1,
						                 		Nummer = tel1,
						                 		Voorkeur = true
						                 	});
					}

					if (tel2 != null)
					{
						communicatie.Add(new CommunicatieInfo
						{
							CommunicatieTypeID = 1,
							Nummer = tel2,
							Voorkeur = false
						});
					}

					if (fax != null)
					{
						communicatie.Add(new CommunicatieInfo
						{
							CommunicatieTypeID = 2,
							Nummer = fax,
							Voorkeur = true
						});
					}

					if (!string.IsNullOrEmpty(eMail))
					{
						communicatie.Add(new CommunicatieInfo
						{
							CommunicatieTypeID = 3,
							Nummer = eMail,
							Voorkeur = true
						});
					}

					#endregion

					var persoonLidInfo = new PersoonLidInfo()
	                                	{	
	                                		PersoonDetail = new PersoonDetail()
	                                		                	{
	                                		                		AdNummer = adNr,
	                                		                		Naam = reader["NAAM"].ToString(),
	                                		                		VoorNaam = reader["VOORNAAM"].ToString(),
	                                		                		GeboorteDatum = (gebDatum == DateTime.MinValue ? null: (DateTime?)gebDatum),
	                                		                		Geslacht = geslacht,
	                                		                	},
	                                		LidInfo = lid,
							PersoonsAdresInfo = adressen,
							CommunicatieInfo = communicatie
	                                	};


					resultaat.Add(persoonLidInfo);
				}

				connectie.Close();
				return resultaat;
			}			
		}

		/// <summary>
		/// Haalt persoonsgegevens en lidgegevens op van leden in paradox.
		/// </summary>
		/// <returns>De opgehaalde gegevens in een rij PersoonLidInfo</returns>
		public IEnumerable<PersoonLidInfo> LedenOphalen()
		{
			var helper = new ImportHelper();

			using (var connectie = new OleDbConnection(_connectionString))
			{
				// codes gebruikt in e-mailbestanden voor man en vrouw.

				const string PDOXMAN = "M";
				const string PDOXVROUW = "V";

				// codes gebruikt voor kind en leiding

				const string PDOXKIND = "LD";
				const string PDOXLEIDING = "LE";

				var resultaat = new List<PersoonLidInfo>();

				connectie.Open();

				var query = new OleDbCommand(
					"SELECT NAAM, VOORNAAM, GEBDATUM, GESLACHT, ADNR, AANSL_NR, SOORT, " +
					"	STRAAT_NR, POSTNR, GEMEENTE, LAND, STRAAT_NR2, POSTNR2, GEMEENTE2, LAND2, POST_OP, " +
					"	TEL, TEL2, FAX, E_MAIL " +
					"FROM PERSOON LEFT OUTER JOIN LID ON PERSOON.NR = LID.PERS_NR "
					 + "WHERE SOORT=@pdoxkind OR SOORT=@pdoxleiding"
					,
					connectie);

				query.Parameters.Add(new OleDbParameter("@pdoxkind", PDOXKIND));
				query.Parameters.Add(new OleDbParameter("@pdoxleiding", PDOXLEIDING));

				var reader = query.ExecuteReader();

				while (reader.Read())
				{
					GeslachtsType geslacht;
					LidInfo lid;
					DateTime gebDatum;


					int adInt;
					int? adNr;

					if (int.TryParse(reader["ADNR"].ToString().Trim(), out adInt))
					{
						adNr = adInt;
					}
					else
					{
						adNr = null;
					}

					DateTime.TryParse(reader["GEBDATUM"].ToString(), out gebDatum);

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

					if (reader["AANSL_NR"] == null)
					{
						lid = null;
					}
					else
					{
						LidType lt = String.Compare(reader["SOORT"].ToString(), PDOXKIND, true) == 0 ? LidType.Kind : LidType.Leiding;

						lid = new LidInfo {Type = lt};
					}

					#region adressen

					var adressen = new List<PersoonsAdresInfo>();

					bool eersteAdresThuis = (int.Parse(reader["POST_OP"].ToString()) == 1);
					var persoonsAdres = helper.MaakAdresInfo(
						reader["STRAAT_NR"].ToString(),
						reader["POSTNR"].ToString(),
						reader["GEMEENTE"].ToString(),
						reader["LAND"].ToString(),
						eersteAdresThuis ? AdresTypeEnum.Thuis : AdresTypeEnum.Overig);

					if (persoonsAdres != null)
					{
						adressen.Add(persoonsAdres);
					}

					persoonsAdres = helper.MaakAdresInfo(
						reader["STRAAT_NR2"].ToString(),
						reader["POSTNR2"].ToString(),
						reader["GEMEENTE2"].ToString(),
						reader["LAND2"].ToString(),
						!eersteAdresThuis ? AdresTypeEnum.Thuis : AdresTypeEnum.Overig);

					if (persoonsAdres != null)
					{
						adressen.Add(persoonsAdres);
					}

					#endregion

					#region communicatie

					var communicatie = new List<CommunicatieInfo>();
					string tel1 = helper.FormatteerTelefoonNr(reader["TEL"].ToString());
					string tel2 = helper.FormatteerTelefoonNr(reader["TEL2"].ToString());
					string fax = helper.FormatteerTelefoonNr(reader["FAX"].ToString());
					string eMail = reader["E_MAIL"].ToString();

					if (tel1 != null)
					{
						communicatie.Add(new CommunicatieInfo
						                 	{
						                 		CommunicatieTypeID = 1,
						                 		Nummer = tel1,
						                 		Voorkeur = true
						                 	});
					}

					if (tel2 != null)
					{
						communicatie.Add(new CommunicatieInfo
						                 	{
						                 		CommunicatieTypeID = 1,
						                 		Nummer = tel2,
						                 		Voorkeur = false
						                 	});
					}

					if (fax != null)
					{
						communicatie.Add(new CommunicatieInfo
						                 	{
						                 		CommunicatieTypeID = 2,
						                 		Nummer = fax,
						                 		Voorkeur = true
						                 	});
					}

					if (!string.IsNullOrEmpty(eMail))
					{
						communicatie.Add(new CommunicatieInfo
						                 	{
						                 		CommunicatieTypeID = 3,
						                 		Nummer = eMail,
						                 		Voorkeur = true
						                 	});
					}

					#endregion

					var persoonLidInfo = new PersoonLidInfo()
					                     	{
					                     		PersoonDetail = new PersoonDetail()
					                     		                	{
					                     		                		AdNummer = adNr,
					                     		                		Naam = reader["NAAM"].ToString(),
					                     		                		VoorNaam = reader["VOORNAAM"].ToString(),
					                     		                		GeboorteDatum =
					                     		                			(gebDatum == DateTime.MinValue ? null : (DateTime?) gebDatum),
					                     		                		Geslacht = geslacht,
					                     		                	},
					                     		LidInfo = lid,
					                     		PersoonsAdresInfo = adressen,
					                     		CommunicatieInfo = communicatie
					                     	};


					resultaat.Add(persoonLidInfo);
				}

				connectie.Close();
				return resultaat;
			}
			
		}
	}
}
