using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Diagnostics;
using System.Text;

using AutoMapper;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Fouten.Exceptions;
using System.Security.Permissions;


namespace Chiro.Gap.Services
{
	// OPM: als je de naam van de class "GroepenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.
	public class GroepenService : IGroepenService
	{
		#region Manager Injection

		private readonly GroepenManager _groepenMgr;
		private readonly AfdelingsJaarManager _afdelingsJaarMgr;
		private readonly AdressenManager _adresMgr;
		private readonly GroepsWerkJaarManager _werkJaarMgr;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly GelieerdePersonenManager _gelieerdePersonenMgr;
		private readonly CategorieenManager _categorieenMgr;

		public GroepenService(
			GroepenManager gm, 
			AfdelingsJaarManager ajm, 
			GroepsWerkJaarManager wm, 
			GelieerdePersonenManager gpm, 
			AdressenManager adresMgr,
			CategorieenManager cm,
			IAutorisatieManager am)
		{
			_groepenMgr = gm;
			_afdelingsJaarMgr = ajm;
			_werkJaarMgr = wm;
			_autorisatieMgr = am;
			_gelieerdePersonenMgr = gpm;
			_adresMgr = adresMgr;
			_categorieenMgr = cm;
		}

		#endregion

		#region algemene members

		public GroepInfo InfoOphalen(int GroepId)
		{
			var gr = Ophalen(GroepId);
			return Mapper.Map<Groep, GroepInfo>(gr);
		}

		public Groep Bewaren(Groep g)
		{
			try
			{
				return _groepenMgr.Bewaren(g);
			}
			catch (Exception e)
			{
				// TODO: fatsoenlijke exception handling
				throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
			}
		}

		public int RecentsteGroepsWerkJaarIDGet(int groepID)
		{
			return _werkJaarMgr.RecentsteGroepsWerkJaarIDGet(groepID);
		}

		/// <summary>
		/// TODO: Documentatie bijwerken, en naam veranderen in HuidgWerkJaarOphalen
		/// (of iets gelijkaardig; zie coding standaard). 
		/// Deze documentatie is alleszins onvolledig, want ze gaat ervan uit dat groepen
		/// nooit ophouden te bestaan.  Wat moet deze functie teruggeven als de groep
		/// geen werking meer heeft?
		/// 
		/// Geeft het huidige werkjaar van de gegeven groep terug. Dit is gegarandeerd het huidige jaartal wanneer de
		/// huidige dag tussen de deadline voor het nieuwe werkjaar en de begindatum van het volgende werkjaar ligt.
		/// In de tussenperiode hangt het ervan af of de groep de overgang al heeft gemaakt, en dit is te zien aan 
		/// het laatst gemaakte groepswerkjaar
		/// </summary>
		/// <param name="groepID"></param>
		/// <returns></returns>
		public int HuidigWerkJaarGet(int groepID)
		{
			return _werkJaarMgr.HuidigWerkJaarGet(groepID);
		}

		#endregion

		#region ophalen
		public Groep Ophalen(int groepID)
		{
			var result = _groepenMgr.Ophalen(groepID);
			return result;
		}

		public Groep OphalenMetAdressen(int groepID)
		{
			throw new NotImplementedException();
		}

