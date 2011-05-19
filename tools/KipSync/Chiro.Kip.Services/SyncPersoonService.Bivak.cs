using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Chiro.Kip.Data;
using Chiro.Kip.ServiceContracts.DataContracts;
using Adres = Chiro.Kip.ServiceContracts.DataContracts.Adres;

namespace Chiro.Kip.Services
{
	public partial class SyncPersoonService
	{
		/// <summary>
		/// Bivakaangifte
		/// </summary>
		/// <param name="bivak">gegevens voor de bivakaangifte</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void BivakBewaren(Bivak bivak)
		{
			var feedback = new StringBuilder();

			ChiroGroep groep;
			using (var db = new kipadminEntities())
			{
				// Bestaat de bivakaangifte al in Kipadmin?

				var aangifte = (from a in db.BivakAangifte.Include("Groep").Include("BivakOverzichtB").Include("BivakOverzichtU").Include("BivakOverzichtS")
				                where a.GapUitstapID == bivak.UitstapID
				                select a).FirstOrDefault();

				groep = (from cg in db.Groep.OfType<ChiroGroep>()
				         where String.Compare(cg.STAMNR, bivak.StamNummer, true) == 0
				         select cg).FirstOrDefault();

				if (aangifte == null)
				{
					// Nieuwe maken

					aangifte = new BivakAangifte
					           	{
					           		WerkJaar = bivak.WerkJaar,
					           		DatumVan = bivak.DatumVan,
					           		DatumTot = bivak.DatumTot,
								BivakNaam = bivak.Naam,
								Opmerkingen = bivak.Opmerkingen,
								Groep = groep,
								GapUitstapID = bivak.UitstapID
					           	};
					db.AddToBivakAangifte(aangifte);
					feedback.AppendLine(String.Format("Nieuwe bivakaangifte gemaakt voor {0}: {1}", groep.STAMNR, bivak.Naam));
				}
				else
				{
					// Het enige dat zou mogen veranderen, zijn de data, naam en opmerkingen
					// (Anders zou er vanalles mislopen met de consistentie)
					Debug.Assert(String.Compare(groep.STAMNR.Trim(), bivak.StamNummer.Trim(), true) == 0);

					// Boven 'Trim' toegevoegd, omdat de stamnummers uit GAP soms eindigen op een spatie? 
					// Vermoedelijk omdat de groepscode een CHAR(10) is, en geen VARCHAR(10)?
					// I kan mij niet voorstellen dat enkel hier problemen geeft...

					Debug.Assert(aangifte.WerkJaar == bivak.WerkJaar);
					aangifte.BivakNaam = bivak.Naam;
					aangifte.Opmerkingen = bivak.Opmerkingen;
					aangifte.DatumVan = bivak.DatumVan;
					aangifte.DatumTot = bivak.DatumTot;
					feedback.AppendLine(String.Format("Bivakaangifte geupdatet voor {0}: {1}", groep.STAMNR, bivak.Naam));
				}

				db.SaveChanges();
				feedback.AppendLine(String.Format("BivakAangifteID:", aangifte.ID));
			}
			_log.BerichtLoggen(groep.GroepID, feedback.ToString());
			FixOudeBivakTabel(groep.GroepID, bivak.WerkJaar);
		}

