// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Permissions;

using AutoMapper;

using Chiro.Gap.Fouten;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Orm;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Workers;

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
		private readonly CommVormManager _cvMgr;
		private readonly CategorieenManager _catMgr;
		private readonly IAutorisatieManager _auMgr;

		public GelieerdePersonenService(
			GelieerdePersonenManager gpm,
			PersonenManager pm,
			AdressenManager adm,
			GroepenManager gm,
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
			_cvMgr = cvm;
			_catMgr = cm;
			_lidMgr = lm;
		}

		#endregion

		#region IGelieerdePersonenService Members

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var result = _gpMgr.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalTotaal);
			return result;
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonInfo> PaginaOphalenUitCategorieMetLidInfo(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var gelieerdePersonen = _gpMgr.PaginaOphalenMetLidInfoVolgensCategorie(categorieID, pagina, paginaGrootte, out aantalTotaal);
			return Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonInfo>>(gelieerdePersonen);
		}

		// *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
		// je aangemeld bent, op je lokale computer in de groep CgUsers zit.
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonInfo> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var gelieerdePersonen = _gpMgr.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
			var result = Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonInfo>>(gelieerdePersonen);
			return result;
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public int PersoonBewaren(GelieerdePersoon persoon)
		{
			_gpMgr.Bewaren(persoon);
			return persoon.ID;
		}

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren: Chiroleeftijd, en
		/// de velden van <c>info.Persoon</c></param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID van de bewaarde persoon</returns>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.</remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public int Aanmaken(GelieerdePersoon info, int groepID)
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
		/// <returns>ID van de bewaarde persoon</returns>
		/// <param name="forceer">Als deze <c>true</c> is, wordt de nieuwe persoon sowieso gemaakt, ook
		/// al lijkt hij op een bestaande gelieerde persoon.  Is <paramref>force</paramref>
		/// <c>false</c>, dan wordt er een exceptie opgegooid als de persoon te hard lijkt op een
		/// bestaande.</param>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.</remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public int GeforceerdAanmaken(GelieerdePersoon info, int groepID, bool forceer)
		{
			// Indien 'forceer' niet gezet is, moet een FaultException opgeworpen worden
			// als de  nieuwe persoon te hard lijkt op een bestaande Gelieerde Persoon.
			
			// FIXME: Deze businesslogica moet in de workers gebeuren, waar dan een exception opgeworpen
			// kan worden, die we hier mappen op een faultcontract.

			if (!forceer)
			{
				IList<GelieerdePersoon> bestaandePersonen =
					_gpMgr.ZoekGelijkaardig(info.Persoon, groepID);

				if (bestaandePersonen.Count > 0)
				{
					var fault = new BlokkerendeObjectenFault<BestaatAlFoutCode, PersoonInfo>{
						FoutCode = BestaatAlFoutCode.PersoonBestaatAl,
						Objecten = Mapper.Map<IList<GelieerdePersoon>, IList<PersoonInfo>>(bestaandePersonen) };

					throw new FaultException<BlokkerendeObjectenFault<BestaatAlFoutCode, PersoonInfo>>(fault);

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

			GelieerdePersoon gelieerd = _gpMgr.Koppelen(info.Persoon, g, info.ChiroLeefTijd);
			gelieerd = _gpMgr.Bewaren(gelieerd);
			return gelieerd.ID;
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte)
		{
			throw new NotImplementedException();
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
		{
			return _gpMgr.DetailsOphalen(gelieerdePersoonID);
		}

		public PersoonLidInfo AlleDetailsOphalen(int gelieerdePersoonID, int groepID)
		{
			GelieerdePersoon gp = _gpMgr.DetailsOphalen(gelieerdePersoonID);
			PersoonLidInfo pl = Mapper.Map<GelieerdePersoon, PersoonLidInfo>(gp);
			GroepsWerkJaar gwj = _grMgr.RecentsteGroepsWerkJaarGet(groepID);
			Lid l = _lidMgr.OphalenViaPersoon(gp.ID, gwj.ID);
			if (l != null)
			{
				LidInfo ff = Mapper.Map<Lid, LidInfo>(l);
				pl.LidInfo = ff;
			}
			
			return pl;
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public AdresInfo AdresMetBewonersOphalen(int adresID)
		{
			return Mapper.Map<Adres, AdresInfo>(_adrMgr.AdresMetBewonersOphalen(adresID));
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
		/// <param name="adresType">Adrestype dat alle aangepaste PersoonsAdressen zullen krijgen</param>
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
		public void PersonenVerhuizen(
			IList<int> persoonIDs,
			AdresInfo naarAdres,
			int oudAdresID,
			AdresTypeEnum adresType)
		{
			// Zoek adres op in database, of maak een nieuw.
			// (als straat en gemeente gekend)

			Adres nieuwAdres = null;
			try
			{
				nieuwAdres = _adrMgr.ZoekenOfMaken(
					naarAdres.StraatNaamNaam,
					naarAdres.HuisNr,
					naarAdres.Bus,
					naarAdres.WoonPlaatsID,
					naarAdres.PostNr,
					String.Empty);	// TODO: buitenlandse adressen (#238)
			}
			catch (OngeldigObjectException<AdresFoutCode> ex)
			{
				var fault = Mapper.Map<OngeldigObjectException<AdresFoutCode>,
					OngeldigObjectFault<AdresFoutCode>>(ex);

				throw new FaultException<OngeldigObjectFault<AdresFoutCode>>(fault);
			}

			// Om foefelen te vermijden: we werken enkel op de gelieerde
			// personen waar de gebruiker GAV voor is.
			IList<int> mijnPersonen = _auMgr.EnkelMijnPersonen(persoonIDs);

			// Haal bronadres en alle bewoners op
			Adres oudAdres = _adrMgr.AdresMetBewonersOphalen(oudAdresID);

			// Selecteer enkel bewoners uit mijnGelieerdePersonen
			IList<Persoon> teVerhuizen =
				(from PersoonsAdres pa
				in oudAdres.PersoonsAdres
				 where mijnPersonen.Contains(pa.Persoon.ID)
				 select pa.Persoon).ToList();

			// Bovenstaande query meteen evalueren en resultaten in een lijst.
			// Als ik dat niet doe, dan verandert het 'in' gedeelte van
			// de foreach tijdens de loop, en daar kan .net niet mee
			// lachen.
			foreach (Persoon verhuizer in teVerhuizen)
			{
				_pMgr.Verhuizen(verhuizer, oudAdres, nieuwAdres, adresType);
			}

			// Persisteren
			_adrMgr.Bewaren(nieuwAdres);

			// Bij een verhuis, blijven de PersoonsAdresobjecten dezelfde,
			// maar worden ze aan een ander adres gekoppeld.  Een post
			// van het nieuwe adres (met persoonsadressen) koppelt bijgevolg
			// de persoonsobjecten los van het oude adres.
			// Bijgevolg moet het oudeAdres niet gepersisteerd worden.
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AdresToevoegenAanPersonen(List<int> personenIDs, AdresInfo adr, AdresTypeEnum adresType)
		{
			// Dit gaat sterk lijken op verhuizen.

			// Adres opzoeken in database
			Adres adres = null;
			try
			{
				adres = _adrMgr.ZoekenOfMaken(adr.StraatNaamNaam, adr.HuisNr, adr.Bus, adr.WoonPlaatsID, adr.PostNr, String.Empty);
			}
			catch (OngeldigObjectException<AdresFoutCode> ex)
			{
				var fault = Mapper.Map<OngeldigObjectException<AdresFoutCode>,
					OngeldigObjectFault<AdresFoutCode>>(ex);

				throw new FaultException<OngeldigObjectFault<AdresFoutCode>>(fault);
			}

			// Personen ophalen
			IList<Persoon> personenLijst = _pMgr.LijstOphalen(personenIDs);

			// Adres koppelen
			foreach (Persoon p in personenLijst)
			{
				_pMgr.AdresToevoegen(p, adres, adresType);
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

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<BewonersInfo> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			IList<Persoon> lijst = _pMgr.HuisGenotenOphalen(gelieerdePersoonID);
			return Mapper.Map<IList<Persoon>, IList<BewonersInfo>>(lijst);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommunicatieVormToevoegenAanPersoon(int gelieerdepersonenID, CommunicatieVorm commvorm, int typeID)
		{
			_cvMgr.CommunicatieVormToevoegen(commvorm, gelieerdepersonenID, typeID);
		}

		// FIXME: de parameter 'gelieerdePersoonID' is overbodig; zie ticket #145.
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommunicatieVormVerwijderenVanPersoon(int gelieerdePersoonID, int commvormID)
		{
			GelieerdePersoon gp = _gpMgr.OphalenMetCommVormen(gelieerdePersoonID);
			CommunicatieVorm cv = (from commVorm in gp.Communicatie
									where commVorm.ID == commvormID
									select commVorm).FirstOrDefault();
			
			if (cv == null)
			{
				throw new ArgumentException(Resources.FouteCommunicatieVormVoorPersoonString);
			}
			_cvMgr.CommunicatieVormVerwijderen(cv, gp);	// persisteert
		}

		// TODO dit moet gecontroleerd worden!
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommunicatieVormAanpassen(CommunicatieVorm v)
		{
			_cvMgr.Bewaren(v);
		}

		public CommunicatieVorm CommunicatieVormOphalen(int commvormID)
		{
			return _cvMgr.Ophalen(commvormID);
		}

		public CommunicatieType CommunicatieTypeOphalen(int commTypeID)
		{
			return _cvMgr.CommunicatieTypeOphalen(commTypeID);
		}

		public IEnumerable<CommunicatieType> CommunicatieTypesOphalen()
		{
			return _cvMgr.CommunicatieTypesOphalen();
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

		public IEnumerable<Categorie> CategorieenOphalen(int groepID)
		{
			return _gpMgr.CategorieenOphalen(groepID);
		}

		public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID)
		{
			throw new NotImplementedException();
		}

		#endregion categorieën

		#endregion
	}
}
