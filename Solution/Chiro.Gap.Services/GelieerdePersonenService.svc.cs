using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Permissions;

using AutoMapper;

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
			IAutorisatieManager aum)
		{
			_gpMgr = gpm;
			_pMgr = pm;
			_auMgr = aum;
			_adrMgr = adm;
			_grMgr = gm;
			_cvMgr = cvm;
			_catMgr = cm;
		}

		#endregion

		#region IGelieerdePersonenService Members

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<GelieerdePersoon> AllenOphalen(int groepID)
		{
			var result = _gpMgr.AllenOphalen(groepID);
			return result;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var result = _gpMgr.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalTotaal);
			return result;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonInfo> PaginaOphalenMetLidInfoVolgensCategorie(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var gelieerdePersonen = _gpMgr.PaginaOphalenMetLidInfoVolgensCategorie(categorieID, pagina, paginaGrootte, out aantalTotaal);
			return Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonInfo>>(gelieerdePersonen);
		}

		// *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
		// je aangemeld bent, op je lokale computer in de groep CgUsers zit.
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonInfo> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			var gelieerdePersonen = _gpMgr.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
			return Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonInfo>>(gelieerdePersonen);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public int GeforceerdAanmaken(GelieerdePersoon info, int groepID, bool forceer)
		{
			// Indien 'forceer' niet gezet is, moet een FaultException opgeworpen worden
			// als de  nieuwe persoon te hard lijkt op een bestaande Gelieerde Persoon.

			if (!forceer)
			{
				IList<GelieerdePersoon> bestaandePersonen =
					_gpMgr.ZoekGelijkaardig(info.Persoon, groepID);


				if (bestaandePersonen.Count > 0)
				{
					throw new FaultException<GelijkaardigePersoonFault>(new GelijkaardigePersoonFault {
						GelijkaardigePersonen = Mapper.Map<IList<GelieerdePersoon>, IList<PersoonInfo>>(bestaandePersonen)});

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

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte)
		{
			throw new NotImplementedException();
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID)
		{
			return _gpMgr.DetailsOphalen(gelieerdePersoonID);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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
		/// 
		/// (2) Deze functie werkt op PersoonID's en niet op
		/// GelieerdePersoonID's, en bijgevolg hoort dit eerder thuis
		/// in een PersonenService dan in een GelieerdePersonenService.
		/// </remarks>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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
					naarAdres.Straat, 
					naarAdres.HuisNr, 
					naarAdres.Bus, 
					naarAdres.Gemeente, 
					naarAdres.PostNr, 
					String.Empty);	// TODO: buitenlandse adressen (#238)
			}
			catch (AdresException ex)
			{
				throw new FaultException<AdresFault>(ex.Fault);
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

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AdresToevoegenAanPersonen(List<int> personenIDs, AdresInfo adr, AdresTypeEnum adresType)
		{
			// Dit gaat sterk lijken op verhuizen.

			// Adres opzoeken in database
			Adres adres = null;
			try
			{
				adres = _adrMgr.ZoekenOfMaken(adr.Straat, adr.HuisNr, adr.Bus, adr.Gemeente, adr.PostNr, String.Empty);
			}
			catch (AdresException ex)
			{
				throw new FaultException<AdresFault>(ex.Fault);
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

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AdresVerwijderenVanPersonen(List<int> personenIDs, int adresID)
		{
			// Adres ophalen, met bewoners voor GAV
			Adres adr = _adrMgr.AdresMetBewonersOphalen(adresID);

			IList<PersoonsAdres> teVerwijderen = (from pa in adr.PersoonsAdres
							      where personenIDs.Contains(pa.Persoon.ID)
							      select pa).ToList();

			foreach (PersoonsAdres pa in teVerwijderen)
			{
				pa.TeVerwijderen = true;
			}

			_adrMgr.Bewaren(adr);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<BewonersInfo> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			IList<Persoon> lijst = _pMgr.HuisGenotenOphalen(gelieerdePersoonID);
			return Mapper.Map<IList<Persoon>, IList<BewonersInfo>>(lijst);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommunicatieVormToevoegenAanPersoon(int gelieerdepersonenID, CommunicatieVorm commvorm, int typeID)
		{
			_cvMgr.CommunicatieVormToevoegen(commvorm, gelieerdepersonenID, typeID);
		}

		// FIXME: de parameter 'gelieerdePersoonID' is overbodig; zie ticket #145.
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void CommunicatieVormVerwijderenVanPersoon(int gelieerdePersoonID, int commvormID)
		{
            GelieerdePersoon gp = _gpMgr.OphalenMetCommVormen(gelieerdePersoonID);
            CommunicatieVorm cv = null;
            bool found = false;
            foreach (CommunicatieVorm c in gp.Communicatie)
            {
                if (c.ID == commvormID)
                {
                    cv = c;
                    found = true;
                }
            }
            if(!found)
            {
                throw new ArgumentException(Resources.FouteCommunicatieVormVoorPersoonString);
            }
			_cvMgr.CommunicatieVormVerwijderen(cv, gp);
            _gpMgr.BewarenMetCommVormen(gp);

		}

		///TODO dit moet gecontroleerd worden!
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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

		#region categorieen
		/// <summary>
		/// Koppelt een lijst gebruikers aan een categorie
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van de te koppelen gebruikers</param>
		/// <param name="categorieID">ID van de te koppelen categorie</param>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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

			// Ontkoppelen
			_gpMgr.CategorieLoskoppelen(gelieerdepersonenIDs, categorie);

			// Bewaren
			_catMgr.BewarenMetPersonen(categorie);
		}

		public IEnumerable<Categorie> CategorieenOphalen(int groepID)
		{
			return _gpMgr.CategorieenOphalen(groepID);
		}

		public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID)
		{
			throw new NotImplementedException();
		}

		#endregion categorieen

        #endregion
    }
}
