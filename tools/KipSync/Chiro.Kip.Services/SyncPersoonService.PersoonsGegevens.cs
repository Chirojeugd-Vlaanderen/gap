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
using AutoMapper;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.UpdateSvc.Contracts;
using Chiro.Kip.Data;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Kip.Workers;
using Adres = Chiro.Kip.ServiceContracts.DataContracts.Adres;
using KipPersoon = Chiro.Kip.Data.Persoon;
using Persoon = Chiro.Kip.ServiceContracts.DataContracts.Persoon;

namespace Chiro.Kip.Services
{
	public partial class SyncPersoonService
	{
		/// <summary>
		/// Probeert een persoon te vinden op basis van persoonsgegevens, adressen en communicatie.
		/// Als dat lukt, worden de meegegeven persoonsgegevens, adressen, communicatie overgenomen in de
		/// database.  Als dat niet lukt, wordt een nieuwe persoon aangemaakt.
		/// </summary>
		/// <param name="details">Details van de te vinden persoon</param>
		/// <returns>AD-nummer van gevonden/aangemaakte persoon.</returns>
		private int UpdatenOfMaken(PersoonDetails details)
		{
			var persoon = details.Persoon;
			var adres = details.Adres;
			var adresType = details.AdresType;
			var communicatieMiddelen = details.Communicatie;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<Persoon, KipPersoon>()
			    .ForMember(dst => dst.AdNummer, opt => opt.Ignore())
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			Chiro.Kip.Data.Persoon gevonden;

			// Doe eigenlijk hetzelfde als bij PersoonUpdaten, maar in dit geval hebben we meer info
			// om bestaande personen op te zoeken.

			using (var db = new kipadminEntities())
			{
				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(persoon);
				if (communicatieMiddelen != null)
				{
					zoekInfo.Communicatie = (from cm in communicatieMiddelen select cm.Waarde).ToArray();
				}

				if (adres != null)
				{
					zoekInfo.PostNr = KipPostNr(adres);
				}

				// Zoek of maak gevraagde persoon
				gevonden = mgr.Zoeken(zoekInfo, true, db);

				// Neem nieuwe gegevens over
				Mapper.Map(persoon, gevonden);
				db.SaveChanges();
			}
			// Als er geen AD-nummer was, dan heeft de SaveChanges er voor ons eentje gemaakt.
			Debug.Assert(gevonden.AdNummer > 0);

			string feedback = String.Format(
			    "Persoon herkend/aangemaakt: ID{0} {1} {2} AD{3}",
			    persoon.ID,
			    persoon.VoorNaam,
			    persoon.Naam,
			    gevonden.AdNummer);

			// AD-nummer overnemen in persoon en GAP
			persoon.AdNummer = gevonden.AdNummer;

            ServiceHelper.CallService<IUpdateService>(svc => svc.AdNummerToekennen(persoon.ID, gevonden.AdNummer));

			if (adres != null)
			{
				StandaardAdresBewaren(
				    adres,
				    new Bewoner[] { new Bewoner { Persoon = persoon, AdresType = adresType } });
			}

			AlleCommunicatieBewaren(persoon, communicatieMiddelen);
			_log.BerichtLoggen(0, feedback);

			return gevonden.AdNummer;
		}

		/// <summary>
		/// Updatet een persoon in Kipadmin op basis van de gegevens in GAP.  Als er geen AD-nummer is, dan doen we
		/// een schamele poging om de persoon al te vinden.   Als ook dat niet lukt, maken we een nieuwe persoon aan.
		/// Bij ontbrekend AD-nummer, en wordt het achteraf ingevuld bij <paramref name="persoon"/>.  (Niet interessant
		/// voor service, maar wel voor andere methods die deze aanroepen).
		/// </summary>
		/// <param name="persoon">Informatie over een geupdatete persoon in GAP</param>
		/// <remarks>Als AD-nummer ontbreekt, wordt er sowieso een nieuwe persoon gemaakt.</remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PersoonUpdaten(Persoon persoon)
		{
			if (String.IsNullOrEmpty(persoon.VoorNaam))
			{
				_log.FoutLoggen(0, String.Format(
					"Persoon zonder voornaam niet geupdatet: {0}",
					persoon.Naam));
				return;
			}

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<Persoon, KipPersoon>()
			    .ForMember(dst => dst.AdNummer, opt => opt.Ignore())
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				KipPersoon kipPersoon;

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(persoon);
				kipPersoon = mgr.Zoeken(zoekInfo, true, db);
				Mapper.Map(persoon, kipPersoon);
				kipPersoon.Stempel = DateTime.Now;
				db.SaveChanges();
				if (persoon.AdNummer != kipPersoon.AdNummer)
				{
					persoon.AdNummer = kipPersoon.AdNummer;
					ServiceHelper.CallService<IUpdateService>(svc => svc.AdNummerToekennen(persoon.ID, kipPersoon.AdNummer));
				}
			}

