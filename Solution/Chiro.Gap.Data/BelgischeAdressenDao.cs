// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor adressen
	/// </summary>
	public class BelgischeAdressenDao : Dao<BelgischAdres, ChiroGroepEntities>, IBelgischeAdressenDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor adressen
		/// </summary>
		public BelgischeAdressenDao()
		{
			ConnectedEntities = new Expression<Func<BelgischAdres, object>>[3] 
			    { 
						e => e.StraatNaam.WithoutUpdate(),
						e => e.WoonPlaats.WithoutUpdate(),
						e => e.PersoonsAdres.First().Persoon.WithoutUpdate() 
			    };
		}

		// TODO (#655): kan deze methode ook vervangen worden door de generische 'bewaren met lambda expressies'??
		// Ja hoor, maar ik heb nu geen tijd :-)

		/// <summary>
		/// Bewaart adres en eventuele gekoppelde persoonsadressen, 
		/// Personen en GelieerdePersonen.
		/// </summary>
		/// <param name="adr">Te bewaren adres</param>
		/// <returns>Referentie naar het bewaarde adres</returns>
		/// <opmerking>ID en Versie worden aangepast in Adres, eventuele
		/// gewijzigde velden in gerelateerde entity's niet!</opmerking>
		public override BelgischAdres Bewaren(BelgischAdres adr)
		{
			// Deze assertions moeten eigenlijk afgedwongen worden
			// door de businesslaag.  En eigenlijk moet deze method ook
			// werken zonder die asserties (en dan de juiste dingen
			// bijcreeren).
			// Voorlopig niet dus.

			Debug.Assert(adr.StraatNaam != null);
			Debug.Assert(adr.WoonPlaats != null);
			Debug.Assert(adr.StraatNaam.ID != 0);
			Debug.Assert(adr.WoonPlaats.ID != 0);

			using (var db = new ChiroGroepEntities())
			{
				db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

				var geattachtAdres = db.AttachObjectGraph(
									adr,
									e => e.StraatNaam.WithoutUpdate(),
									e => e.WoonPlaats.WithoutUpdate(),
									e => e.PersoonsAdres.First().Adres,
									e => e.PersoonsAdres.First().Persoon.WithoutUpdate(),
									e => e.PersoonsAdres.First().GelieerdePersoon);

				// bewaardAdres is het geattachte adres.  Hiervan neem
				// ik ID en versie over; de rest laat ik ongemoeid.
				//
				// (waardoor rowversies van gekoppelde entity's niet
				// meer zullen kloppen :(.  Maar ik kan bewaardadres
				// ook niet detachen zonder de navigation property's
				// te verliezen :(.)

				db.SaveChanges();

				adr.ID = geattachtAdres.ID;
				adr.Versie = geattachtAdres.Versie;
			}
			return adr;
		}

		/// <summary>
		/// Haalt adres op op basis van criteria
		/// </summary>
		/// <param name="straatNaam">De naam van de straat</param>
		/// <param name="huisNr">Het huisnummer</param>
		/// <param name="bus">Het eventuele busnummer</param>
		/// <param name="postNr">Het postnummer</param>
		/// <param name="postCode">De lettercode die in het buitenland aan postnummers toegevoegd wordt</param>
		/// <param name="woonPlaatsNaam">Naam van de woonplaats</param>
		/// <param name="metBewoners">Bij <c>true</c> worden ook de
		/// persoonsadressen en gelieerde personen opgehaald</param>
		/// <returns>Een adres als gevonden, anders null</returns>
		public BelgischAdres Ophalen(
			string straatNaam, int? huisNr, string bus,
			int postNr, string postCode, string woonPlaatsNaam,
			bool metBewoners)
		{
			BelgischAdres resultaat;

			using (var db = new ChiroGroepEntities())
			{
				db.Adres.MergeOption = MergeOption.NoTracking;

				var adressentabel = db.Adres.OfType<BelgischAdres>().Include(adr => adr.StraatNaam).Include(adr => adr.WoonPlaats);

				if (metBewoners)
				{
					adressentabel = adressentabel.Include(adr => adr.PersoonsAdres);
				}

				resultaat = (
					from a in adressentabel
					where (a.StraatNaam.Naam == straatNaam && a.StraatNaam.PostNummer == postNr
					&& a.WoonPlaats.Naam == woonPlaatsNaam && a.WoonPlaats.PostNummer == postNr
					&& (a.HuisNr == null && huisNr == null || a.HuisNr == huisNr)
					&& (a.Bus == null && bus == null || a.Bus == bus))
					select a).FirstOrDefault();

				// Gekke constructie voor huisnummer, bus en postcode, omdat null anders niet goed
				// opgevangen wordt.  (je krijgt bijv. where PostCode == null in de SQL query, wat niet werkt)

				// Eager loading lukt blijkbaar wel sinds .net 4, dus de workaround die hieronder stond, mag weg.

			}

			return resultaat;
		}

		/// <summary>
		/// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groepen
		/// met IDs in <paramref name="groepIDs"/>
		/// </summary>
		/// <param name="adresID">ID van het op te halen adres</param>
		/// <param name="groepIDs">ID van de groep waaruit bewoners moeten worden gehaald</param>
		/// <param name="alleGelieerdePersonen">Indien <c>true</c> worden alle gelieerde personen van de bewoners mee opgehaald, 
		/// inclusief de gelieerde personen die tot een andere groep behoren.</param>
		/// <returns>Het gevraagde adres met de relevante bewoners.</returns>
		/// <remarks>ALLE ANDERE ADRESSEN VAN DE GEKOPPELDE BEWONERS WORDEN OOK MEE OPGEHAALD</remarks>
		public BelgischAdres BewonersOphalen(int adresID, IEnumerable<int> groepIDs, bool alleGelieerdePersonen)
		{
			// TODO (#238): Dit moet weg uit een BelgischeAdressenDao, naar een algemene AdressenDao.
			BelgischAdres adres;

			using (var db = new ChiroGroepEntities())
			{
				IEnumerable<GelieerdePersoon> gps;

				if (alleGelieerdePersonen == false)
				{
					// alle gelieerde personen gelieerd aan groep, en wonend op adres

					gps = (from gp in db.GelieerdePersoon
							.Include(gp2 => gp2.Persoon.PersoonsAdres.First().Adres)
							.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.Groep.ID, groepIDs))
					       where gp.Persoon.PersoonsAdres.Any(pa => pa.Adres.ID == adresID)
					       select gp).ToArray();
				}
				else
				{
					// Personen gekoppeld aan de gelieerde personen uit de gevraagde groepen wonend op de gevraagde
					// adressen.

					var pers = (from gp in db.GelieerdePersoon
							.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.Groep.ID, groepIDs))
						    where gp.Persoon.PersoonsAdres.Any(pa => pa.Adres.ID == adresID)
						    select gp.Persoon);

					// selecteer nu alle gelieerde personen gekoppeld aan de personen

					var query = (from gp in pers.SelectMany(p => p.GelieerdePersoon) select gp) as ObjectQuery<GelieerdePersoon>;

					gps = query
						.Include(gp => gp.Persoon.PersoonsAdres.First().Adres)
						.ToArray();
				}

				// Ik heb nu alle gelieerde personen die ik nodig heb, met al hun adressen.
				// Eerst moeten straten en woonplaatsen van die adressen mee opgehaald worden.

				foreach (var pa in gps.SelectMany(gp => gp.Persoon.PersoonsAdres))
				{
					if (pa.Adres is BelgischAdres)
					{
						((BelgischAdres)pa.Adres).StraatNaamReference.Load();
						((BelgischAdres)pa.Adres).WoonPlaatsReference.Load();
					}
					else if (pa.Adres is BuitenLandsAdres)
					{
						((BuitenLandsAdres)pa.Adres).LandReference.Load();
					}
				}


				// Ik kies uit de eerste het goeie adres, en dan heb ik normaalgezien alles wat ik nodg heb.

				adres = (from pa in gps.First().Persoon.PersoonsAdres
					 where pa.Adres.ID == adresID
					 select pa.Adres).FirstOrDefault() as BelgischAdres;

				if (adres != null)
				{
					adres.StraatNaamReference.Load();
					adres.WoonPlaatsReference.Load();
				}
			}

			return Utility.DetachObjectGraph(adres);
		}
	}
}
