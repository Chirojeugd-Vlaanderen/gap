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

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Chiro.Kip.Data;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Kip.Workers.Properties;
using Persoon = Chiro.Kip.Data.Persoon;

namespace Chiro.Kip.Workers
{
	/// <summary>
	/// Wat 'businesslogica' aan kipadminkant mbt personen, en stiekem ook leden.  Maar helemaal niet zo proper als in GAP; 
	/// de objectcontext (data layer) is hier alomtegenwoordig.
	/// </summary>
	public class PersonenManager
	{
		/// <summary>
		/// Zoekt een persoon, in verschillende stappen:
		/// (1) Als gegeven AD-nummer bestaat, er een persoon met dat AD-nummer bestaat, dan wordt die opgeleverd
		/// (2) Aks (1) failt, zoek op GapID.  Indien gevonden, lever op
		/// (3) Als nog niet gevonden, zoek dan naar een persoon met dezelfde naam/geslacht en min 1 overeenkomstige commvorm
		/// (4) Nog steeds niets gevonden? Zoek persoon met zelfde naam,geslacht en geboortedatum, uit gegeven postnummer.
		/// (5) Nog niet gevonden? Zoek enkel op naam, geslacht en geboortedatum
		/// (6) Als nog steeds niet gevonden, en <paramref name="maken"/> is gezet, wordt de persoon aangemaakt
		/// </summary>
		/// <param name="zoekInfo">Bevat de informatie nodig voor bovenvermelde procedure</param>
		/// <param name="maken">Indien <c>true</c>, wordt de persoon gemaakt als hij niet gevonden wordt.</param>
		/// <param name="db">De objectcontext voor entity framework</param>
		/// <returns>De gevonden persoon, met adres en communicatie..  
		/// Indien niemand gevonden: <c>null</c> of de aangemaakte persoon, alnaargelang <paramref name="maken"/>.</returns>
		/// <remarks>Als de gevraagde persoon aangemaakt wordt, dan wordt die nog niet bewaard! Roep achteraf zelf SaveChanges aan!
		/// Er mag gerust een deel van de info in <paramref name="zoekInfo"/> ontbreken.
		/// 
		/// TODO: 
		/// </remarks>
		public Persoon Zoeken(PersoonZoekInfo zoekInfo, bool maken, kipadminEntities db)
		{
			Persoon resultaat;

			// poging 1
			if (zoekInfo.AdNummer != null)
			{
				resultaat = (from p in db.PersoonSet.Include("kipContactInfo").Include("kipWoont.KipAdres")
					     where p.AdNummer == zoekInfo.AdNummer
					     select p).FirstOrDefault();
				if (resultaat != null)
				{
				    return resultaat;
				}
			}

			// poging 2
			if (zoekInfo.GapID != null)
			{
                resultaat = (from p in db.PersoonSet.Include("kipContactInfo").Include("kipWoont.KipAdres")
					     where p.GapID == zoekInfo.GapID select p).FirstOrDefault();
				if (resultaat != null) return resultaat;
			}

			// Niemand gevonden op basis van ID's.  Dan moet er een naam en voornaam zijn, of we spelen niet meer mee.

			if (String.IsNullOrEmpty(zoekInfo.Naam) || String.IsNullOrEmpty(zoekInfo.VoorNaam))
			{
				throw new InvalidOperationException(Resources.OnvoldoendePersoonsInfo);
			}

			// Zoeken bij naamgenoten.

			var naamgenoten = (from p in db.PersoonSet
                        .Include("kipContactInfo").Include("kipWoont.KipAdres")
					   where p.Naam == zoekInfo.Naam && p.VoorNaam == zoekInfo.VoorNaam
						 && p.Geslacht == zoekInfo.Geslacht
					   select p).ToArray();  // ToArray lijkt nodig te zijn om communicatie te behouden.

			// Poging 3
			if (zoekInfo.Communicatie != null)
			{
                // zoek in de communicatiemiddelen van alle naamgenoten naar
                // overeenkomstige communicatiemiddelen uit zoekInfo.Communicatie.
                // De eerste persoon die gekoppeld is aan zo'n gevonden
                // communicatiemiddel, kan dienen als resultaat.

                // TODO: Dit is niet fatsoenlijk getest!

			    resultaat = (from ci in naamgenoten.SelectMany(ng => ng.kipContactInfo)
			                 where zoekInfo.Communicatie.Contains(ci.Info)
			                 select ci.kipPersoon).FirstOrDefault();

				if (resultaat != null) return resultaat;
			}

			// Poging 4
			if (!(zoekInfo.Communicatie == null || zoekInfo.PostNr == null))
			{
				string postNummerString = zoekInfo.PostNr.ToString();

				resultaat = (from ng in naamgenoten
					  where ng.GeboorteDatum == zoekInfo.GeboorteDatum
						&& ng.kipWoont.Any(wnt => wnt.kipAdres.PostNr == postNummerString)
					  select ng).FirstOrDefault();
				if (resultaat != null) return resultaat;
			}

			// Poging 5
			resultaat = (from ng in naamgenoten
			             where ng.GeboorteDatum == zoekInfo.GeboorteDatum
			             select ng).FirstOrDefault();

			if (resultaat != null) return resultaat;

			if (!maken) return null;

			// Nog steeds niet gevonden? Maak nieuw.
			Mapper.CreateMap<PersoonZoekInfo, Persoon>().ForMember(dst => dst.AdNummer, opt => opt.Ignore());

			resultaat = new Persoon();
			Mapper.Map(zoekInfo, resultaat);
			resultaat.BurgerlijkeStandId = Settings.Default.StandaardBurgerlijkeStaat;
			resultaat.Stempel = DateTime.Now;
			db.AddToPersoonSet(resultaat);

			return resultaat;
		}