		public Groep OphalenMetFuncties(int groepID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Haalt een groep op, met daaraan gekoppeld al zijn afdelingen
		/// </summary>
		/// <param name="groepID">ID van de gevraagde groep</param>
		/// <returns>De gevraagde groep, met daaraan gekoppeld al zijn afdelingen</returns>
		public Groep OphalenMetAfdelingen(int groepID)
		{
			var result = _groepenMgr.OphalenMetAfdelingen(groepID);
			return result;
		}

		public Groep OphalenMetVrijeVelden(int groepID)
		{
			throw new NotImplementedException();
		}

		/*        public Groep OphalenMetCategorieen(int groepID)
			{
			    var result = gm.Ophalen(groepID, e => e.Categorie);
			    return result;
			}*/

		public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
		{
			return _groepenMgr.OfficieleAfdelingenOphalen();
		}

		public IEnumerable<GroepInfo> MijnGroepenOphalen()
		{
			var result = _autorisatieMgr.GekoppeldeGroepenGet();
			return Mapper.Map<IEnumerable<Groep>, IEnumerable<GroepInfo>>(result);
		}

		#endregion

		#region afdelingen

		// Bedoeling van het afdelingsgedeelte:
		// er zijn een aantal officiële afdelingen, die een range van leeftijden hebben. Blijven dat altijd dezelfde?
		// Elke Chirogroep heeft elk werkjaar haar eigen afdelingen, die ook een range van leeftijden hebben.
		// 
		// Elke afdeling moet overeenkomen met een officiële afdeling.
		// Er is niet gespecifieerd of het mogelijk is om een eerste-jaar-rakkers en een tweede-jaar-rakkers te hebben
		// 
		// Omdat bovenstaande niet echt duidelijk is en misschien niet altijd voldoende:
		// waarom moet er een mapping zijn met een officiële afdeling? Als dit echt moet, dan is het bovenstaande niet duidelijk,
		// en stel ik het onderstaande voor
		// 
		// Elke afdeling heeft een naam, een afkorting en een boolean NOGINGEBRUIK?
		// Elk afdelingsjaar heeft een afdeling en een interval van leeftijden.
		// Voor elke leeftijd is er een mapping met een officiële afdeling
		// elke leeftijd kan maar op 1 officiële afdeling gemapt worden
		// 
		// Voorbeelden:
		// "de kleintjes" = {minis, speelclub}
		// "de 5de jaars" = {eerste jaar rakkers}
		// "rakwi's" = {tweede jaar speelclub, rakkers}

		/// <summary>
		/// Maakt een nieuwe afdeling voor een gegeven groep
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="naam">naam van de afdeling</param>
		/// <param name="afkorting">afkorting van de afdeling (voor lijsten, overzichten,...)</param>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AfdelingAanmaken(int groepID, string naam, string afkorting)
		{
			Groep g = _groepenMgr.Ophalen(groepID);
			_groepenMgr.AfdelingToevoegen(g, naam, afkorting);
			_groepenMgr.Bewaren(g, e => e.Afdeling);
		}


		/// <summary>
		/// Bewerkt een AfdelingsJaar: 
		/// andere OfficieleAfdeling en/of andere leeftijden
		/// </summary>
		/// <param name="afdID">AfdelingsJaarID</param>
		/// <param name="offafdID">OfficieleAfdelingsID</param>
		/// <param name="geboortVan">GeboorteJaarVan</param>
		/// <param name="geboortTot">GeboorteJaarTot</param>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AfdelingsJaarBewarenMetWijzigingen(int afdID, int offafdID, int geboorteVan, int geboorteTot)
		{
			AfdelingsJaar aj = _afdelingsJaarMgr.Ophalen(afdID);
			OfficieleAfdeling oa = _groepenMgr.OfficieleAfdelingenOphalen().Where(a => a.ID == offafdID).FirstOrDefault<OfficieleAfdeling>();
			aj.OfficieleAfdeling = oa;
			aj.GeboorteJaarVan = geboorteVan;
			aj.GeboorteJaarTot = geboorteTot;
			_afdelingsJaarMgr.Bewaren(aj);
		}


		/// <summary>
		/// Verwijdert een afdelingsjaar
		/// en controleert of er geen leden in zitten.
		/// </summary>
		/// <param name="afdelingsJaarID"></param>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AfdelingsJaarVerwijderen(int afdelingsJaarID)
		{
			_afdelingsJaarMgr.Verwijderen(afdelingsJaarID);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public AfdelingsJaar AfdelingsJaarOphalen(int afdelingsJaarID)
		{
			return _afdelingsJaarMgr.Ophalen(afdelingsJaarID);
		}


		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AfdelingsJaarAanmaken(int groepID, int afdelingsID, int offiafdelingsID, int geboortejaarbegin, int geboortejaareind)
		{
			Groep g = _groepenMgr.OphalenMetAfdelingen(groepID);
			Afdeling afd = g.Afdeling.Where(a => a.ID == afdelingsID).FirstOrDefault<Afdeling>();
			OfficieleAfdeling offafd = _groepenMgr.OfficieleAfdelingenOphalen().Where(o => o.ID == offiafdelingsID).FirstOrDefault<OfficieleAfdeling>();

			if (afd == null || offafd == null)
			{
				throw new FoutieveGroepException(String.Format(Resources.FouteAfdelingVoorGroepString, g.Naam));
			}

			GroepsWerkJaar huidigWerkJaar = _werkJaarMgr.RecentsteGroepsWerkJaarGet(g.ID);

			AfdelingsJaar afdjaar = _groepenMgr.AfdelingsJaarMaken(afd, offafd, huidigWerkJaar, geboortejaarbegin, geboortejaareind);

			_afdelingsJaarMgr.Bewaren(afdjaar);

		}


		public IList<AfdelingInfo> AfdelingenOphalen(int groepswerkjaarID)
		{
			var groepswerkjaar = _groepenMgr.GroepsWerkJaarOphalenMetAfdelingInfo(groepswerkjaarID);
			return Mapper.Map<IList<AfdelingsJaar>, IList<AfdelingInfo>>(groepswerkjaar.AfdelingsJaar.OrderBy(e => e.GeboorteJaarVan).ToList<AfdelingsJaar>());
		}

		#endregion

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<GroepsWerkJaar> WerkJarenOphalen(int groepsID)
		{
			return (from gwj in _groepenMgr.OphalenMetGroepsWerkJaren(groepsID).GroepsWerkJaar
				orderby gwj.WerkJaar descending
				select gwj
			       ).ToList();
		}

		#region Categorieen

		/// <summary>
		/// Haalt een groep op, met daaraan gekoppeld al zijn categorieen
		/// </summary>
		/// <param name="groepID">ID van op te halen groep</param>
		/// <returns>Groep met ID <paramref name="groepID"/>, met daaraan gekoppeld al zijn 
		/// categorieen.</returns>
		public Groep OphalenMetCategorieen(int groepID)
		{
			var result = _groepenMgr.OphalenMetCategorieen(groepID);
			return result;
		}

		/// <summary>
		/// Maakt een nieuwe categorie voor de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor nieuwe categorie wordt gemaakt</param>
		/// <param name="naam">naam voor de nieuwe categorie</param>
		/// <param name="code">code voor de nieuwe categorie</param>
		public int CategorieToevoegen(int groepID, string naam, string code)
		{
			Groep g = OphalenMetCategorieen(groepID);
			Categorie c = _groepenMgr.CategorieToevoegen(g, naam, code);
			g = _groepenMgr.Bewaren(g, e => e.Categorie);
			//TODO kan dit niet mooier om de ID op te vragen?
			foreach (Categorie cc in g.Categorie)
			{
				if (cc.Naam.Equals(naam))
				{
					c = cc;
				}
			}
			//TODO de lambda expressies hieruit halen en er terug methoden van maken (die security kunnen checken)
			return c.ID;
		}

		/// <summary>
		/// Verwijdert de categorie met gegeven <paramref name="categorieID"/>
		/// </summary>
		/// <param name="categorieID">ID van de te verwijderen categorie</param>
		public void CategorieVerwijderen(int categorieID)
		{
			Categorie c = _categorieenMgr.Ophalen(categorieID);
			c.TeVerwijderen = true;
			_categorieenMgr.Bewaren(c);
		}

		public void CategorieAanpassen(int categorieID, string nieuwenaam)
		{
			/*Groep g = OphalenMetCategorieen(groepID);
			Categorie c = null;*/
			throw new NotImplementedException();
		}


		/// <summary>
		/// Zoekt de categorieID op van de categorie bepaald door de gegeven 
		/// <paramref name="groepID"/> en <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de gezochte categorie gekoppeld is</param>
		/// <param name="code">code van de te zoeken categorie</param>
		/// <returns>Het categorieID als de categorie gevonden is, anders 0.</returns>
		public int CategorieIDOphalen(int groepID, string code)
		{
			Categorie cat = _categorieenMgr.Ophalen(groepID, code);
			return (cat == null) ? 0 : cat.ID;
		}

		#endregion categorieen

		#region adressen
		/// <summary>
		/// Maakt een lijst met alle deelgemeentes uit de database; nuttig voor autocompletion
		/// in de ui.
		/// </summary>
		/// <returns>Lijst met alle beschikbare deelgemeentes</returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<GemeenteInfo> GemeentesOphalen()
		{
			return Mapper.Map<IList<Subgemeente>, IList<GemeenteInfo>>(_adresMgr.GemeentesOphalen());
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<StraatInfo> StratenOphalen(String straatBegin, int postNr)
		{
			return Mapper.Map<IList<Straat>, IList<StraatInfo>>(_adresMgr.StratenOphalen(straatBegin, postNr));
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		/// <remarks>Ik had deze functie ook graag StratenOphalen genoemd, maar je mag geen 2 
		/// WCF-functies met dezelfde naam in 1 service hebben.  Spijtig.</remarks>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(String straatBegin, IEnumerable<int> postNrs)
		{
			return Mapper.Map<IList<Straat>, IList<StraatInfo>>(_adresMgr.StratenOphalen(straatBegin, postNrs));
		}
		#endregion
	}
}