		/// <summary>
		/// Bouw de tabel 'BivakOverzicht' op.  Dit is de oude tabel voor de bivakaangifte, met
		/// 1 'gewoon' bivak, 1 buitenlands bivak, en 1 bivak voor een of meerdere aparte afdelingen.
		/// </summary>
		/// <param name="groepID">ID van groep waarvoor tabel moet worden gefixt</param>
		/// <param name="werkJaar">werkjaar waarvoor de tabel moet worden gefixt</param>
		/// <remarks>Het mag duidelijk zijn dat dit natte-vingerwerk is.</remarks>
		private void FixOudeBivakTabel(int groepID, int werkJaar)
		{
			var feedback = new StringBuilder();

			using (var db = new kipadminEntities())
			{
				BivakAangifte aangifte = null;

				// Kijk of er al een record bestaat in de oude bivaktabel.

				var overzicht = (from bo in db.BivakOverzicht.Include("BivakAangifteB").Include("BivakAangifteS").Include("BivakAangifteU")
				                 where bo.Groep.GroepID == groepID && bo.werkjaar == werkJaar
				                 select bo).FirstOrDefault();

				feedback.AppendLine("Bijwerken bivakoverzicht.");
				
				if (overzicht == null)
				{
					// Maak het record zo nodig aan.

					var groep = (from grp in db.Groep
						     where grp.GroepID == groepID
						     select grp).FirstOrDefault();

					overzicht = new BivakOverzicht
					            	{
					            		Groep = groep,
								werkjaar = werkJaar
					            	};
					db.AddToBivakOverzicht(overzicht);

					feedback.AppendLine("Nieuw overzicht gemaakt");
				}

				// Bivakaangiftes van de groep in het gegeven werkjaar.

				var alleAangiften = from ba in db.BivakAangifte.Include("kipAdres").Include("kipPersoon")
				                where ba.WerkJaar == werkJaar && ba.Groep.GroepID == groepID
				                select ba;

				var binnenlandseAangiften = (from ag in alleAangiften
				                             where ag.kipAdres == null ||
				                             	(String.IsNullOrEmpty(ag.kipAdres.Land) ||
				                             	 String.Compare(ag.kipAdres.Land, Properties.Resources.Belgie, true) != 0)
				                             select ag).OrderByDescending(ag => ag.ID).ToArray();

				var buitenlandseAangiften = (from ag in alleAangiften
							     where ag.kipAdres != null &&
								(!String.IsNullOrEmpty(ag.kipAdres.Land) &&
								 String.Compare(ag.kipAdres.Land, Properties.Resources.Belgie, true) == 0)
							     select ag).OrderByDescending(ag => ag.ID).ToArray();

				// Als er een buitenlandse aangifte is, dan moet die zeker bewaard worden als buitenlands bivak.

				if (buitenlandseAangiften.FirstOrDefault() != null)
				{
					aangifte = buitenlandseAangiften.First();

					overzicht.BivakAangifteU = aangifte;
					overzicht.U_BEGIN = aangifte.DatumVan;
					overzicht.U_EIND = aangifte.DatumTot;
					if (!String.IsNullOrEmpty(aangifte.BivakPlaatsNaam))
					{
						overzicht.U_NAAM = aangifte.BivakPlaatsNaam;
					}

					if (aangifte.kipAdres != null)
					{
						overzicht.U_STRAAT = String.Format("{0} {1} {2}", aangifte.kipAdres.Straat, aangifte.kipAdres.Nr);
						overzicht.U_POSTNR = aangifte.kipAdres.PostNr;
						overzicht.U_GEMEENTE = aangifte.kipAdres.Gemeente;
						overzicht.U_LAND = aangifte.kipAdres.Land;
					}
					// Buitenlands bivak had blijkbaar geen provincie of verantwoordelijke.

					feedback.AppendLine(String.Format("Geregistreerd in overzicht als buitenlands bivak: ID{1} {0}", aangifte.BivakNaam, aangifte.ID));
				}

				// Nu de niet-buitenlandse aangiften.

				if (binnenlandseAangiften.FirstOrDefault() != null)
				{
					if (binnenlandseAangiften.Last() != binnenlandseAangiften.First())
					{
						// Als er meer dan 1 is, dan is de recentste het 'afdelingsbivak', en de 
						// tweede-recentste het 'algemeen' bivak.

						// Arrangeer eerst het 'afdelingsbivak'

						aangifte = binnenlandseAangiften[0];

						overzicht.BivakAangifteS = aangifte;
						overzicht.S_BEGIN = aangifte.DatumVan;
						overzicht.S_EIND = aangifte.DatumTot;
						if (!String.IsNullOrEmpty(aangifte.BivakPlaatsNaam))
						{
							overzicht.S_NAAM = aangifte.BivakPlaatsNaam;
						}

						if (aangifte.kipAdres != null)
						{
							overzicht.S_STRAAT = String.Format("{0} {1} {2}", aangifte.kipAdres.Straat, aangifte.kipAdres.Nr);
							overzicht.S_POSTNR = aangifte.kipAdres.PostNr;
							overzicht.S_GEMEENTE = aangifte.kipAdres.Gemeente;
							overzicht.S_LAND = aangifte.kipAdres.Land;

							overzicht.S_PROVINCI = PostNrNaarProvincie(aangifte.kipAdres.PostNr);
						}

						if (aangifte.kipPersoon != null)
						{
							overzicht.VerantwoordelijkeS = aangifte.kipPersoon;
						}

						feedback.AppendLine(String.Format("Geregistreerd in overzicht als 'afdelingsbivak': ID{1} {0}", aangifte.BivakNaam, aangifte.ID));

						// De op 1 na recentste wordt dan het 'gewone' bivak.

						aangifte = binnenlandseAangiften[1];
					}
					else
					{
						// Als er maar 1 niet-buitenlandse aangifte is, dan wordt dat de 'gewone'
						// bivakaangifte.

						aangifte = binnenlandseAangiften[0];
					}
				}
				else
				{
					// Als er geen niet-buitenlandse bivakken zijn, dan wordt het buitenlands bivak
					// ook het 'gewone' bivak.

					if (buitenlandseAangiften.FirstOrDefault() != null)
					{
						aangifte = buitenlandseAangiften.FirstOrDefault();
					}
				}

				if (aangifte != null)
				{
					overzicht.BivakAangifteB = aangifte;
					overzicht.B_BEGIN = aangifte.DatumVan;
					overzicht.B_EIND = aangifte.DatumTot;

					if (aangifte.kipAdres != null)
					{
						overzicht.B_STRAAT = String.Format("{0} {1} {2}", aangifte.kipAdres.Straat, aangifte.kipAdres.Nr);
						overzicht.B_POSTNR = aangifte.kipAdres.PostNr;
						overzicht.B_GEMEENTE = aangifte.kipAdres.Gemeente;
						overzicht.B_LAND = aangifte.kipAdres.Land;

						overzicht.B_PROVINCI = PostNrNaarProvincie(aangifte.kipAdres.PostNr);
					}

					if (!String.IsNullOrEmpty(aangifte.BivakPlaatsNaam))
					{
						overzicht.B_NAAM = aangifte.BivakPlaatsNaam;
					}

					if (aangifte.kipPersoon != null)
					{
						overzicht.VerantwoordelijkeB = aangifte.kipPersoon;
					}
					feedback.AppendLine(String.Format("Geregistreerd in overzicht als 'gewoon' bivak:  ID{1} {0}", aangifte.BivakNaam, aangifte.ID));
				}
				db.SaveChanges();
				feedback.AppendLine(String.Format("OverzichtsID: {0}", overzicht.id));
			}
			_log.BerichtLoggen(groepID, feedback.ToString());
		}

