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


namespace Chiro.Gap.Services
{
    // OPM: als je de naam van de class "GroepenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.
	public class GroepenService : IGroepenService
	{
		#region Manager Injection

		private readonly GroepenManager _groepenMgr;
		private readonly WerkJaarManager _werkjaarMgr;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly GelieerdePersonenManager _gelieerdePersonenMgr = Factory.Maak<GelieerdePersonenManager>();

		public GroepenService(GroepenManager gm, WerkJaarManager wm, GelieerdePersonenManager gpm, IAutorisatieManager am)
		{
			_groepenMgr = gm;
			_werkjaarMgr = wm;
			_autorisatieMgr = am;
			_gelieerdePersonenMgr = gpm;
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
			return _werkjaarMgr.RecentsteGroepsWerkJaarIDGet(groepID);
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
			return _werkjaarMgr.HuidigWerkJaarGet(groepID);
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

        public Groep OphalenMetAfdelingen(int groepID)
        {
            var result = _groepenMgr.Ophalen(groepID, e => e.Afdeling);
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
        public void AfdelingAanmaken(int groepID, string naam, string afkorting)
		{
			Groep g = _groepenMgr.Ophalen(groepID);
			_groepenMgr.AfdelingToevoegen(g, naam, afkorting);
			_groepenMgr.Bewaren(g, e => e.Afdeling);
		}

        public void AfdelingsJaarAanmaken(int groepID, int afdelingsID, int offiafdelingsID, int geboortejaarbegin, int geboortejaareind)
		{
            Groep g = _groepenMgr.Ophalen(groepID, e => e.Afdeling);
            Afdeling afd = null;
            foreach (Afdeling a in g.Afdeling)
            {
                if (a.ID == afdelingsID)
                {
                    afd = a;
                }
            }

            OfficieleAfdeling offafd = null;
            foreach (OfficieleAfdeling a in _groepenMgr.OfficieleAfdelingenOphalen())
            {
                if (a.ID == offiafdelingsID)
                {
                    offafd = a;
                }
            }

            if (afd == null || offafd == null)
            {
                throw new FoutieveGroepException(String.Format(Resources.FouteAfdelingVoorGroepString, g.Naam));
            }

            GroepsWerkJaar huidigWerkJaar = _groepenMgr.RecentsteGroepsWerkJaarGet(g.ID);

			AfdelingsJaar afdjaar = _groepenMgr.AfdelingsJaarMaken(afd, offafd, huidigWerkJaar, geboortejaarbegin, geboortejaareind);

            Factory.Maak<GroepenManager>();

            throw new NotImplementedException();

            /*_groepenMgr.Bewaren(afdjaar, aj => aj.OfficieleAfdeling.WithoutUpdate(),
                                aj => aj.Afdeling.WithoutUpdate(),
                                aj => aj.GroepsWerkJaar.WithoutUpdate());*/
		}

		#endregion

		#region Categorieen

		//TODO een efficiente manier vinden om een bepaalde eigenschap toe te voegen aan een al geladen element.
		//of anders in de workers methoden aanbieden om lambda expressies mee te geven: dan eerst bepalen wat allemaal nodig is, dan 1 keer laden
		//en dan zijn we terug bij het idee om in het object bij te houden wat hij allemaal heeft geladen

        // Bedenking van Johan: Lambda-expressies lijken me niet wenselijk in de businesslaag, omdat je
		// niet kan controleren of de gebruiker het recht wel heeft de zaken gespecifieerd in de expressie op
		// te vragen.
		public Groep OphalenMetCategorieen(int groepID)
		{
			var result = _groepenMgr.Ophalen(groepID, e => e.Categorie);
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
            foreach(Categorie cc in g.Categorie)
            {
                if(cc.Naam.Equals(naam))
                {
                    c = cc;
                }
            }
            //TODO de lambda expressies hieruit halen en er terug methoden van maken (die security kunnen checken)
            return c.ID;
		}

		public void CategorieVerwijderen(int categorieID, int groepID)
		{
            Groep g = OphalenMetCategorieen(groepID);
            Categorie c = null;
            foreach(Categorie cc in g.Categorie)
            {
                if (cc.ID.Equals(categorieID))
                {
                    c = cc;
                }
            }
            if (c == null)
            {
                throw new ArgumentException(Resources.FouteCategorieVoorGroepString);
            }
            _groepenMgr.CategorieVerwijderen(g, c);
            _groepenMgr.Bewaren(g, e => e.Categorie);
		}

		public void CategorieAanpassen(int categorieID, string nieuwenaam)
		{
            /*Groep g = OphalenMetCategorieen(groepID);
            Categorie c = null;*/
			throw new NotImplementedException();
		}

		#endregion categorieen
	}
}