			_log.BerichtLoggen(0, String.Format(
				"Persoon geupdatet: ID{0} {1} {2} AD{3}",
				persoon.ID,
				persoon.VoorNaam,
				persoon.Naam,
				persoon.AdNummer));
		}

		/// <summary>
		/// Aan te roepen als een voorkeursadres gewijzigd moet worden.  Deze method vervangt in kipadmin
		/// gewoon het adres met volgnummer 1.  De andere adressen blijven onaangeroerd.
		/// </summary>
		/// <param name="adres">Nieuw voorkeursadres</param>
		/// <param name="bewoners">AD-nummers en adrestypes voor personen de dat adres moeten krijgen</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
		{
			var feedback = new StringBuilder();

			var pMgr = new PersonenManager();

			Debug.Assert(adres != null); // We gaan niet belachelijk doen he

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
				.ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
				.ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			// Kipadmin kent het onderscheid postnummer-postcode niet.  Als er een postcode is, wordt die
			// met een spatie aan de postnummer geplakt.

			string postNr = KipPostNr(adres);

			using (var db = new kipadminEntities())
			{
				var personen = new List<KipPersoon>();

				// Zoek alle 'bewoners' op.  De gevonden bewoners worden bewaard
				// in 'personen'.

				foreach (var b in bewoners)
				{
					// NOOT: Adressen van bewoners zonder AD-nummer mogen wel bewaard worden.  Het
					// kan namelijk zijn dat een persoon al wel aangesloten is, maar dat
					// zijn AD-nummer nog niet teruggesynct is.

					if (b.Persoon == null)
					{
						_log.FoutLoggen(0, String.Format(Properties.Resources.AdresZonderPersoon, adres.Straat, adres.HuisNr, adres.Bus, adres.PostNr, adres.WoonPlaats));
					}
					else if (String.IsNullOrEmpty(b.Persoon.Naam) || String.IsNullOrEmpty(b.Persoon.VoorNaam))
					{
						_log.FoutLoggen(0, String.Format(Properties.Resources.NegeerPersoonOnvolledigeNaam, b.Persoon.VoorNaam, b.Persoon.Naam, b.Persoon.AdNummer));
					}
					else
					{
						var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(b.Persoon);
						zoekInfo.PostNr = postNr;

						var gevonden = pMgr.Zoeken(zoekInfo, false, db);

						if (gevonden == null)
						{
							// Er worden soms wel eens huisgenoten meegegeven die niet in kipadmin zitten.
							// Als we ze tegen komen, negeren we het adres.

							_log.BerichtLoggen(0, String.Format(
								"Adreswijziging: {0} {1} niet gevonden",
								b.Persoon.VoorNaam, b.Persoon.Naam));
						}
						else
						{
							personen.Add(gevonden);

							// In bewoners ad-nummer aanpassen, zoda we straks het juiste adrestype kunnen vinden
							b.Persoon.AdNummer = gevonden.AdNummer;
						}
					}
				}


				// Vind of maak adres

				// Als dat entities op sql server zijn, dan gebeurt het zoeken sowieso 
				// hoofdletterongevoelig.

				Data.Adres adresInDb = AdresVindenOfMaken(adres, db);


				// We zitten met het gedoe dat in Kipadmin de adressen een volgnummer hebben.  De voorkeurs-
				// adressen moeten bewaard worden met volgnummer 1.
				//
				// Strategie:
				//	verwijder koppelingen met to-adres waar volgnummer verschilt van 1
				//	vind of maak 'woont'-object met volgnummer 1
				//	koppel 'woont'-object met volgnummer 1 aan to-adres 


				var teVerwijderen =
					personen.SelectMany(prs => prs.kipWoont).Where(kw => kw.kipAdres.ID == adresInDb.ID && kw.VolgNr != 1);

				if (teVerwijderen.Count() > 0)
				{
					foreach (var wnt in teVerwijderen.ToArray())
					{
						db.DeleteObject(wnt);
					}

					// Als het nieuwe adres al een adres was met een ander volgnummer,
					// moet dat hier toch al verwijderd worden, om te vermijden dat een persoon
					// bij het saven tussentijds 2 keer op hetzelfde adres woont
					// (-> unique index violation)

					db.SaveChanges();
				}



				// TODO: Het adrestype bepalen is iedere keer een linq-expressie, en dus iedere
				// keer een loop.  Kan dat niet efficienter?

				foreach (var p in personen)
				{
					var eersteWnt = (from wnt in p.kipWoont
							 where wnt.VolgNr == 1
							 select wnt).FirstOrDefault();

					int adresTypeID = (int)(from b in bewoners
								where b.Persoon != null && b.Persoon.AdNummer == p.AdNummer
								select b.AdresType).FirstOrDefault();

					var kAdrType = (from at in db.AdresTypeSet
							where at.ID == adresTypeID
							select at).FirstOrDefault();

					if (eersteWnt == null)
					{
						eersteWnt = new Woont
						{
							kipPersoon = p,
							kipAdres = adresInDb,
							Geldig = true,
							VolgNr = 1,
							kipAdresType = kAdrType
						};
						db.AddToWoontSet(eersteWnt);
					}
					else
					{
						eersteWnt.kipAdres = adresInDb;
						eersteWnt.kipAdresType = kAdrType;
						eersteWnt.Geldig = true;
					}


					feedback.AppendLine(String.Format("Update voorkeuradres: AD{0}", p.AdNummer));
				}

				// fingers crossed:

				db.SaveChanges();
			}
			_log.BerichtLoggen(0, feedback.ToString());
		}

