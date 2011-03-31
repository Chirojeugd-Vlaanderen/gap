using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class AdressenDao : Dao<Adres, ChiroGroepEntities>, IAdressenDao
	{
		/// <summary>
		/// Haalt het adres op, samen met straat en woonplaats voor Belgisch adres, en samen met land voor
		/// buitenlands adres.
		/// </summary>
		/// <param name="id">ID op te halen adres</param>
		/// <param name="paths">bepaalt mee op te halen gekoppelde entiteiten</param>
		/// <returns>Het gevraagde adres</returns>
		public override Adres Ophalen(int id, params Expression<Func<Adres, object>>[] paths)
		{
			Adres resultaat;

			using (var db = new ChiroGroepEntities())
			{
				db.Adres.MergeOption = MergeOption.NoTracking;

				// Haal basisadres op

				resultaat = base.Ophalen(id, paths);

				// Koppel nu de relevante gekoppelde entiteiten.

				if (resultaat is BelgischAdres)
				{
					((BelgischAdres) resultaat).StraatNaamReference.Load();
					((BelgischAdres)resultaat).WoonPlaatsReference.Load();
				}
				else if (resultaat is BuitenLandsAdres)
				{
					((BuitenLandsAdres) resultaat).LandReference.Load();
				}

			}

			return resultaat;
		}

		/// <summary>
		/// Haalt adres op, op basis van de adresgegevens
		/// </summary>
		/// <param name="adresInfo">adresgegevens</param>
		/// <param name="metBewoners">Indien <c>true</c>, worden ook de PersoonsAdressen
		/// opgehaald.  (ALLE persoonsadressen gekoppeld aan het adres; niet
		/// zomaar over de lijn sturen dus)</param>
		/// <returns>Gevraagd adresobject</returns>
		public Adres Ophalen(
			AdresInfo adresInfo,
			bool metBewoners)
		{
			Adres resultaat;

			using (var db = new ChiroGroepEntities())
			{
				db.Adres.MergeOption = MergeOption.NoTracking;

				if (String.IsNullOrEmpty(adresInfo.LandNaam) || String.Compare(adresInfo.LandNaam, Properties.Resources.Belgie, true) == 0)
				{
					// Belgisch adres

					var adressentabel = db.Adres.OfType<BelgischAdres>().Include(adr => adr.StraatNaam).Include(adr => adr.WoonPlaats);

					if (metBewoners)
					{
						adressentabel = adressentabel.Include(adr => adr.PersoonsAdres);
					}

					resultaat = (
					            	from a in adressentabel
					            	where (a.StraatNaam.Naam == adresInfo.StraatNaamNaam && a.StraatNaam.PostNummer == adresInfo.PostNr
					            	       && a.WoonPlaats.Naam == adresInfo.WoonPlaatsNaam && a.WoonPlaats.PostNummer == adresInfo.PostNr
					            	       && (a.HuisNr == null && adresInfo.HuisNr == null || a.HuisNr == adresInfo.HuisNr)
					            	       && (a.Bus == null && adresInfo.Bus == null || a.Bus == adresInfo.Bus))
					            	select a).FirstOrDefault();

					// Gekke constructie voor huisnummer, bus en postcode, omdat null anders niet goed
					// opgevangen wordt.  (je krijgt bijv. where PostCode == null in de SQL query, wat niet werkt)
				}
				else
				{
					var adressentabel = db.Adres.OfType<BuitenLandsAdres>().Include(adr => adr.Land);

					if (metBewoners)
					{
						adressentabel = adressentabel.Include(adr => adr.PersoonsAdres);
					}

					resultaat = (
							from a in adressentabel
							where (a.Straat == adresInfo.StraatNaamNaam 
							       && a.WoonPlaats == adresInfo.WoonPlaatsNaam
							       && a.PostNummer == adresInfo.PostNr
							       && a.PostCode == adresInfo.PostCode
							       && (a.HuisNr == null && adresInfo.HuisNr == null || a.HuisNr == adresInfo.HuisNr)
							       && (a.Bus == null && adresInfo.Bus == null || a.Bus == adresInfo.Bus)
							       && (a.Land.Naam == adresInfo.LandNaam))
							select a).FirstOrDefault();

					// Gekke constructie voor huisnummer, bus en postcode, omdat null anders niet goed
					// opgevangen wordt.  (je krijgt bijv. where PostCode == null in de SQL query, wat niet werkt)
					
				}

			}

			return resultaat;
		}

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
			if (adr is BelgischAdres)
			{
				// Deze assertions moeten eigenlijk afgedwongen worden
				// door de businesslaag.  En eigenlijk moet deze method ook
				// werken zonder die asserties (en dan de juiste dingen
				// bijcreeren).
				// Voorlopig niet dus.

				Debug.Assert(((BelgischAdres)adr).StraatNaam != null);
				Debug.Assert(((BelgischAdres)adr).WoonPlaats != null);
				Debug.Assert(((BelgischAdres)adr).StraatNaam.ID != 0);
				Debug.Assert(((BelgischAdres)adr).WoonPlaats.ID != 0);
			}
			else
			{
				Debug.Assert(adr is BuitenLandsAdres);
				Debug.Assert(((BuitenLandsAdres)adr).Land != null);
				Debug.Assert(((BuitenLandsAdres)adr).Land.ID != 0);
			}

			using (var db = new ChiroGroepEntities())
			{
				Adres geattachtAdres;

				db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

				if (adr is BelgischAdres)
				{
					geattachtAdres = db.AttachObjectGraph(
						(BelgischAdres)adr,
						e => e.StraatNaam.WithoutUpdate(),
						e => e.WoonPlaats.WithoutUpdate(),
						e => e.PersoonsAdres.First().Adres,
						e => e.PersoonsAdres.First().Persoon.WithoutUpdate(),
						e => e.PersoonsAdres.First().GelieerdePersoon);
				}
				else
				{
					Debug.Assert(adr is BuitenLandsAdres);
					geattachtAdres = db.AttachObjectGraph(
						(BuitenLandsAdres) adr,
						e => e.Land.WithoutUpdate(),
						e => e.PersoonsAdres.First().Adres,
						e => e.PersoonsAdres.First().Persoon.WithoutUpdate(),
						e => e.PersoonsAdres.First().GelieerdePersoon);
				}


				// geattachtAdres is het geattachte adres.  Hiervan neem
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
		/// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groepen
		/// met IDs in <paramref name="groepIDs"/>
		/// </summary>
		/// <param name="adresID">ID van het op te halen adres</param>
		/// <param name="groepIDs">ID van de groep waaruit bewoners moeten worden gehaald</param>
		/// <param name="alleGelieerdePersonen">Indien <c>true</c> worden alle gelieerde personen van de bewoners mee opgehaald, 
		/// inclusief de gelieerde personen die tot een andere groep behoren.</param>
		/// <returns>Het gevraagde adres met de relevante bewoners.</returns>
		/// <remarks>ALLE ANDERE ADRESSEN VAN DE GEKOPPELDE BEWONERS WORDEN OOK MEE OPGEHAALD</remarks>
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
					 select pa.Adres).FirstOrDefault();

				if (adres is BelgischAdres)
				{
					((BelgischAdres)adres).StraatNaamReference.Load();
					((BelgischAdres)adres).WoonPlaatsReference.Load();
				}
				else if (adres is BuitenLandsAdres)
				{
					((BuitenLandsAdres) adres).LandReference.Load();
				}
			}

			return Utility.DetachObjectGraph(adres);
		}

	}
}
