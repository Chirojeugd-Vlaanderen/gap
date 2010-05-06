// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

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

		/// <summary>
		/// Creëert nieuwe adressenmanager
		/// </summary>
		/// <param name="dao">Repository voor adressen</param>
		/// <param name="stratenDao">Repository voor straten</param>
		/// <param name="subgemeenteDao">Repository voor 'subgemeentes'</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		public AdressenManager(IAdressenDao dao, IStratenDao stratenDao, ISubgemeenteDao subgemeenteDao, IAutorisatieManager autorisatieMgr)
		{
			_dao = dao;
			_stratenDao = stratenDao;
			_subgemeenteDao = subgemeenteDao;
			_autorisatieMgr = autorisatieMgr;
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
		/// Persisteert adres in de database
		/// </summary>
		/// <param name="adr">Te persisteren adres</param>
		/// <returns>Het adres met eventueel nieuw ID</returns>
		public Adres Bewaren(Adres adr)
		{
			return _dao.Bewaren(adr);
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

			// Al maar preventief een VerhuisFault aanmaken.  Als daar uiteindelijk
			// geen foutberichten inzitten, dan is er geen probleem.  Anders
			// creëer ik een exception met de verhuisfault daarin.

			Debug.Assert(straatNaam != String.Empty);
			Debug.Assert(postNr > 0);
			// Debug.Assert(HuisNr > 0);
			Debug.Assert(woonPlaatsNaam != String.Empty);

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
						FoutNummer = FoutNummers.StraatNietGevonden,
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
						FoutNummer = FoutNummers.WoonPlaatsNietGevonden,
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
			return StratenOphalen(straatBegin, new int[] { postNr });
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
