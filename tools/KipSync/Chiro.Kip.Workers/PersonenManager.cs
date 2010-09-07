using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Chiro.Cdf.Data.Entity;
using Chiro.Kip.Data;

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
		/// Er mag gerust een deel van de info in <paramref name="zoekInfo"/> ontbreken</remarks>
		public Persoon Zoeken(PersoonZoekInfo zoekInfo, bool maken, kipadminEntities db)
		{
			Persoon resultaat;

			// poging 1
			if (zoekInfo.AdNummer != null)
			{
				resultaat = (from p in db.PersoonSet.Include(prs => prs.kipContactInfo).Include(prs => prs.kipWoont.First().kipAdres)
					     where p.AdNummer == zoekInfo.AdNummer
					     select p).FirstOrDefault();
				if (resultaat != null) return resultaat;
			}

			// poging 2
			if (zoekInfo.GapID != null)
			{
				resultaat = (from p in db.PersoonSet.Include(prs => prs.kipContactInfo).Include(prs => prs.kipWoont.First().kipAdres)
					     where p.GapID == zoekInfo.GapID select p).FirstOrDefault();
				if (resultaat != null) return resultaat;
			}

			// Niemand gevonden op basis van ID's.  Dan moet er een naam en voornaam zijn, of we spelen niet meer mee.

			if (String.IsNullOrEmpty(zoekInfo.Naam) || String.IsNullOrEmpty(zoekInfo.VoorNaam))
			{
				throw new InvalidOperationException(Properties.Resources.OnvoldoendePersoonsInfo);
			}

			// Zoeken bij naamgenoten.

			var naamgenoten = (from p in db.PersoonSet
						.Include(prs => prs.kipContactInfo)
						.Include(prs => prs.kipWoont.First().kipAdres)
					   where p.Naam == zoekInfo.Naam && p.VoorNaam == zoekInfo.VoorNaam
						 && p.Geslacht == zoekInfo.Geslacht
					   select p);

			// Poging 3
			if (zoekInfo.Communicatie != null)
			{
				resultaat = naamgenoten.SelectMany(ng => ng.kipContactInfo)
					.Where(Utility.BuildContainsExpression<ContactInfo, string>(ci => ci.Info, zoekInfo.Communicatie))
					.Select(ci => ci.kipPersoon).FirstOrDefault();
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
			resultaat.BurgerlijkeStandId = Properties.Settings.Default.StandaardBurgerlijkeStaat;
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

			var lid = (from l in db.Lid.Include(ld => ld.HeeftFunctie.First().Functie).Include(ld=>ld.Persoon)
				   where l.Persoon.AdNummer == persoon.AdNummer
					 && l.Groep.GroepID == groepID
					 && l.werkjaar == werkJaar
				   select l).FirstOrDefault();

			return lid;
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
		public int? PostNr { get; set; }
	}
}
