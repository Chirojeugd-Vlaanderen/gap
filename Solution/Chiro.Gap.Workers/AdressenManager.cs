// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. adressen bevat
	/// </summary>
	public class AdressenManager
	{
		private IAdressenDao _dao;
		private IStratenDao _stratenDao;
		private ISubgemeenteDao _subgemeenteDao;
		private IAutorisatieManager _autorisatieMgr;

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
		///  - subgemeente.naam
		/// <para />
		/// Als er zo geen adres bestaat, wordt het aangemaakt, op
		/// voorwaarde dat de straat en subgemeente geidentificeerd
		/// kunnen worden.  Als ook dat laatste niet het geval is,
		/// wordt een exception gethrowd.
		/// </summary>
		/// <param name="StraatNaam">De naam van de straat</param>
		/// <param name="HuisNr">Het huisnummer</param>
		/// <param name="Bus">Het eventuele busnummer</param>
		/// <param name="GemeenteNaam">De naam van de gemeente</param>
		/// <param name="PostNr">Het postnummer van de gemeente</param>
		/// <param name="PostCode">Tekst die in het buitenland volgt op postnummers</param>
		/// <returns>Gevonden adres</returns>
		/// <remarks>Ieder heeft het recht adressen op te zoeken</remarks>
		public Adres ZoekenOfMaken(String StraatNaam, int HuisNr, String Bus, String GemeenteNaam, int PostNr, String PostCode)
		{
			AdresFault fault = new AdresFault();

			// Al maar preventief een VerhuisFault aanmaken.  Als daar uiteindelijk
			// geen foutberichten inzitten, dan is er geen probleem.  Anders
			// creëer ik een exception met de verhuisfault daarin.

			Adres adresInDb;

			Debug.Assert(StraatNaam != String.Empty);
			Debug.Assert(PostNr > 0);
			// Debug.Assert(HuisNr > 0);
			Debug.Assert(GemeenteNaam != String.Empty);

			adresInDb = _dao.Ophalen(StraatNaam, HuisNr, Bus, PostNr, PostCode, GemeenteNaam, false);

			Adres adr = new Adres();

			if (adresInDb == null)
			{
				// Adres niet gevonden.  Probeer straat en gemeente te vinden

				StraatNaam s;
				WoonPlaats sg;

				s = _stratenDao.Ophalen(StraatNaam, PostNr);
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
					fault.BerichtToevoegen(AdresFaultCode.OnbekendeStraat, "Straat",
						String.Format("Straat {0} met postnummer {1} niet gevonden.", StraatNaam, PostNr));
				}

				sg = _subgemeenteDao.Ophalen(GemeenteNaam, PostNr);
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
					fault.BerichtToevoegen(AdresFaultCode.OnbekendeGemeente, "Gemeente",
						String.Format("Deelgemeente {0} met postnummer {1} niet gevonden.", GemeenteNaam, PostNr));
				}

				if (fault.Berichten.Count != 0)
				{
					throw new AdresException(fault);
				}

				if (PostCode != null && !PostCode.Equals(String.Empty))
				{
					adr.PostCode = PostCode;
				}
				adr.HuisNr = HuisNr;
				adr.Bus = Bus;

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
