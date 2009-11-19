using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Permissions;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Orm;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Fouten.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using AutoMapper;

namespace Chiro.Gap.Services
{
	// NOTE: If you change the class name "GelieerdePersonenService" here, you must also update the reference to "GelieerdePersonenService" in Web.config.
	public class GelieerdePersonenService : IGelieerdePersonenService
	{
		#region Manager Injection

		private readonly GelieerdePersonenManager _gpMgr;
		private readonly PersonenManager _pMgr;
		private readonly AutorisatieManager _auMgr;
		private readonly AdressenManager _adrMgr;
		private readonly GroepenManager _grMgr;
		private readonly CommVormManager _cvMgr;

		public GelieerdePersonenService(GelieerdePersonenManager gpm, PersonenManager pm, AutorisatieManager aum
		    , AdressenManager adm, GroepenManager gm, CommVormManager cvm)
		{
			_gpMgr = gpm;
			_pMgr = pm;
			_auMgr = aum;
			_adrMgr = adm;
			_grMgr = gm;
			_cvMgr = cvm;
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

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public int PersoonAanmaken(GelieerdePersoon persoon, int groepID)
		{
			Groep g = _grMgr.Ophalen(groepID);
			g.GelieerdePersoon.Add(persoon);
			persoon.Groep = g;
			GelieerdePersoon gp = _gpMgr.Bewaren(persoon);
			return gp.ID;
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
		public Adres AdresMetBewonersOphalen(int adresID)
		{
			return _adrMgr.AdresMetBewonersOphalen(adresID);
		}

		// Verhuizen van een lijst personen
		// FIXME: Deze functie werkt op PersoonID's en niet op
		// GelieerdePersoonID's, en bijgevolg hoort dit eerder thuis
		// in een PersonenService ipv een GelieerdePersonenService.
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void PersonenVerhuizen(IList<int> personenIDs, Adres nieuwAdres, int oudAdresID)
		{
			// Zoek adres op in database, of maak een nieuw.
			// (als straat en gemeente gekend)
			try
			{
				nieuwAdres = _adrMgr.ZoekenOfMaken(nieuwAdres);
			}
			catch (AdresException ex)
			{
				throw new FaultException<AdresFault>(ex.Fault);
			}

			// Om foefelen te vermijden: we werken enkel op de gelieerde
			// personen waar de gebruiker GAV voor is.
			IList<int> mijnPersonen = _auMgr.EnkelMijnPersonen(personenIDs);

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
				_pMgr.Verhuizen(verhuizer, oudAdres, nieuwAdres);
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
		public void AdresToevoegenAanPersonen(List<int> personenIDs, Adres adres, AdresTypeEnum adresType)
		{
			// Dit gaat sterk lijken op verhuizen.

			// Adres opzoeken in database
			try
			{
				adres = _adrMgr.ZoekenOfMaken(adres);
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
		public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			return _pMgr.HuisGenotenOphalen(gelieerdePersoonID);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommVormToevoegenAanPersoon(int gelieerdepersonenID, CommunicatieVorm commvorm, int typeID)
		{
			_cvMgr.CommVormToevoegen(commvorm, gelieerdepersonenID, typeID);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommVormVerwijderenVanPersoon(int gelieerdepersonenID, int commvormID)
		{
			_cvMgr.CommVormVerwijderen(commvormID, gelieerdepersonenID);
		}

		///TODO dit moet gecontroleerd worden!
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AanpassenCommVorm(CommunicatieVorm v)
		{
			_cvMgr.Bewaren(v);
		}

		public CommunicatieVorm ophalenCommVorm(int commvormID)
		{
			return _cvMgr.Ophalen(commvormID);
		}

		public IEnumerable<CommunicatieType> ophalenCommunicatieTypes()
		{
			return _cvMgr.ophalenCommunicatieTypes();
		}

		#endregion

		#region categorieen
		public void CategorieKoppelen(IList<int> gelieerdepersonenIDs, int categorieID)
		{
			_gpMgr.CategorieKoppelen(gelieerdepersonenIDs, categorieID, true);
		}

		public void CategorieVerwijderenVanPersoon(IList<int> gelieerdepersonenIDs, int categorieID)
		{
			_gpMgr.CategorieKoppelen(gelieerdepersonenIDs, categorieID, false);
		}

		public IEnumerable<Categorie> ophalenCategorieen(int groepID)
		{
			return _gpMgr.ophalenCategorieen(groepID);
		}

		public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID)
		{
			throw new NotImplementedException();
		}

		#endregion categorieen
	}
}