		/// <summary>
		/// Converteert een postnummer naar een provincie
		/// </summary>
		/// <param name="postNr">Postnummmer, als string</param>
		/// <returns>De kipadmincode voor bijhorende povincie</returns>
		/// <remarks>Ik heb die vreemde codes in Kipadmin ook niet uitgevonden</remarks>
		private static string PostNrNaarProvincie(string postNr)
		{
			int nr;

			if (int.TryParse(postNr, out nr) == false)
			{
				return "PR?";
			}

			if (nr < 1300) return "BHG";	// eigenlijk geen provincie, maar kipadmin weet dat niet
			else if (nr < 1500) return "WBR";
			else if (nr < 2000) return "VBR";
			else if (nr < 3000) return "ANT";
			else if (nr < 3500) return "VBR";	// VBR heeft blijkbaar 2 ranges
			else if (nr < 4000) return "LIM";
			else if (nr < 5000) return "LUK";	// Luik. I didn't invent this.
			else if (nr < 6000) return "NAM";
			else if (nr < 6600) return "HEN";
			else if (nr < 7000) return "LUX";
			else if (nr < 8000) return "HEN";	// Ook 2 ranges voor HEN
			else if (nr < 9000) return "WVL";
			else return "OVL";
		}


		/// <summary>
		/// Bewaart <paramref name="plaatsNaam"/> en <paramref name="adres"/> voor een bivak
		/// in Kipadmin.
		/// </summary>
		/// <param name="uitstapID">ID van de uitstap in GAP</param>
		/// <param name="plaatsNaam">naam van de bivakplaats</param>
		/// <param name="adres">adres van de bivakplaats</param>
		public void BivakPlaatsBewaren(int uitstapID, string plaatsNaam, Adres adres)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stelt de persoon met gegeven <paramref name="adNummer"/> in als contactpersoon voor
		/// het bivak met gegeven <paramref name="uitstapID"/>
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) voor het bivak</param>
		/// <param name="adNummer">AD-nummer contactpersoon bivak</param>
		public void BivakContactBewaren(int uitstapID, int adNummer)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stelt de persoon met gegeven <paramref name="details"/> in als contactpersoon voor
		/// het bivak met gegeven <paramref name="uitstapID"/>
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) voor het bivak</param>
		/// <param name="details">gegevens van de persoon</param>
		/// <remarks>Deze method mag enkel gebruikt worden als het ad-nummer van de
		/// persoon onbestaand of onbekend is.</remarks>
		public void BivakContactBewarenAdOnbekend(int uitstapID, PersoonDetails details)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Verwijdert een bivak uit kipadmin.
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) van het te verwijderen bivak</param>
		public void BivakVerwijderen(int uitstapID)
		{
			throw new NotImplementedException();
		}
	}
}
