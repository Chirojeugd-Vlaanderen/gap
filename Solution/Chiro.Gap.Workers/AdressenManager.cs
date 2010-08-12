// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.KipSync;

using Adres = Chiro.Gap.Orm.Adres;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. adressen bevat
	/// </summary>
	public class AdressenManager
	{
		private readonly IAdressenDao _dao;
		private readonly IStratenDao _stratenDao;
		private readonly ISubgemeenteDao _subgemeenteDao;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly ISyncPersoonService _sync;

		/// <summary>
		/// Creëert nieuwe adressenmanager
		/// </summary>
		/// <param name="dao">Repository voor adressen</param>
		/// <param name="stratenDao">Repository voor straten</param>
		/// <param name="subgemeenteDao">Repository voor 'subgemeentes'</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		/// <param name="sync">Synchronisatieservice naar Kipadmin</param>
		public AdressenManager(
			IAdressenDao dao, 
			IStratenDao stratenDao, 
			ISubgemeenteDao subgemeenteDao, 
			IAutorisatieManager autorisatieMgr,
			ISyncPersoonService sync)
		{
			_dao = dao;
			_stratenDao = stratenDao;
			_subgemeenteDao = subgemeenteDao;
			_autorisatieMgr = autorisatieMgr;
			_sync = sync;
		}

		#region proxy naar data acces

		/// <summary>
		/// Haalt een adres op, inclusief bewoners waar de ingelogde
		/// user gebruikersrechten op heeft.
		/// </summary>
		/// <param name="adresID">ID van het gevraagde adres</param>
		/// <returns>Adres met daaraan gekoppeld de bewoners</returns>
		public Adres AdresMetBewonersOphalen(int adresID)
		{
			// Adressen zitten vast in de database, en daar is niets
			// interessants over te zeggen.  Voorlopig mag iedereen elk
			// adres opzoeken.  Voor de bewonersgegevens worden de
			// rechten uiteraard wel gecontroleerd.

			return _dao.BewonersOphalen(adresID, _autorisatieMgr.GebruikersNaamGet());
		}

		/// <summary>
		/// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groep met ID
		/// <paramref name="groepID"/>
		/// </summary>
		/// <param name="adresID">ID van het op te halen adres</param>
		/// <param name="groepID">ID van de groep waaruit bewoners moeten worden gehaald</param>
		/// <returns>Het gevraagde adres met de relevante bewoners.</returns>
		public Adres AdresMetBewonersOphalen(int adresID, int groepID)
		{
			return AdresMetBewonersOphalen(adresID, groepID, false);
		}

		/// <summary>
		/// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groep met ID
		/// <paramref name="groepID"/>
		/// </summary>
		/// <param name="adresID">ID van het op te halen adres</param>
		/// <param name="groepID">ID van de groep waaruit bewoners moeten worden gehaald</param>
		/// <param name="alleGelieerdePersonen">Indien true, worden alle gelieerde personen van de bewoners mee opgehaald,
		/// ook diegene waar je geen GAV voor bent.</param>
		/// <returns>Het gevraagde adres met de relevante bewoners.</returns>
		public Adres AdresMetBewonersOphalen(int adresID, int groepID, bool alleGelieerdePersonen)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.BewonersOphalen(adresID, new[] { groepID }, alleGelieerdePersonen);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groepen met ID
		/// in <paramref name="groepIDs"/>
		/// </summary>
		/// <param name="adresID">ID van het op te halen adres</param>
		/// <param name="groepIDs">ID van de groepen waaruit bewoners moeten worden gehaald</param>
		/// <param name="alleGelieerdePersonen">Indien true, worden alle gelieerde personen van de bewoners mee opgehaald,
		/// ook diegene waar je geen GAV voor bent.</param>
		/// <returns>Het gevraagde adres met de relevante bewoners.</returns>
		public Adres AdresMetBewonersOphalen(int adresID, IEnumerable<int> groepIDs, bool alleGelieerdePersonen)
		{
			if (_autorisatieMgr.IsGavGroepen(groepIDs))
			{
				return _dao.BewonersOphalen(adresID, groepIDs, alleGelieerdePersonen);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Persisteert adres in de database, samen met alle gekoppelde personen en gelieerde personen.
		/// </summary>
		/// <param name="adr">Te persisteren adres</param>
		/// <returns>Het adres met eventueel nieuw ID</returns>
		public Adres Bewaren(Adres adr)
		{
			Adres resultaat;
#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				// enkel voorkeursadressen van personen met ad-nummer naar Kipadmin
				// Dit kan voor heen-en-weer-effecten in Kipadmin zorgen, als 2 groepen
				// beurtelings hun voorkeuradres aanpassen.
				// (Gelukkig wordt er momenteel niet teruggesynct van kipadmin naar gap)

				// check op voorkeursadres is een beetje tricky:
				// als een persoonsadres een gelieerde persoon heeft, dan wil dat zeggen dat
				// het persoonsadres het voorkeursadres is van die gelieerde persoon.
				// De persoon van het persoonsadres moet altijd dezelfde zijn als de persoon van
				// de gelieerde persoon.
				//
				// Bijwerking: als je een adres wijzigt, dat voor jouw groep niet standaard is,
				// maar voor een andere groep wel, dan gaat dat adres *toch* als standaardadres
				// naar Kipadmin.  Maar dat is op zich geen probleem.

				var teSyncen = from pa in adr.PersoonsAdres
				               where pa.GelieerdePersoon.Count > 0 // voorkeursadres
				                     && pa.Persoon.AdNummer != null	// met ad-nummer
				               select new KipSync.Bewoner
				                      	{
				                      		AdNummer = pa.Persoon.AdNummer ?? 0,
								AdresType = (KipSync.AdresTypeEnum)pa.AdresType
				                      	};

				// TODO (#238): Buitenlandse adressen!

				var syncAdres = new KipSync.Adres
				                	{
				                		Bus = adr.Bus,
				                		HuisNr = adr.HuisNr,
				                		Land = "",
				                		PostNr = adr.StraatNaam.PostNummer,
				                		Straat = adr.StraatNaam.Naam,
				                		WoonPlaats = adr.WoonPlaats.Naam
				                	};

				_sync.VoorkeurAdresUpdated(syncAdres, teSyncen.ToList());

				resultaat = _dao.Bewaren(adr);
#if KIPDORP
				tx.Complete();
			}
#endif
			return resultaat;
		}

		#endregion

		/// <summary>
		/// Zoekt adres (incl. straat en subgemeente), op basis
		/// van
		///  - straat.naam
		///  - huisnummer
		///  - straat.postnr
		///  - woonplaats.id
		/// <para />
		/// Als er zo geen adres bestaat, wordt het aangemaakt, op
		/// voorwaarde dat de straat en subgemeente geidentificeerd
		/// kunnen worden.  Als ook dat laatste niet het geval is,
		/// wordt een exception gethrowd.
		/// </summary>
		/// <param name="straatNaam">De naam van de straat</param>
		/// <param name="huisNr">Het huisnummer</param>
		/// <param name="bus">Het eventuele busnummer</param>
		/// <param name="woonPlaatsNaam">De naam van de woonplaats</param>
		/// <param name="postNr">Het postnummer van straat en woonplaats</param>
		/// <param name="postCode">Tekst die in het buitenland volgt op postnummers</param>
		/// <returns>Gevonden adres</returns>
		/// <remarks>Ieder heeft het recht adressen op te zoeken</remarks>
		public Adres ZoekenOfMaken(String straatNaam, int? huisNr, string bus, string woonPlaatsNaam, int postNr, string postCode)
		{
			var problemen = new Dictionary<string, FoutBericht>();

			// Al maar preventief een collectie fouten verzamelen.  Als daar uiteindelijk
			// geen foutberichten inzitten, dan is er geen probleem.  Anders
			// creëer ik een exception met de verhuisfault daarin.

			if (straatNaam == String.Empty)
			{
				problemen.Add("StraatNaamNaam", new FoutBericht
				{
					FoutNummer = FoutNummer.StraatOntbreekt,
					Bericht = String.Format(
						Properties.Resources.StraatOntbreekt,
						straatNaam,
						postNr)
				});
			}

			if (postNr < 1000 || postNr > 9999)
			{
				problemen.Add("PostNr", new FoutBericht
				{
					FoutNummer = FoutNummer.OngeldigPostNummer,
					Bericht = String.Format(
						Properties.Resources.OngeldigPostNummer,
						straatNaam,
						postNr)
				});
			}

			if (woonPlaatsNaam == String.Empty)
			{
				problemen.Add("WoonPlaatsNaam", new FoutBericht
				{
					FoutNummer = FoutNummer.WoonPlaatsOntbreekt,
					Bericht = String.Format(
						Properties.Resources.WoonPlaatsOntbreekt,
						straatNaam,
						postNr)
				});
			}

			// Als er hier al fouten zijn: gewoon throwen.  Me hiel 't stad, mor ni me maa!

			if (problemen.Count != 0)
			{
				throw new OngeldigObjectException(problemen);
			}

			var adresInDb = _dao.Ophalen(straatNaam, huisNr, bus, postNr, postCode, woonPlaatsNaam, false);

			var adr = new Adres();

			if (adresInDb == null)
			{
				// Adres niet gevonden.  Probeer straat en gemeente te vinden

				var s = _stratenDao.Ophalen(straatNaam, postNr);
				if (s != null)
				{
					// Straat gevonden: aan adres koppelen

					adr.StraatNaam = s;
					s.Adres.Add(adr);
				}
				else
				{
					// Straat niet gevonden: foutbericht toevoegen

					// FIXME: Dit is geen propere manier van werken.  Die component 'Straat'
					// heeft betrekking op het datacontract 'AdresInfo', wat helemaal niet
					// van belang is in deze layer

					problemen.Add("StraatNaamNaam", new FoutBericht
					{
						FoutNummer = FoutNummer.StraatNietGevonden,
						Bericht = String.Format(
							Properties.Resources.StraatNietGevonden,
							straatNaam,
							postNr)
					});
				}

				var sg = _subgemeenteDao.Ophalen(woonPlaatsNaam, postNr);
				if (sg != null)
				{
					// Gemeente gevonden: aan adres koppelen

					adr.WoonPlaats = sg;
					sg.Adres.Add(adr);
				}
				else
				{
					// Gemeente niet gevonden: foutbericht toevoegen

					// FIXME: hier idem.

					problemen.Add("WoonPlaatsNaam", new FoutBericht
					{
						FoutNummer = FoutNummer.WoonPlaatsNietGevonden,
						Bericht = Properties.Resources.GemeenteNietGevonden
					});
				}

				if (problemen.Count != 0)
				{
					throw new OngeldigObjectException(problemen);
				}

				if (postCode != null && !postCode.Equals(String.Empty))
				{
					adr.PostCode = postCode;
				}
				adr.HuisNr = huisNr;
				adr.Bus = bus;

				adr = _dao.Bewaren(adr);
				// bewaren brengt Versie en ID automatisch in orde.

				return adr;
			}
			else
			{
				Debug.Assert(adresInDb.StraatNaam != null);
				Debug.Assert(adresInDb.WoonPlaats != null);

				return adresInDb;
			}
		}

		#region crab-ophalen

		/// <summary>
		/// Een lijst van subgemeenten ophalen
		/// </summary>
		/// <returns>Een lijst van subgemeenten</returns>
		public IList<WoonPlaats> GemeentesOphalen()
		{
			return _subgemeenteDao.AllesOphalen();
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		public IList<StraatNaam> StratenOphalen(String straatBegin, int postNr)
		{
			return StratenOphalen(straatBegin, new[] { postNr });
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		public IList<StraatNaam> StratenOphalen(String straatBegin, IEnumerable<int> postNrs)
		{
			return _stratenDao.MogelijkhedenOphalen(straatBegin, postNrs);
		}

		#endregion
	}
}