		/// <summary>
		/// Zoekt een lid op in de database, aan de hand van <paramref name="zoekInfo"/>, <paramref name="stamNummer"/>
		/// en <paramref name="werkJaar"/>
		/// </summary>
		/// <param name="zoekInfo">informatie om persoon te vinden</param>
		/// <param name="stamNummer">stamnummer van groep waarin lid</param>
		/// <param name="werkJaar">werkjaar waarin lid</param>
		/// <param name="db">objectcontext</param>
		/// <returns>gevonden lid met functies en persoon, of <c>null</c> indien niet gevonden</returns>
		public Lid LidZoeken(PersoonZoekInfo zoekInfo, string stamNummer, int werkJaar, kipadminEntities db)
		{
			// TODO (#555): Van zodra we met oud-leidingsploegen werken, zullen
			// we GroepID's uit Kipadmin moeten gebruiken.  Maar voorlopig dus
			// met stamnummer.

			var persoon = Zoeken(zoekInfo, false, db);
			if (persoon == null)
			{
				return null;
			}

			int groepID = (from g in db.Groep.OfType<ChiroGroep>()
				       where g.STAMNR == stamNummer
				       select g.GroepID).FirstOrDefault();

			var lid = (from l in db.Lid.Include("HeeftFunctie.Functie").Include("Persoon")
				   where l.Persoon.AdNummer == persoon.AdNummer
					 && l.Groep.GroepID == groepID
					 && l.werkjaar == werkJaar
				   select l).FirstOrDefault();

			return lid;
		}

        /// <summary>
        /// Voegt <paramref name="communicatie"/> toe als communicatiemiddel voor <paramref name="persoon"/>.
        /// </summary>
        /// <param name="communicatie">Toe te voegen communicatie</param>
        /// <param name="persoon">Persoon die communicatie krijgt</param>
        /// <param name="db">Object context</param>
        /// <returns><c>false</c> als de communicatievorm al bestond, anders <c>true</c>.</returns>
        public bool CommunicatieToevoegen(CommunicatieMiddel communicatie, Persoon persoon, kipadminEntities db)
        {
            var bestaande = (from ci in persoon.kipContactInfo
                            where ci.ContactTypeId == (int)communicatie.Type
                            select ci).ToList();

            // Voeg enkel toe als nog niet bestaat.

            var gevonden = (from ci in bestaande
                            where
                                String.Compare(ci.Info, communicatie.Waarde,
                                               StringComparison.OrdinalIgnoreCase) == 0
                            select ci).FirstOrDefault();

            if (gevonden == null)
            {
                // volgnummer bepalen
                int volgnr;

                if (bestaande.FirstOrDefault() == null)
                {
                    // Er bestaan er nog geen: volgnr = 1
                    volgnr = 1;
                }
                else
                {
                    volgnr = (from ci in bestaande select ci.VolgNr).Max() + 1;
                }

                var contactinfo = new ContactInfo
                                  {
                                      ContactInfoId = 0,
                                      ContactTypeId = (int) communicatie.Type,
                                      GeenMailings = communicatie.GeenMailings,
                                      Info = communicatie.Waarde,
                                      kipPersoon = persoon,
                                      VolgNr = volgnr
                                  };
                db.AddToContactInfoSet(contactinfo);
                db.SaveChanges();

                return true;
            }

            // communicatievorm was wel gevonden. Neem 'GeenMailings' over.

            gevonden.GeenMailings = communicatie.GeenMailings;
            db.SaveChanges();
            
            return false;
        }

	}

	/// <summary>
	/// Informatie die gebruikt wordt om een persoon op te zoeken
	/// </summary>
	public class PersoonZoekInfo
	{
		public int? AdNummer { get; set; }
		public int? GapID { get; set; }
		public string Naam { get; set; }
		public string VoorNaam { get; set; }
		public int Geslacht { get; set; }
		public IEnumerable<string> Communicatie { get; set; }
		public DateTime? GeboorteDatum { get; set; }
		public string PostNr { get; set; }
	}
}
