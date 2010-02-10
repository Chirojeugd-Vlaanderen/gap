using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using System.Linq.Expressions;


namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Data access voor adressen; zie IAdressenDao voor documentatie.
	/// </summary>
	public class AdressenDao : Dao<Adres, ChiroGroepEntities>, IAdressenDao
	{
		public AdressenDao()
		{
			connectedEntities = new Expression<Func<Adres, object>>[3] { 
                                        e=>e.Straat.WithoutUpdate(),
                                        e=>e.Subgemeente.WithoutUpdate(),
                                        e=>e.PersoonsAdres.First().Persoon.WithoutUpdate() };
		}

		//TODO kan deze methode ook vervangen worden door de generische bewaren met lambda expressies??

		/// <summary>
		/// Bewaart adres en eventuele gekoppelde persoonsadressen en 
		/// Personen.
		/// </summary>
		/// <param name="nieuweEntiteit">Te bewaren adres</param>
		/// <returns>referentie naar het bewaarde adres</returns>
		/// <opmerking>ID en Versie worden aangepast in Adres, eventuele
		/// gewijzigde velden in gerelateerde entity's niet!</opmerking>
		public override Adres Bewaren(Adres adr)
		{
			// Deze assertions moeten eigenlijk afgedwongen worden
			// door de businesslaag.  En eigenlijk moet deze method ook
			// werken zonder die asserties (en dan de juiste dingen
			// bijcreeren).
			// Voorlopig niet dus.

			Debug.Assert(adr.Straat != null);
			Debug.Assert(adr.Subgemeente != null);
			Debug.Assert(adr.Straat.ID != 0);
			Debug.Assert(adr.Subgemeente.ID != 0);

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

				Adres geattachtAdres = db.AttachObjectGraph(adr, e => e.Straat.WithoutUpdate()
									, e => e.Subgemeente.WithoutUpdate()
									, e => e.PersoonsAdres.First().Adres
									, e => e.PersoonsAdres.First().Persoon.WithoutUpdate());

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

			using (ChiroGroepEntities db = new ChiroGroepEntities())
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
				    = db.PersoonsAdres.Include("Adres.Straat")
				    .Include("Adres.Subgemeente")
				    .Include("Persoon")
				    .Where(pera => pera.Adres.ID == adresID);


				if (user != "")
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
			};

			resultaat = lijst[0].Adres;

			return resultaat;
		}

		/// <summary>
		/// Haalt adres op op basis van criteria
		/// </summary>
		/// <param name="straatNaam"></param>
		/// <param name="huisNr"></param>
		/// <param name="bus"></param>
		/// <param name="postNr"></param>
		/// <param name="postCode"></param>
		/// <param name="gemeenteNaam"></param>
		/// <param name="metBewoners">indien true, worden ook de
		/// persoonsadressen en gelieerde personen opgehaald</param>
		/// <returns>Een adres als gevonden, anders null</returns>
		public Adres Ophalen(string straatNaam, int? huisNr, string bus, int postNr, string postCode, string gemeenteNaam, bool metBewoners)
		{
			Adres resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Adres.MergeOption = MergeOption.NoTracking;

				var adressentabel = db.Adres.Include("Straat").Include("Subgemeente");

				if (metBewoners)
				{
					adressentabel = adressentabel.Include("PersoonsAdres");
				}

				resultaat = (
				    from Adres a in adressentabel
				    where (a.Straat.Naam == straatNaam && a.Subgemeente.Naam == gemeenteNaam
				    && a.Straat.PostNr == postNr
				    && a.HuisNr == huisNr && a.Bus == bus && a.PostCode == postCode)
				    select a).FirstOrDefault();

				// Alweer een rariteit met entity framework:
				// De 'eager loading' van Straat en Subgemeente werkt niet.
				//
				// Dan moet het maar zo:

				if (resultaat != null)
				{
					// Dit is uiteraard enkel zinvol als er iets gevonden is.

					resultaat.StraatReference.Load();
					resultaat.SubgemeenteReference.Load();

					if (metBewoners)
					{
						resultaat.PersoonsAdres.Load();
					}

				}
			}

			return resultaat;
		}
	}
}
