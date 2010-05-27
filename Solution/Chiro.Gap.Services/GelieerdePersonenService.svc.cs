// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using AutoMapper;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
	// OPM: als je de naam van de class "GelieerdePersonenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

	public class GelieerdePersonenService : IGelieerdePersonenService
	{
		#region Manager Injection

		private readonly GelieerdePersonenManager _gpMgr;
		private readonly PersonenManager _pMgr;
		private readonly LedenManager _lidMgr;
		private readonly AdressenManager _adrMgr;
		private readonly GroepenManager _grMgr;
		private readonly GroepsWerkJaarManager _gwjMgr;
		private readonly CommVormManager _cvMgr;
		private readonly CategorieenManager _catMgr;
		private readonly IAutorisatieManager _auMgr;

		public GelieerdePersonenService(
			GelieerdePersonenManager gpm,
			PersonenManager pm,
			AdressenManager adm,
			GroepenManager gm,
			GroepsWerkJaarManager gwjm,
			CommVormManager cvm,
			CategorieenManager cm,
			IAutorisatieManager aum,
			LedenManager lm)
		{
			_gpMgr = gpm;
			_pMgr = pm;
			_auMgr = aum;
			_adrMgr = adm;
			_grMgr = gm;
			_gwjMgr = gwjm;
			_cvMgr = cvm;
			_catMgr = cm;
			_lidMgr = lm;
		}

		#endregion

		#region IGelieerdePersonenService Members

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonDetail> PaginaOphalenUitCategorieMetLidInfo(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var gelieerdePersonen = _gpMgr.PaginaOphalenMetLidInfoVolgensCategorie(categorieID, pagina, paginaGrootte, out aantalTotaal);
			return Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonDetail>>(gelieerdePersonen);
		}

		// *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
		// je aangemeld bent, op je lokale computer in de groep CgUsers zit.
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonDetail> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var gelieerdePersonen = _gpMgr.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
			var result = Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonDetail>>(gelieerdePersonen);
			return result;
		}

		/// <summary>
		/// Updatet een persoon op basis van <paramref name="persoonInfo"/>
		/// </summary>
		/// <param name="persoonInfo">Info over te bewaren persoon</param>
		/// <returns>ID van de bewaarde persoon</returns>
		public int Bewaren(PersoonInfo persoonInfo)
		{
			// Haal eerst gelieerde persoon op.
			var gp = _gpMgr.Ophalen(persoonInfo.GelieerdePersoonID);
			gp.ChiroLeefTijd = persoonInfo.ChiroLeefTijd;

			Mapper.Map(persoonInfo, gp.Persoon);
			// In de hoop dat de members die geen 'Ignore hebben' overschreven worden,
			// en de andere niet.

			_gpMgr.Bewaren(gp);

			return persoonInfo.GelieerdePersoonID;
		}

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren: Chiroleeftijd, en
		/// de velden van <c>info.Persoon</c></param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.</remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public PersoonIDs Aanmaken(PersoonInfo info, int groepID)
		{
			return GeforceerdAanmaken(info, groepID, false);
		}

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren: Chiroleeftijd, en
		/// de velden van <c>info.Persoon</c></param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
		/// <param name="forceer">Als deze <c>true</c> is, wordt de nieuwe persoon sowieso gemaakt, ook
		/// al lijkt hij op een bestaande gelieerde persoon.  Is <paramref>force</paramref>
		/// <c>false</c>, dan wordt er een exceptie opgegooid als de persoon te hard lijkt op een
		/// bestaande.</param>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.</remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public PersoonIDs GeforceerdAanmaken(PersoonInfo info, int groepID, bool forceer)
		{
			// Indien 'forceer' niet gezet is, moet een FaultException opgeworpen worden
			// als de  nieuwe persoon te hard lijkt op een bestaande Gelieerde Persoon.
			
			// FIXME: Deze businesslogica moet in de workers gebeuren, waar dan een exception opgeworpen
			// kan worden, die we hier mappen op een faultcontract.

			var nieuwePersoon = new Persoon
			                    	{
			                    		AdNummer = info.AdNummer,
			                    		VoorNaam = info.VoorNaam,
			                    		Naam = info.Naam,
			                    		GeboorteDatum = info.GeboorteDatum,
			                    		Geslacht = info.Geslacht
			                    	};

			if (!forceer)
			{
				IList<GelieerdePersoon> bestaandePersonen =
					_gpMgr.ZoekGelijkaardig(nieuwePersoon, groepID);

				if (bestaandePersonen.Count > 0)
				{
					var fault = new BlokkerendeObjectenFault<PersoonDetail>{
						FoutNummer = FoutNummers.BestaatAl,
						Objecten = Mapper.Map<IList<GelieerdePersoon>, IList<PersoonDetail>>(bestaandePersonen) };

					throw new FaultException<BlokkerendeObjectenFault<PersoonDetail>>(fault);

					// ********************************************************************************
					// * BELANGRIJK: Als je debugger breakt op deze throw, dan is dat geen probleem.  *
					// * Dat wil gewoon zeggen dat er een gelieerde persoon gevonden is die lijkt op  *
					// * de nieuw toe te voegen persoon.  Er gaat een faultexception over de lijn,    *
					// * die door de UI gecatcht moet worden.                                         *
					// ********************************************************************************
				}
			}

			// De parameter 'info' wordt hier eigenlijk niet gebruikt als GelieerdePersoon,
			// maar als datacontract dat de persoonsinfo en de Chiroleeftijd bevat.

			Groep g = _grMgr.Ophalen(groepID);

			// Gebruik de businesslaag om info.Persoon te koppelen aan de opgehaalde groep.

			GelieerdePersoon gelieerd = _gpMgr.Koppelen(nieuwePersoon, g, info.ChiroLeefTijd);
			gelieerd = _gpMgr.Bewaren(gelieerd);
			return new PersoonIDs {GelieerdePersoonID = gelieerd.ID, PersoonID = gelieerd.Persoon.ID};
		}

		/// <summary>
		/// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
		/// </summary>
		/// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
		/// <returns>GelieerdePersoon met persoonsgegevens</returns>
		public PersoonDetail DetailsOphalen(int gelieerdePersoonID)
		{
			return Mapper.Map<GelieerdePersoon, PersoonDetail>(_gpMgr.DetailsOphalen(gelieerdePersoonID));
		}

		/// <summary>
		/// Haalt gelieerde persoon op met ALLE nodige info om het persoons-bewerken scherm te vullen:
		/// persoonsgegevens, categorieen, communicatievormen, lidinfo, afdelingsinfo, adressen
		/// functies
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gevraagde gelieerde persoon</param>
		/// <returns>
		/// Gelieerde persoon met ALLE nodige info om het persoons-bewerken scherm te vullen:
		/// persoonsgegevens, categorieen, communicatievormen, lidinfo, afdelingsinfo, adressen
		/// functies
		/// </returns>
		public PersoonLidInfo AlleDetailsOphalen(int gelieerdePersoonID)
		{
			var gp = _gpMgr.DetailsOphalen(gelieerdePersoonID);
			var pl = Mapper.Map<GelieerdePersoon, PersoonLidInfo>(gp);
			var gwj = _gwjMgr.RecentsteOphalen(gp.Groep.ID, GroepsWerkJaarExtras.GroepsFuncties);

			var l = _lidMgr.OphalenViaPersoon(gp.ID, gwj.ID);
			if (l != null)
			{
				var ff = Mapper.Map<Lid, LidInfo>(l);
				pl.LidInfo = ff;
			}
			
			return pl;
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public GezinInfo GezinOphalen(int adresID)
		{
			var adres = _adrMgr.AdresMetBewonersOphalen(adresID);
			var resultaat = Mapper.Map<Adres, GezinInfo>(adres);

			return resultaat;
		}

		/// <summary>
		/// Verhuist gelieerde personen van een oud naar een nieuw
		/// adres.
		/// (De koppelingen Persoon-Oudadres worden aangepast 
		/// naar Persoon-NieuwAdres.)
		/// </summary>
		/// <param name="persoonIDs">ID's van te verhuizen Personen (niet gelieerd!)</param>
		/// <param name="naarAdres">AdresInfo-object met nieuwe adresgegevens</param>
		/// <param name="oudAdresID">ID van het oude adres</param>
		/// <remarks>
		/// (1) nieuwAdres.ID wordt genegeerd.  Het adresID wordt altijd
		/// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
		/// dan krijgt het adres een nieuw ID. 
		/// <para/>
		/// (2) Deze functie werkt op PersoonID's en niet op
		/// GelieerdePersoonID's, en bijgevolg hoort dit eerder thuis
		/// in een PersonenService dan in een GelieerdePersonenService.
		/// </remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void Verhuizen(
			IEnumerable<int> persoonIDs,
			PersoonsAdresInfo naarAdres,
			int oudAdresID)
		{
			// Zoek adres op in database, of maak een nieuw.
			// (als straat en gemeente gekend)

			Adres nieuwAdres;
			try
			{
				nieuwAdres = _adrMgr.ZoekenOfMaken(
					naarAdres.StraatNaamNaam,
					naarAdres.HuisNr,
					naarAdres.Bus,
					naarAdres.WoonPlaatsNaam,
					naarAdres.PostNr,
					null);	// TODO: buitenlandse adressen (#238)
			}
			catch (OngeldigObjectException ex)
			{
				var fault = Mapper.Map<OngeldigObjectException, OngeldigObjectFault>(ex);

				throw new FaultException<OngeldigObjectFault>(fault);
			}

			// Haal te verhuizen personen op, samen met hun adressen.

			IEnumerable<Persoon> personenLijst = _pMgr.LijstOphalen(persoonIDs, PersoonsExtras.Adressen);

			// Kijk na of het naar-adres toevallig mee opgehaald is.  Zo ja, werken we daarmee verder
			// (iet of wat consistenter)

			PersoonsAdres a = personenLijst.SelectMany(prs => prs.PersoonsAdres)
				.Where(pa => pa.Adres.ID == nieuwAdres.ID).FirstOrDefault();

			if (a != null)
			{
				nieuwAdres = a.Adres;
			}

			// Het oud adres is normaal gezien gekoppeld aan een van de te verhuizen personen.

			Adres oudAdres = personenLijst.SelectMany(prs => prs.PersoonsAdres)
				.Where(pa => pa.Adres.ID == oudAdresID).Select(pa=>pa.Adres).FirstOrDefault();

			try
			{
				_pMgr.Verhuizen(personenLijst, oudAdres, nieuwAdres, naarAdres.AdresType);
			}
			catch (BlokkerendeObjectenException<PersoonsAdres> ex)
			{
				var fault = Mapper.Map<BlokkerendeObjectenException<PersoonsAdres>,
					BlokkerendeObjectenFault<PersoonsAdresInfo2>>(ex);

				throw new FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>>(fault);
			}

			// Persisteren
			_adrMgr.Bewaren(nieuwAdres);

			// Bij een verhuis, blijven de PersoonsAdresobjecten dezelfde,
			// maar worden ze aan een ander adres gekoppeld.  Een post
			// van het nieuwe adres (met persoonsadressen) koppelt bijgevolg
			// de persoonsobjecten los van het oude adres.
			// Bijgevolg moet het oudeAdres niet gepersisteerd worden.
		}

		/// <summary>
		/// Voegt een adres toe aan een verzameling personen
		/// </summary>
		/// <param name="personenIDs">ID's van Personen
		/// waaraan het nieuwe adres toegevoegd moet worden.</param>
		/// <param name="adr">Toe te voegen adres</param>
		public void AdresToevoegenPersonen(List<int> personenIDs, PersoonsAdresInfo adr, bool voorkeur)
		{
			// Dit gaat sterk lijken op verhuizen.

			// Adres opzoeken in database
			Adres adres;
			try
			{
				adres = _adrMgr.ZoekenOfMaken(adr.StraatNaamNaam, adr.HuisNr, adr.Bus, adr.WoonPlaatsNaam, adr.PostNr, null);
			}
			catch (OngeldigObjectException ex)
			{
				var fault = Mapper.Map<OngeldigObjectException,	OngeldigObjectFault>(ex);

				throw new FaultException<OngeldigObjectFault>(fault);
			}

			// Personen ophalen
			IEnumerable<Persoon> personenLijst = _pMgr.LijstOphalen(personenIDs, PersoonsExtras.Adressen);

			// Adres koppelen aan personen

			try
			{
				_pMgr.AdresToevoegen(personenLijst, adres, adr.AdresType, voorkeur);
			}
			catch (BlokkerendeObjectenException<PersoonsAdres> ex)
			{
				var fault = Mapper.Map<BlokkerendeObjectenException<PersoonsAdres>, BlokkerendeObjectenFault<PersoonsAdresInfo2>>(ex);

				throw new FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>>(fault);
			}

			// persisteren
			_adrMgr.Bewaren(adres);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AdresVerwijderenVanPersonen(List<int> personenIDs, int adresID)
		{
			// Adres ophalen, met bewoners voor GAV
			Adres adr = _adrMgr.AdresMetBewonersOphalen(adresID);

			IList<PersoonsAdres> teVerwijderen = (from pa in adr.PersoonsAdres
								  where personenIDs.Contains(pa.Persoon.ID)
								  select pa).ToList();

			// TODO: worker method gebruiken 

			foreach (PersoonsAdres pa in teVerwijderen)
			{
				pa.TeVerwijderen = true;
			}

			_adrMgr.Bewaren(adr);
		}

		public void VoorkeursAdresMaken(int persoonsAdresID, int gelieerdePersoonID)
		{
			GelieerdePersoon gp = _gpMgr.DetailsOphalen(gelieerdePersoonID);
			_gpMgr.VoorkeurInstellen(gp, persoonsAdresID);
			_gpMgr.BewarenMetPersoonsAdressen(gp);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<BewonersInfo> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			IList<Persoon> lijst = _pMgr.HuisGenotenOphalen(gelieerdePersoonID);
			return Mapper.Map<IList<Persoon>, IList<BewonersInfo>>(lijst);
		}

		/// <summary>
		/// Voegt een commvorm toe aan een gelieerde persoon
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
		/// <param name="commInfo">De communicatievorm die aan die persoon gekoppeld moet worden</param>
		public void CommunicatieVormToevoegen(int gelieerdePersoonID, CommunicatieInfo commInfo)
		{
			// TODO: Deze method moet nog aangepast worden.  De geijkte manier van werken is:
			// 1. Haal gelieerde persoon op
			// 2. Creer nieuwe communicatievorm
			// 3. Gebruik business om te koppelen
			// 4. Bewaar

			var communicatieVorm = Mapper.Map<CommunicatieInfo, CommunicatieVorm>(commInfo);
			communicatieVorm.CommunicatieType = _cvMgr.CommunicatieTypeOphalen(commInfo.CommunicatieTypeID);

			try
			{
				GelieerdePersoon gp = _gpMgr.OphalenMetCommVormen(gelieerdePersoonID);
				_cvMgr.AanpassingenDoorvoeren(gp, communicatieVorm);
				_gpMgr.BewarenMetCommVormen(gp);
			}
			catch (ValidatieException)
			{
				// TODO: specifiekere info bij in de exceptie.  Zie ticket #497.
				throw new FaultException<GapFault>(new GapFault() {FoutNummer = FoutNummers.ValidatieFout});
			}
		}

		// FIXME: de parameter 'gelieerdePersoonID' is overbodig; zie ticket #145.
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommunicatieVormVerwijderenVanPersoon(int gelieerdePersoonID, int commvormID)
		{
			var gp = _gpMgr.OphalenMetCommVormen(gelieerdePersoonID);
			var cv = (from commVorm in gp.Communicatie
									where commVorm.ID == commvormID
									select commVorm).FirstOrDefault();
			
			if (cv == null)
			{
				throw new ArgumentException(Resources.FouteCommunicatieVormVoorPersoonString);
			}
			_cvMgr.CommunicatieVormVerwijderen(cv, gp);	// persisteert
		}

		// TODO dit moet gecontroleerd worden!
		/// <summary>
		/// Persisteert de wijzigingen aan een bestaande communicatievorm
		/// </summary>
		/// <param name="v">De aan te passen communicatievorm</param>
		public void CommunicatieVormAanpassen(CommunicatieInfo v)
		{
			var communicatieVorm = Mapper.Map<CommunicatieInfo, CommunicatieVorm>(v);
			communicatieVorm.CommunicatieType = _cvMgr.CommunicatieTypeOphalen(v.CommunicatieTypeID);

			try{
				GelieerdePersoon gp = _cvMgr.OphalenMetGelieerdePersoon(v.ID);
				_cvMgr.AanpassingenDoorvoeren(gp, communicatieVorm);
				_gpMgr.BewarenMetCommVormen(gp);
			}
			catch (ValidatieException)
			{
				// TODO: specifiekere info bij in de exceptie.  Zie ticket #497.
				throw new FaultException<GapFault>(new GapFault() {FoutNummer = FoutNummers.ValidatieFout});
			}
		}

		/// <summary>
		/// Haalt detail van een communicatievorm op
		/// </summary>
		/// <param name="commvormID">ID van de communicatievorm waarover het gaat</param>
		/// <returns>De communicatievorm met de opgegeven ID</returns>
		public CommunicatieInfo CommunicatieVormOphalen(int commvormID)
		{
			return Mapper.Map<CommunicatieVorm, CommunicatieInfo>(_cvMgr.Ophalen(commvormID));
		}

		/// <summary>
		/// Haalt info over een bepaald communicatietype op, op basis van ID
		/// </summary>
		/// <param name="commTypeID">De ID van het communicatietype</param>
		/// <returns>Info over het gevraagde communicatietype</returns>
		public CommunicatieTypeInfo CommunicatieTypeOphalen(int commTypeID)
		{
			return Mapper.Map<CommunicatieType,CommunicatieTypeInfo>(
				_cvMgr.CommunicatieTypeOphalen(commTypeID));
		}

		public IEnumerable<CommunicatieTypeInfo> CommunicatieTypesOphalen()
		{
			return Mapper.Map<IEnumerable<CommunicatieType>, IEnumerable<CommunicatieTypeInfo>>(
				_cvMgr.CommunicatieTypesOphalen());
		}

		public int PersoonIDGet(int gelieerdePersoonID)
		{
			// TODO: Heel de gelieerde persoon + persoon ophalen voor enkel 1 ID is nog altijd overkill; zie issue #154
			return _gpMgr.Ophalen(gelieerdePersoonID).Persoon.ID;
		}

		#region categorieën
		/// <summary>
		/// Koppelt een lijst gebruikers aan een categorie
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van de te koppelen gebruikers</param>
		/// <param name="categorieIDs">ID's van de te koppelen categorieën</param>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CategorieKoppelen(IList<int> gelieerdepersonenIDs, IList<int> categorieIDs)
		{
			IList<GelieerdePersoon> gelpersonen = _gpMgr.Ophalen(gelieerdepersonenIDs);

			foreach (int catID in categorieIDs)
			{
				Categorie categorie = _catMgr.Ophalen(catID);

				// Koppelen
				_gpMgr.CategorieKoppelen(gelpersonen, categorie);

				// Bewaren
				_catMgr.BewarenMetPersonen(categorie);
			}
		}

		/// <summary>
		/// Koppelt een lijst gebruikers los van een categorie
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van los te koppelen gebruikers</param>
		/// <param name="categorieID">ID van de categorie</param>
		public void CategorieVerwijderen(IList<int> gelieerdepersonenIDs, int categorieID)
		{
			// Haal personen op met groep
			IList<GelieerdePersoon> gelieerdePersonen = _gpMgr.Ophalen(gelieerdepersonenIDs);

			// Haal categorie op met groep
			Categorie categorie = _catMgr.Ophalen(categorieID);

			// Ontkoppelen en persisteren (verwijderen persisteert altijd meteen)
			_gpMgr.CategorieLoskoppelen(gelieerdepersonenIDs, categorie);
		}
		#endregion categorieën

		#endregion
	}
}