		/// <summary>
		/// Zoekt het gegeven <paramref name="adres"/> op. Als het niet bestaat, maak het
		/// dan aan.  (SaveChanges wordt nog niet aangeroepen.)
		/// </summary>
		/// <param name="adres">Adresgegevens op basis waarvan het adres moet worden gezocht.</param>
		/// <param name="db">Datacontext waarin gezocht/aangemaakt moet worden</param>
		/// <returns>Het gevonden of gemaakte adres</returns>
		private Data.Adres AdresVindenOfMaken(Adres adres, kipadminEntities db)
		{
			string postNr = KipPostNr(adres);
			string huisNr = null;

			if (adres.HuisNr != null)
			{
				if (String.IsNullOrEmpty(adres.Bus))
				{
					huisNr = adres.HuisNr.ToString();
				}
				else if (adres.Bus[0] >= '0' && adres.Bus[0] <= '9')
				{
					// Als de bus numeriek is, dan zetten we 'bus' tussen
					// huisnummer en bus

					huisNr = String.Format("{0} bus {1}", adres.HuisNr, adres.Bus);
				}
				else
				{
					// zo niet: een spatie

					huisNr = String.Format("{0} {1}", adres.HuisNr, adres.Bus);
				}
			}


			var adresInDb = (from adr in db.AdresSet.Include("kipWoont.kipPersoon").Include("kipWoont.kipAdresType")
			                 where adr.Straat == adres.Straat
			                       && adr.Nr == huisNr
			                       && adr.PostNr == postNr
			                       && adr.Gemeente == adres.WoonPlaats
			                       && (string.IsNullOrEmpty(adr.Land) && string.IsNullOrEmpty(adres.Land)
			                           || adr.Land == adres.Land)
			                 select adr).FirstOrDefault();

			if (adresInDb == null)
			{
				adresInDb = new Chiro.Kip.Data.Adres
				            	{
				            		ID = 0,
				            		Straat = adres.Straat,
				            		Nr = huisNr,
				            		PostNr = postNr,
				            		Gemeente = adres.WoonPlaats,
				            		Land = adres.Land
				            	};
				db.AddToAdresSet(adresInDb);

				_log.BerichtLoggen(0, String.Format(
					"Nieuw adres gemaakt ({0}): {1} {2}, {3} {4} * {5}",
					adresInDb.ID, adresInDb.Straat, adresInDb.Nr, adresInDb.PostNr, adresInDb.Gemeente, adresInDb.Land));

			}
			else
			{
				_log.BerichtLoggen(0, String.Format(
					"Bestaand adres gevonden ({0}): {1} {2}, {3} {4} * {5}",
					adresInDb.ID, adresInDb.Straat, adresInDb.Nr, adresInDb.PostNr, adresInDb.Gemeente, adresInDb.Land));
			}
			return adresInDb;
		}

