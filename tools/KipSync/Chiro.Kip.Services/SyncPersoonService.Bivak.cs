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
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Chiro.Kip.Data;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Kip.Workers;
using Adres = Chiro.Kip.ServiceContracts.DataContracts.Adres;
using Persoon = Chiro.Kip.Data.Persoon;

namespace Chiro.Kip.Services
{
	public partial class SyncPersoonService
	{
		/// <summary>
		/// Bivakaangifte
		/// </summary>
		/// <param name="bivak">gegevens voor de bivakaangifte</param>
		/// <remarks>bivak.WerkJaar wordt genegeerd, het werkjaar wordt bepaald op basis van de startdatum van het bivak.</remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void BivakBewaren(Bivak bivak)
		{
            var wjm = new WerkJaarManager();
			var feedback = new StringBuilder();

			ChiroGroep groep;
			using (var db = new kipadminEntities())
			{
				// Bestaat de bivakaangifte al in Kipadmin?

				var aangifte = (from a in db.BivakAangifte.Include("Groep").Include("BivakOverzichtB").Include("BivakOverzichtU").Include("BivakOverzichtS")
				                where a.GapUitstapID == bivak.UitstapID
				                select a).FirstOrDefault();

				groep = (from cg in db.Groep.OfType<ChiroGroep>()
				         where System.String.Compare(cg.STAMNR, bivak.StamNummer, System.StringComparison.OrdinalIgnoreCase) == 0
				         select cg).FirstOrDefault();

                if (groep == null)
                {
                    _log.FoutLoggen(0, String.Format("Bivakaangifte: groep met stamnummer {0} niet gevonden", bivak.StamNummer));
                }
				else if (aangifte == null)
				{
					// Nieuwe maken

					aangifte = new BivakAangifte
					           	{
					           		WerkJaar = wjm.DatumNaarWerkJaar(bivak.DatumVan),
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
					Debug.Assert(System.String.Compare(groep.STAMNR.Trim(), bivak.StamNummer.Trim(), System.StringComparison.OrdinalIgnoreCase) == 0);

					// Boven 'Trim' toegevoegd, omdat de stamnummers uit GAP soms eindigen op een spatie? 
					// Vermoedelijk omdat de groepscode een CHAR(10) is, en geen VARCHAR(10)?
					// Ik kan mij niet voorstellen dat enkel hier problemen geeft...

                    // Het zou kunnen dat een groep eerst een fout jaar ingeeft voor de bivakaangifte, en dat jaartal
                    // daarna rechtzet. In dat geval moeten we hier het werkjaar overschrijven.


                    if (aangifte.WerkJaar != wjm.DatumNaarWerkJaar(bivak.DatumVan))
                    {
                        _log.BerichtLoggen(groep.GroepID, String.Format("Werkjaar wijzigen van bestaande bivakaangifte {0}", groep.STAMNR));
                    }

                    aangifte.WerkJaar = wjm.DatumNaarWerkJaar(bivak.DatumVan);
					aangifte.BivakNaam = bivak.Naam;
					aangifte.Opmerkingen = bivak.Opmerkingen;
					aangifte.DatumVan = bivak.DatumVan;
					aangifte.DatumTot = bivak.DatumTot;
					feedback.AppendLine(String.Format("Bivakaangifte geupdatet voor {0}: {1}", groep.STAMNR, bivak.Naam));
				}

				db.SaveChanges();
                Debug.Assert(aangifte != null);
				feedback.AppendLine(String.Format("BivakAangifteID: {0}", aangifte.ID));
			}
            Debug.Assert(groep != null);
			_log.BerichtLoggen(groep.GroepID, feedback.ToString());
            FixOudeBivakTabel(groep.GroepID, wjm.DatumNaarWerkJaar(bivak.DatumVan));
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
                // Bivakaangiftes van de groep in het gegeven werkjaar.

			    var alleAangiften = from ba in db.BivakAangifte.Include("kipAdres").Include("kipPersoon.kipContactInfo")
			                        where ba.WerkJaar == werkJaar && ba.Groep.GroepID == groepID && ba.kipAdres != null 
                                    select ba;

                // Ik bekijk alleen de bivakaangiften met adres, want anders kan Ingrid DB er niet aan uit, en dat
                // is te allen prijze te vermijden ;-)

                if (alleAangiften.FirstOrDefault() == null)
                {
                    // Als er niets overschiet, veranderen we ook niets in de overzichtstabel.
                    return;
                }


				BivakAangifte aangifte = null;

				// Kijk of er al een record bestaat in de oude bivaktabel.

				var overzicht =
					(from bo in
					 	db.BivakOverzicht.Include("BivakAangifteB").Include("BivakAangifteS").Include("BivakAangifteU").Include(
					 		"VerantwoordelijkeB")
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
								        werkjaar = werkJaar,
                                        datum = DateTime.Now
					            	};
					db.AddToBivakOverzicht(overzicht);

					feedback.AppendLine("Nieuw overzicht gemaakt");
				}

				// Zet oostkantons op 'N'.  Als er 'gewoon bivak' of 'afdelingsbivak' in oostkantons is,
				// wordt dat achteraf 'J'.
				overzicht.OOSTKANTO = "N";

                // Fix 'kaartje'.  Dit is een nullable string, en het kipadminstuk dat de kaartjes arrangeert flipt als
                // daar iets anders staat dan "J" of "N".

                if (String.Compare(overzicht.KAARTJE, "T", true) == 0)
                {
                    // Een bug na de wijziging van kipBivak van table naar view, zette soms 'T' (true) ipv 'J' (ja)
                    overzicht.KAARTJE = "J";
                }
                else if (String.Compare(overzicht.KAARTJE, "J", true) != 0)
                {
                    // alles wat niet 'J' is, wordt of blijft 'N'.  (Het zal vooral null of 'F' zijn dat wordt omgezet)
                    overzicht.KAARTJE = "N";
                }


                // Splits bivakaangiftes op in binnenlandse en buitenlandse
                // Aangezien we enkel de aangiften met adressen hebben opgevraagd, moeten we daar
                // alvast niet meer op checken.

				var binnenlandseAangiften = (from ag in alleAangiften
				                             where String.IsNullOrEmpty(ag.kipAdres.Land) ||
				                             	 String.Compare(ag.kipAdres.Land, Properties.Resources.Belgie, true) == 0
				                             select ag).OrderByDescending(ag => ag.ID).ToArray();

				var buitenlandseAangiften = (from ag in alleAangiften
							     where !String.IsNullOrEmpty(ag.kipAdres.Land) &&
								 String.Compare(ag.kipAdres.Land, Properties.Resources.Belgie, true) != 0
							     select ag).OrderByDescending(ag => ag.ID).ToArray();

				// Als er een buitenlandse aangifte is, dan moet die zeker bewaard worden als buitenlands bivak.

				if (buitenlandseAangiften.FirstOrDefault() != null)
				{
					aangifte = buitenlandseAangiften.First();

				    overzicht.U_BUITEN = "J";
					overzicht.BivakAangifteU = aangifte;
					overzicht.U_BEGIN = aangifte.DatumVan;
					overzicht.U_EIND = aangifte.DatumTot;
					if (!String.IsNullOrEmpty(aangifte.BivakPlaatsNaam))
					{
						overzicht.U_NAAM = aangifte.BivakPlaatsNaam;
					}

					if (aangifte.kipAdres != null)
					{
						overzicht.U_STRAAT = String.Format("{0} {1}", aangifte.kipAdres.Straat, aangifte.kipAdres.Nr);
						overzicht.U_POSTNR = aangifte.kipAdres.PostNr;
						overzicht.U_GEMEENTE = aangifte.kipAdres.Gemeente;
						overzicht.U_LAND = aangifte.kipAdres.Land;
					}

					if (aangifte.kipPersoon != null)
					{
						overzicht.U_TEL = GsmNrGet(aangifte.kipPersoon);
					}


					feedback.AppendLine(String.Format("Geregistreerd in overzicht als buitenlands bivak: ID{1} {0}", aangifte.BivakNaam, aangifte.ID));
				}
				else
				{
				    overzicht.U_BUITEN = "N";
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

					    overzicht.S_APART = "J";
						overzicht.BivakAangifteS = aangifte;
						overzicht.S_BEGIN = aangifte.DatumVan;
						overzicht.S_EIND = aangifte.DatumTot;
						if (!String.IsNullOrEmpty(aangifte.BivakPlaatsNaam))
						{
							overzicht.S_NAAM = aangifte.BivakPlaatsNaam;
						}

						if (aangifte.kipAdres != null)
						{
							overzicht.S_STRAAT = String.Format("{0} {1}", aangifte.kipAdres.Straat, aangifte.kipAdres.Nr);
							overzicht.S_POSTNR = aangifte.kipAdres.PostNr;
							overzicht.S_GEMEENTE = aangifte.kipAdres.Gemeente;
							overzicht.S_LAND = aangifte.kipAdres.Land;


                            if (String.IsNullOrEmpty(aangifte.kipAdres.Land) || String.Compare(aangifte.kipAdres.Land, Properties.Resources.Belgie, true) == 0)
							{
								if (PostNrInOostKantons(aangifte.kipAdres.PostNr))
								{
									overzicht.OOSTKANTO = "J";
								}
								overzicht.S_PROVINCI = PostNrNaarProvincie(aangifte.kipAdres.PostNr);
							}

						}

						if (aangifte.kipPersoon != null)
						{
							overzicht.S_TEL = GsmNrGet(aangifte.kipPersoon);
						}

						feedback.AppendLine(String.Format("Geregistreerd in overzicht als 'afdelingsbivak': ID{1} {0}", aangifte.BivakNaam, aangifte.ID));

						// De op 1 na recentste wordt dan het 'gewone' bivak.

						aangifte = binnenlandseAangiften[1];
					}
					else
					{
						// Als er maar 1 niet-buitenlandse aangifte is, dan wordt dat de 'gewone'
						// bivakaangifte.

					    overzicht.S_APART = "N";

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
						overzicht.B_STRAAT = String.Format("{0} {1}", aangifte.kipAdres.Straat, aangifte.kipAdres.Nr);
						overzicht.B_POSTNR = aangifte.kipAdres.PostNr;
						overzicht.B_GEMEENTE = aangifte.kipAdres.Gemeente;
						overzicht.B_LAND = aangifte.kipAdres.Land;

						if (String.IsNullOrEmpty(overzicht.B_LAND) || String.Compare(overzicht.B_LAND, Properties.Resources.Belgie, true) == 0)
						{
							overzicht.B_PROVINCI = PostNrNaarProvincie(aangifte.kipAdres.PostNr);
							if (PostNrInOostKantons(aangifte.kipAdres.PostNr))
							{
								overzicht.OOSTKANTO = "J";
							}
						}
					}

					if (!String.IsNullOrEmpty(aangifte.BivakPlaatsNaam))
					{
						overzicht.B_NAAM = aangifte.BivakPlaatsNaam;
					}

					if (aangifte.kipPersoon != null)
					{
						overzicht.B_TEL = GsmNrGet(aangifte.kipPersoon);
						overzicht.VerantwoordelijkeB = aangifte.kipPersoon;
					}

					if (overzicht.VerantwoordelijkeB == null)
					{
						// In kipadmin wordt dezelfde tabel gebruik voor aanvraag kampeermateriaal (tenten) en 
						// bivakaangifte, en het verschil zit 'em in de bivakverantwoordelijke.  Als die er is,
						// is het een bivakaangifte.  (O nee!)
						//
						// Dus als er geen verantwoordelijke is, dan hangen we daar voor het gemak de contactpersoon aan.

						var contact = (from l in db.Lid
						               where l.Groep.GroepID == groepID &&
						                     l.werkjaar == werkJaar &&
						                     l.HeeftFunctie.Any(hf => hf.Functie.id == (int) FunctieEnum.ContactPersoon)
						               select l.Persoon).FirstOrDefault();
						overzicht.VerantwoordelijkeB = contact;

						if (contact == null)
						{
							feedback.AppendLine("Geen verantwoordelijke, noch contactpersoon voor groep.  Jammer voor hen.");
						}
						else
						{
							feedback.AppendLine(String.Format(
								"Er was nog geen verantwoordelijke; we nemen de contactpersoon {0} {1}",
								contact.VoorNaam,
								contact.Naam));
						}
					}

					feedback.AppendLine(String.Format("Geregistreerd in overzicht als 'gewoon' bivak:  ID{1} {0}", aangifte.BivakNaam, aangifte.ID));
				}
				overzicht.STEMPEL = DateTime.Now;
				db.SaveChanges();
				feedback.AppendLine(String.Format("OverzichtsID: {0}", overzicht.id));
			}
			_log.BerichtLoggen(groepID, feedback.ToString());
		}

		/// <summary>
		/// Geeft <c>true</c> als het gegeven postnummer in de oostkantons ligt
		/// anders <c>false</c>.
		/// </summary>
		/// <param name="postNr">Belgisch postnummer</param>
		/// <returns><c>true</c> als het gegeven postnummer in de oostkantons ligt
		/// anders <c>false</c>.</returns>
		/// <remarks>Als het postnummer niet numeriek is, wort <c>null</c> opgeleverd.</remarks>
		private static bool PostNrInOostKantons(string postNr)
		{
			int nr;

			if (int.TryParse(postNr, out nr))
			{
				return (nr >= 4700 && nr < 4800 || nr >= 4950 && nr < 4970);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Probeer een GSM-nummer van de persoon vast te krijgen
		/// </summary>
		/// <param name="kipPersoon">Persoon uit kipadmin, met contactinfo gekoppeld</param>
		/// <returns>Een GSM-nummer, of anders een gewoon telefoonnummer</returns>
		private static string GsmNrGet(Persoon kipPersoon)
		{
			var nummers = (from nr in kipPersoon.kipContactInfo
			               where nr.ContactTypeId == 1
			               select nr).OrderByDescending(nr => nr.GeenMailings).ThenBy(nr => nr.VolgNr).Select(nr => nr.Info).
				ToArray();
			string resultaat = String.Empty;

			if (nummers.FirstOrDefault() == null)
			{
				return String.Empty;
			}
			else
			{
				resultaat = (from nr in nummers
				             where IsGsmNr(nr)
				             select nr).FirstOrDefault();
				if (resultaat != null)
				{
					return resultaat;
				}
				else
				{
					// Als er geen gsm-nummers gevonden: gewoon eerste telefoonnummer
					return nummers.First();
				}
			}
		}

		/// <summary>
		/// Kort-door-de-bochte method die gokt of een telefoonnummer al dan niet een gsm-nummer is
		/// </summary>
		/// <param name="nr">Te testen telefoonnummer</param>
		/// <returns><c>true</c> als het telefoonnummer een gsm-nummer is</returns>
		private static bool IsGsmNr(string nr)
		{
			// Dit is slechts een zeer ruwe benadering.
			return (nr.StartsWith("04") || nr.StartsWith("+324"));
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
			bool gewijzigd = false;
			BivakAangifte bivak;

			using (var db = new kipadminEntities())
			{
				bivak = (from b in db.BivakAangifte.Include("kipAdres").Include("Groep")
				             where b.GapUitstapID == uitstapID
				             select b).FirstOrDefault();

				if (bivak == null)
				{
					_log.FoutLoggen(0, String.Format(
						"Kan bivakplaats {0} niet toekennen aan onbestaand bivak (uitstapID {1}).",
						plaatsNaam,
						uitstapID
						));
					return;
				}

				var nieuwAdres = AdresVindenOfMaken(adres, db);

				if (nieuwAdres.ID == 0 || bivak.kipAdres == null || bivak.kipAdres.ID != nieuwAdres.ID)
				{
					// Enkel actie als er echt een nieuw adres toegekend moet worden.

					bivak.kipAdres = nieuwAdres;
					bivak.BivakPlaatsNaam = plaatsNaam;
					db.SaveChanges();
					gewijzigd = true;

					_log.BerichtLoggen(0, String.Format(
						"Bivakplaats {0} ({2}) toegekend aan bivak met uitstapID {1}.",
						plaatsNaam,
						uitstapID,
						nieuwAdres.Gemeente));
				}
			}
			if (gewijzigd)
			{
				FixOudeBivakTabel(bivak.Groep.GroepID, bivak.WerkJaar);
			}
		}

		/// <summary>
		/// Stelt de persoon met gegeven <paramref name="adNummer"/> in als contactpersoon voor
		/// het bivak met gegeven <paramref name="uitstapID"/>
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) voor het bivak</param>
		/// <param name="adNummer">AD-nummer contactpersoon bivak</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void BivakContactBewaren(int uitstapID, int adNummer)
		{
			bool gewijzigd = false;
			BivakAangifte bivak;

			using (var db = new kipadminEntities())
			{
				bivak = (from b in db.BivakAangifte.Include("kipPersoon").Include("Groep")
				         where b.GapUitstapID == uitstapID
				         select b).FirstOrDefault();

				if (bivak == null)
				{
					_log.FoutLoggen(0, String.Format(
						"Kan contactpersoon {0} niet toekennen aan onbestaand bivak {1}.",
						adNummer,
						uitstapID));
					return;
				}

				if (bivak.kipPersoon == null || bivak.kipPersoon.AdNummer != adNummer)
				{
					// We doen alleen iets als er nog geen contactpersoon was, of als
					// de contactpersoon iemand anders was.

					
					var persoon = (from p in db.PersoonSet
					               where p.AdNummer == adNummer
					               select p).FirstOrDefault();

					if (persoon == null)
					{
						_log.FoutLoggen(bivak.Groep.GroepID, String.Format("Bivakcontact met ongeldig ad-nummer: {0}", adNummer));
						return;
					}
					bivak.kipPersoon = persoon;
					db.SaveChanges();
					gewijzigd = true;

					_log.BerichtLoggen(bivak.Groep.GroepID, String.Format(
						"Persoon {0} {1} {2} ingesteld als contact voor bivak {3} {4}",
						persoon.AdNummer,
						persoon.VoorNaam,
						persoon.Naam,
						bivak.ID,
						bivak.BivakNaam));

				}
				
			}
			if (gewijzigd)
			{
				FixOudeBivakTabel(bivak.Groep.GroepID, bivak.WerkJaar);
			}
		}

		/// <summary>
		/// Stelt de persoon met gegeven <paramref name="details"/> in als contactpersoon voor
		/// het bivak met gegeven <paramref name="uitstapID"/>
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) voor het bivak</param>
		/// <param name="details">gegevens van de persoon</param>
		/// <remarks>Deze method mag enkel gebruikt worden als het ad-nummer van de
		/// persoon onbestaand of onbekend is.</remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void BivakContactBewarenAdOnbekend(int uitstapID, PersoonDetails details)
		{
			Debug.Assert(details.Persoon.AdNummer == null);

			if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
			{
				_log.FoutLoggen(0, String.Format(
					"Persoon zonder voornaam niet geupdatet: {0}",
					details.Persoon.Naam));
				return;
			}

			int adnr = UpdatenOfMaken(details);
			BivakContactBewaren(uitstapID, adnr);
		}

		/// <summary>
		/// Verwijdert een bivak uit kipadmin.
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) van het te verwijderen bivak</param>
		public void BivakVerwijderen(int uitstapID)
		{
			int groepID, werkJaar, bivakID;

			using (var db = new kipadminEntities())
			{
				var bivak = (from b in db.BivakAangifte.Include("Groep")
					 where b.GapUitstapID == uitstapID
					 select b).FirstOrDefault();

				if (bivak == null)
				{
					_log.FoutLoggen(0, String.Format(
						"Kan onbestaand bivak met uitstapID {0} niet verwijderen.",
						uitstapID));
					return;
				}
			    // Bewaar groepID, want ik denk dat we daar niet meer aankunnen
			    // als bivak wordt verwijderd.

			    groepID = bivak.Groep.GroepID;
			    werkJaar = bivak.WerkJaar;
			    bivakID = bivak.ID;

			    db.DeleteObject(bivak);
			}

			FixOudeBivakTabel(groepID, werkJaar);

			_log.BerichtLoggen(groepID, String.Format(
				"Bivak {0} verwijderd",
				bivakID));
		}
	}
}
