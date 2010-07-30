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
	public class AdressenDao : Dao<Adres, ChiroGroepEntities>, IAdressenDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor adressen
		/// </summary>
		public AdressenDao()
		{
			connectedEntities = new Expression<Func<Adres, object>>[3] 
            { 
				e => e.StraatNaam.WithoutUpdate(),
				e => e.WoonPlaats.WithoutUpdate(),
				e => e.PersoonsAdres.First().Persoon.WithoutUpdate() 
            };
		}

		// TODO kan deze methode ook vervangen worden door de generische bewaren met lambda expressies??
		// Ja hoor, maar ik heb nu geen tijd :-)

		/// <summary>
		/// Bewaart adres en eventuele gekoppelde persoonsadressen, 
		/// Personen en GelieerdePersonen.
		/// </summary>
		/// <param name="adr">Te bewaren adres</param>
		/// <returns>Referentie naar het bewaarde adres</returns>
		/// <opmerking>ID en Versie worden aangepast in Adres, eventuele
		/// gewijzigde velden in gerelateerde entity's niet!</opmerking>
		public override Adres Bewaren(Adres adr)
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
		/// Haalt een adres op, samen met de gekoppelde personen
		/// </summary>
		/// <param name="adresID">ID op te halen adres</param>
		/// <param name="user">Als user gegeven, worden enkel de
		/// personen gekoppeld waar die user bekijkrechten op heeft.
		/// Als user de lege string is, worden alle bewoners meegeleverd.
		/// </param>
		/// <returns>Adresobject met gekoppelde personen</returns>
		public Adres BewonersOphalen(int adresID, string user)
		{
			// TODO: Code na te kijken

			Adres resultaat = null;
			IList<PersoonsAdres> lijst = null;

			using (var db = new ChiroGroepEntities())
			{
				db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

				// NoTracking schakelt object tracking uit.  Op die manier
				// moet het resultaat niet gedetachet worden.  Detachen
				// deed blijkbaar de navigation properties verdwijnen.
				// (Voor entity framework moeten alle objecten in een
				// graaf ofwel aan dezelfde context hangen, ofwel aan
				// geen context hangen.  Het adres detachen, koppelt het
				// los van al de rest.)

				var query
					= db.PersoonsAdres.Include(pa => pa.Adres.StraatNaam)
					.Include(pa => pa.Adres.WoonPlaats)
					.Include("Persoon")
					.Where(pera => pera.Adres.ID == adresID);

				if (user != String.Empty)
				{
					// Controleer op niet-vervallen gebruikersrecht

					query = query
						.Where(pera => pera.Persoon.GelieerdePersoon.Any(
						gp => gp.Groep.GebruikersRecht.Any(
							gr => gr.Gav.Login == user && (gr.VervalDatum == null
							|| gr.VervalDatum > DateTime.Now))));
				}
				query = query.Select(pera => pera);

				// Nadeel van de NoTracking is dat aan elk persoonsadres
				// een identieke kopie van hetzelfde adres zal hangen.
				// Dat moet sebiet manueel gefixt worden.

				lijst = query.ToList();
			}

			// FIXME: Voor het gemak ga ik ervan uit dat er steeds minstens
			// iemand op het gezochte adres woont.  Dat is uiteraard niet 
			// noodzakelijk het geval.

			// Als deze assert failt, dan zou het ook kunnen dat het adres niet bestaat!
			Debug.Assert(lijst.Count > 0);

			// Koppel nu alle PersoonsAdressen aan dezelfde instantie van Adres.
			// (lijst[0].Adres, vandaar dat we lijst[0] zelf niet moeten updaten)

			for (int i = 1; i < lijst.Count; ++i)
			{
				lijst[i].Adres = lijst[0].Adres;
				lijst[0].Adres.PersoonsAdres.Add(lijst[i]);
			}

			resultaat = lijst[0].Adres;

			return resultaat;
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
		public Adres Ophalen(
			string straatNaam, int? huisNr, string bus,
			int postNr, string postCode, string woonPlaatsNaam,
			bool metBewoners)
		{
			Adres resultaat;

			using (var db = new ChiroGroepEntities())
			{
				db.Adres.MergeOption = MergeOption.NoTracking;

				var adressentabel = db.Adres.Include(adr => adr.StraatNaam).Include(adr => adr.WoonPlaats);

				if (metBewoners)
				{
					adressentabel = adressentabel.Include(adr => adr.PersoonsAdres);
				}

				resultaat = (
					from Adres a in adressentabel
					where (a.StraatNaam.Naam == straatNaam && a.StraatNaam.PostNummer == postNr
					&& a.WoonPlaats.Naam == woonPlaatsNaam && a.WoonPlaats.PostNummer == postNr
					&& (a.HuisNr == null && huisNr == null || a.HuisNr == huisNr)
					&& (a.Bus == null && bus == null || a.Bus == bus)
					&& (a.PostCode == null && postCode == null || a.PostCode == postCode))
					select a).FirstOrDefault();

				// Gekke constructie voor huisnummer, bus en postcode, omdat null anders niet goed
				// opgevangen wordt.  (je krijgt bijv. where PostCode == null in de SQL query, wat niet werkt)

				// 'Eager loading' van straatnaam en woonplaats zou niet werken...
				// Voorlopig gaat het zo:

				if (resultaat != null)
				{
					// Dit is uiteraard enkel zinvol als er iets gevonden is.

					resultaat.StraatNaamReference.Load();
					resultaat.WoonPlaatsReference.Load();

					if (metBewoners)
					{
						resultaat.PersoonsAdres.Load();
					}
				}
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
		/// <remarks>Alle andere adressen van de gekoppelde bewoners worden helaas ook mee opgehaald</remarks>
		public Adres BewonersOphalen(int adresID, IEnumerable<int> groepIDs, bool alleGelieerdePersonen)
		{
			Adres adres;

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

					gps = query.Include(gp => gp.Persoon.PersoonsAdres.First().Adres).ToArray();
				}

				// Ik heb nu alle gelieerde personen die ik nodig heb, wel met veel te veel adressen, maar soit.
				// Ik kies uit de eerste het goeie adres, en dan heb ik normaalgezien alles wat ik nodg heb.

				adres = (from pa in gps.First().Persoon.PersoonsAdres
						 where pa.Adres.ID == adresID
						 select pa.Adres).FirstOrDefault();

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