        /// <summary>
        /// Zoekt in de communicatievormen van de persoon bepaald door <paramref name="persoonsGegevens"/> een 
        /// communicatiemiddel op van hetzelfde type als <paramref name="communicatieMiddel"/>, maar met nummer 
        /// <paramref name="nummerBijTeWerken"/>. Die communicatievorm wordt vervangen door de gegevens in 
        /// <paramref name="communicatieMiddel"/>.
        /// </summary>
        /// <param name="persoonsGegevens">bepaald persoon met aan te passen communicatievorm</param>
        /// <param name="nummerBijTeWerken">nummer van te vervangen communicatievorm</param>
        /// <param name="communicatieMiddel">informatie nieuwe communicatievorm</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
	    public void CommunicatieBijwerken(Persoon persoonsGegevens, string nummerBijTeWerken, CommunicatieMiddel communicatieMiddel)
        {
            string feedback = String.Empty;

            Mapper.CreateMap<Persoon, PersoonZoekInfo>()
                  .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int) src.Geslacht))
                  .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

            using (var db = new kipadminEntities())
            {
                var mgr = new PersonenManager();
                var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(persoonsGegevens);
                var persoon = mgr.Zoeken(zoekInfo, false, db);

                if (persoon == null)
                {
                    _log.FoutLoggen(0, String.Format(Properties.Resources.CommAanpassenOnbekendePersoon,
                                                     persoonsGegevens.VoorNaam, persoonsGegevens.Naam, nummerBijTeWerken));
                    return;
                }

                int communicatieTypID = (int)communicatieMiddel.Type;

                var aanTePassen = (from ci in db.ContactInfoSet
                                   where ci.kipPersoon.AdNummer == persoon.AdNummer
                                         && ci.ContactTypeId == communicatieTypID
                                         && ci.Info == nummerBijTeWerken
                                   select ci).FirstOrDefault();

                if (aanTePassen == null)
                {
                    feedback = String.Format(Properties.Resources.TeWijzigenCommunicatieNietGevonden,
                                             persoonsGegevens.VoorNaam, persoonsGegevens.Naam, nummerBijTeWerken,
                                             communicatieMiddel.Waarde);
                    mgr.CommunicatieToevoegen(communicatieMiddel, persoon, db);
                }
                else
                {
                    feedback = String.Format(Properties.Resources.CommVormGewijzigd, persoonsGegevens.VoorNaam,
                                             persoonsGegevens.Naam, nummerBijTeWerken, communicatieMiddel.Waarde);
                    aanTePassen.GeenMailings = communicatieMiddel.GeenMailings;
                    aanTePassen.Info = communicatieMiddel.Waarde;
                }

                db.SaveChanges();
                _log.BerichtLoggen(0, feedback);
            }
        }

	    /// <summary>
		/// Verwijdert alle bestaande contactinfo, en vervangt door de contactinfo meegegeven in 
		/// <paramref name="communicatieMiddelen"/>.
		/// </summary>
		/// <param name="pers">persoon waarvoor contactinfo te updaten</param>
		/// <param name="communicatieMiddelen">te updaten contactinfo</param>
		/// <remarks>Van de persoon zelf blijven we af!</remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AlleCommunicatieBewaren(
			Persoon pers,
			IEnumerable<CommunicatieMiddel> communicatieMiddelen)
		{
			string feedback;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				KipPersoon persoon;

				// Haal eerst persoon op met alle communicatie-info gekend in Kipadmin.

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);
				zoekInfo.Communicatie = (from cm in communicatieMiddelen select cm.Waarde).ToArray();
				persoon = mgr.Zoeken(zoekInfo, false, db);

				if (persoon == null)
				{
					_log.FoutLoggen(0, String.Format(
						Properties.Resources.CommunicatieOnbekendePersoon,
						zoekInfo.VoorNaam,
						zoekInfo.Naam,
						String.Concat(
							(from cm in communicatieMiddelen select cm.Waarde + ';').ToArray())));

					//throw new InvalidOperationException(String.Format(
					//        Properties.Resources.PersoonNietGevonden,
					//        pers.VoorNaam,
					//        pers.Naam));
					return;
				}
				// Hieronder quick and dirty gepruts, 
				// o.a. omdat de communicatievormen in kipadmin genummerd moeten zijn
				// TODO: Toch wat properder proberen

				// Verwijder gewoon alle bestaande communicatie

				var teVerwijderen = (from cv in persoon.kipContactInfo
						     select cv).ToArray();

				foreach (var cv in teVerwijderen)
				{
					db.DeleteObject(cv);
				}

				// Bewaar tussentijds om key violations te vermijden
				db.SaveChanges();

				// Voeg nu de meegeleverde communicatie opnieuw toe.

				var nieuweComm = (from cm in communicatieMiddelen.Distinct()
						  select new ContactInfo
						  {
							  ContactInfoId = 0,
							  ContactTypeId = (int)cm.Type,
							  GeenMailings = cm.GeenMailings,
							  Info = cm.Waarde,
							  kipPersoon = persoon
						  }).OrderBy(nc => nc.ContactTypeId).ToList();

				// Nummeren per type.

				int teller = 0;
				int vorigType = -1;

				foreach (var comm in nieuweComm)
				{
					if (comm.ContactTypeId != vorigType)
					{
						teller = 0;
						vorigType = comm.ContactTypeId;
					}
					comm.VolgNr = ++teller;
					db.AddToContactInfoSet(comm);
				}

				db.SaveChanges();
				feedback = String.Format("Communicatie bewaard voor ID{0} {1} {2} AD{3}", pers.ID, persoon.VoorNaam, persoon.Naam,
							 persoon.AdNummer);
			}
			_log.BerichtLoggen(0, feedback);
		}

		/// <summary>
		/// Voegt communicatevorm <paramref name="communicatie"/> toe aan de communicatievormen van <paramref name="pers"/>,
		/// op voorwaarde dat die er nog niet was.
		/// </summary>
		/// <param name="pers">Persoon die nieuwe communicatievorm moet krijgen</param>
		/// <param name="communicatie">Toe te voegen communicatievorm</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CommunicatieToevoegen(Persoon pers, CommunicatieMiddel communicatie)
		{
			string feedback = String.Empty;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				KipPersoon persoon;

				// Haal eerst persoon op met alle communicatie-info gekend in Kipadmin.

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);
				zoekInfo.Communicatie = new string[] { communicatie.Waarde };
				persoon = mgr.Zoeken(zoekInfo, false, db);

				if (persoon == null)
				{
					//throw new InvalidOperationException(String.Format(
					//        Properties.Resources.PersoonNietGevonden,
					//        pers.VoorNaam,
					//        pers.Naam));
					_log.FoutLoggen(0,
						String.Format(Properties.Resources.CommunicatieOnbekendePersoon,
						pers.VoorNaam,
						pers.Naam,
						communicatie.Waarde));
					return;
				}
				
				if (mgr.CommunicatieToevoegen(communicatie, persoon, db))
				{
				    feedback = String.Format(
				        "Communicate toegevoegd voor ID{0} {1} {2} AD{3}: {4}",
				        persoon.GapID, persoon.VoorNaam, persoon.Naam, persoon.AdNummer, communicatie.Waarde);
				};
			}
			_log.BerichtLoggen(0, feedback);

		}

	    /// <summary>
		/// Verwijdert een communicatiemiddel uit Kipadmin.
		/// </summary>
		/// <param name="pers">Persoonsgegevens van de persoon waarvan het communicatiemiddel moet verdwijnen.</param>
		/// <param name="communicatie">Gegevens over het te verwijderen communicatiemiddel</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CommunicatieVerwijderen(Persoon pers, CommunicatieMiddel communicatie)
		{
			string feedback = String.Empty;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);
				var persoon = mgr.Zoeken(zoekInfo, false, db);

				if (persoon == null)
				{
					_log.FoutLoggen(0, String.Format(
						Properties.Resources.CommVerwijderenOnbekendePersoon,
						pers.VoorNaam,
						pers.Naam,
						communicatie.Waarde));
					return;
				}

				// We gaan ervan uit dat er geen dubbele communicatievormen in de database
				// zitten; we verwijderen enkel de FirstOrDefault.

				int communicatieTypeID = (int)communicatie.Type;

				var teVerwijderen = (from ci in db.ContactInfoSet
						     where ci.kipPersoon.AdNummer == persoon.AdNummer
									   && ci.ContactTypeId == communicatieTypeID
									   && ci.Info == communicatie.Waarde
						     select ci).FirstOrDefault();

				if (teVerwijderen != null)
				{
					feedback = String.Format("Verwijderen communicatie: AD{0} {1}", persoon.AdNummer, communicatie.Waarde);
					db.DeleteObject(teVerwijderen);
					db.SaveChanges();
				}
			}

			_log.BerichtLoggen(0, feedback);
		}
	}
}
