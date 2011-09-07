// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp
{
    /// <summary>
    /// Klasse om veel gebruikte zaken mee op te vragen, die dan ook gecachet kunnen worden.
    /// Enkel als iets niet gecachet is, wordt de servicehelper gebruikt.
    /// </summary>
    public class VeelGebruikt : IVeelGebruikt
    {
        #region cache keys

        private const string GROEPSWERKJAARCACHEKEY = "gwj{0}";
        private const string UNIEKEGROEPCACHEKEY = "uniekegr{0}";
        private const string FUNCTIEPROBLEMENCACHEKEY = "funprob{0}";
        private const string FUNCTIEPROBLEMENAANTALCACHEKEY = "aantalfunprob{0}";
        private const string WOONPLAATSENCACHEKEY = "woonpl";
        private const string LANDENCACHEKEY = "landen";
        private const string ISLIVECACHEKEY = "live";
        private const string LEDENPROBLEMENCACHEKEY = "lidprob{0}";
        private const string LEDENPROBLEMENAANTALCACHEKEY = "aantallidprob{0}";
        private const string BIVAKSTATUSCACHEKEY = "bivstat{0}";
        private const string BIVAKSTATUSAANTALCACHEKEY = "aantalbivstat{0}";
        #endregion

        private readonly Cache _cache = HttpRuntime.Cache;	// misschien hier ook DI voor gebruiken?

        // TODO (#1038): deze klasse kan gemakkelijk opgesplitst worden in partial classes per 'thema' 

        #region Groepswerkjaar

        /// <summary>
        /// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/>
        /// uit de cache.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan groepswerkjaarcache te resetten</param>
        public void GroepsWerkJaarResetten(int groepID)
        {
            _cache.Remove(String.Format(GROEPSWERKJAARCACHEKEY, groepID));
        }

        /// <summary>
        /// Haalt het recentste groepswerkjaar van de groep met gegeven <paramref name="groepID"/>
        /// op uit de cache, of - indien niet beschikbaar - van backend.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan we het groepswerkjaar willen weten.</param>
        /// <returns>Details over het gevraagde groepswerkjaar.</returns>
        public GroepsWerkJaarDetail GroepsWerkJaarOphalen(int groepID)
        {
            var gwjDetail = (GroepsWerkJaarDetail)_cache.Get(String.Format(GROEPSWERKJAARCACHEKEY, groepID));
            if (gwjDetail == null)
            {
                gwjDetail = ServiceHelper.CallService<IGroepenService, GroepsWerkJaarDetail>(g => g.RecentsteGroepsWerkJaarOphalen(groepID));

                // Als de gebruiker geen GAV is, krijgen we hier een FaultException. Die wordt niet opgevangen,
                // maar als je in web.config <customErrors="On"> instelt (ipv "Off" of "RemoteOnly"), dan
                // word je automatisch doorverwezen naar de foutpagina, waar de exception 'afgehandeld' wordt.

                // OPM: kan gi nog null zijn? 
                _cache.Add(
                    String.Format(GROEPSWERKJAARCACHEKEY, groepID),
                    gwjDetail,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(2, 0, 0),
                    CacheItemPriority.Normal,
                    null);
            }

            return gwjDetail;
        }

    	#endregion

        #region Problemen

		/// <summary>
		/// Reset alle problemen omdat de jaarovergang wordt uitgevoerd
		/// </summary>
		/// <param name="groepID">ID van groep met te verwijderen problemen</param>
		public void JaarOvergangReset(int groepID)
		{
			FunctieProblemenResetten(groepID);
			BivakStatusResetten(groepID);
			GroepsWerkJaarResetten(groepID);
			LedenProblemenResetten(groepID);
		}

        /// <summary>
        /// Verwijdert de gecachete functieproblemen van een bepaalde groep
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de problemencache leeg te maken</param>
        public void FunctieProblemenResetten(int groepID)
        {
            // Problemen met functies moeten opgenieuw opgehaald worden na deze operatie. BaseController 
            // gaat na of dat nodig is door naar de telling te kijken, maar ook de gecachete problemen 
            // moeten verwijderd worden. Als het nieuwe aantal problemen even groot is als het vorige, 
            // worden ze anders niet vervangen.
            _cache.Remove(String.Format(FUNCTIEPROBLEMENCACHEKEY, groepID));
            _cache.Remove(String.Format(FUNCTIEPROBLEMENAANTALCACHEKEY, groepID));
        }

        /// <summary>
        /// Haalt de problemen van de groep ivm functies op.  Uit de cache, of als die gegevens niet
        /// aanwezig zijn, via de service.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de functieproblemen opgehaald moeten worden</param>
        /// <returns>De rij functieproblemen</returns>
        public IEnumerable<FunctieProbleemInfo> FunctieProblemenOphalen(int groepID)
        {
            // We werken met twee cache-items om te vermijden dat we te veel naar de databank
            // moeten. Het is ook nodig omdat we geen null kunnen cachen. Als er geen problemen 
            // zijn, moeten we dat dus op een andere manier opslaan: daarvoor dient de teller.
            // Als de tellingcache leeg is (leeggemaakt na toekenning of 'ontslag'), of als de 
            // telling niet overeenkomt met de problemencache, dan halen we de problemen opnieuw 
            // op. Elke operatie waarbij functies toegekend of afgenomen worden, moet de 
            // tellingcache en de problemencache clearen.
            var telling = (int?)_cache.Get(String.Format(FUNCTIEPROBLEMENAANTALCACHEKEY, groepID));
            var functieProblemen = (IEnumerable<FunctieProbleemInfo>)_cache.Get(String.Format(FUNCTIEPROBLEMENCACHEKEY, groepID));

            if (telling == null || telling != (functieProblemen == null ? 0 : functieProblemen.Count()))
            {
                // Als telling niet gezet is, of telling verschilt van het aantal problemen, zit er niets
                // bruikbaars in de cache.  De extra check op functieProblemen == null heb ik toegevoegd
                // om gemakkelijker te kunnen unittesten.  FunctieProblemen == null wordt ook beschouwd
                // als 'er zijn geen problemen'.

                // problemen ophalen
                functieProblemen =
                    ServiceHelper.CallService<IGroepenService, IEnumerable<FunctieProbleemInfo>>(
                        svc => svc.FunctiesControleren(groepID));

                // eventueel de problemen cachen
                if (functieProblemen == null)
                {
                    telling = 0;
                    _cache.Remove(String.Format(FUNCTIEPROBLEMENCACHEKEY, groepID));
                }
                else
                {
                    telling = functieProblemen.Count();

                    _cache.Add(String.Format(FUNCTIEPROBLEMENCACHEKEY, groepID),
                          functieProblemen,
                          null,
                          Cache.NoAbsoluteExpiration,
                          new TimeSpan(2, 0, 0),
                          CacheItemPriority.NotRemovable,
                          null);
                }

                // aantal problemen in cache steken
                _cache.Add(String.Format(FUNCTIEPROBLEMENAANTALCACHEKEY, groepID),
                        telling,
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(2, 0, 0),
                        CacheItemPriority.NotRemovable,
                        null);
            }

            return functieProblemen;
        }

        /// <summary>
        /// Haalt een lijstje op met informatie over ontbrekende gegevens bij leden.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan we de problemen opvragen</param>
        /// <returns>Een rij LedenProbleemInfo met info over de ontbrekende gegevens</returns>
        public IEnumerable<LedenProbleemInfo> LedenProblemenOphalen(int groepID)
        {
            int? telling = (int?)_cache.Get(String.Format(LEDENPROBLEMENAANTALCACHEKEY, groepID));
            var ledenProblemen = (IEnumerable<LedenProbleemInfo>)_cache.Get(
                String.Format(LEDENPROBLEMENCACHEKEY, groepID));

            if (telling == null || telling != (ledenProblemen == null ? 0 : ledenProblemen.Count()))
            {
                ledenProblemen = ServiceHelper.CallService<IGroepenService, IEnumerable<LedenProbleemInfo>>(
                    svc => svc.LedenControleren(groepID));

                if (ledenProblemen == null)
                {
                    telling = 0;
                    _cache.Remove(String.Format(LEDENPROBLEMENCACHEKEY, groepID));
                }
                else
                {
                    telling = ledenProblemen.Count();
                    _cache.Add(String.Format(LEDENPROBLEMENCACHEKEY, groepID),
                           ledenProblemen,
                           null,
                           Cache.NoAbsoluteExpiration,
                           new TimeSpan(2, 0, 0),
                           CacheItemPriority.NotRemovable,
                           null);
                }

                _cache.Add(String.Format(LEDENPROBLEMENAANTALCACHEKEY, groepID),
                    telling,
                           null,
                           Cache.NoAbsoluteExpiration,
                           new TimeSpan(2, 0, 0),
                           CacheItemPriority.NotRemovable,
                           null);
            }
            return ledenProblemen;
        }

        /// <summary>
        /// Verwijdert de gecachete ledenproblemen van een bepaalde groep
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de problemencache leeg te maken</param>
        public void LedenProblemenResetten(int groepID)
        {
            // Problemen met leden moeten opgenieuw opgehaald worden na deze operatie. BaseController 
            // gaat na of dat nodig is door naar de telling te kijken, maar ook de gecachete problemen 
            // moeten verwijderd worden. Als het nieuwe aantal problemen even groot is als het vorige, 
            // worden ze anders niet vervangen.
            _cache.Remove(String.Format(LEDENPROBLEMENCACHEKEY, groepID));
            _cache.Remove(String.Format(LEDENPROBLEMENAANTALCACHEKEY, groepID));
        }

        /// <summary>
        /// Verwijdert de bivakproblemen voor groep met id <paramref name="groepID"/> uit de cache.
        /// </summary>
        /// <param name="groepID">ID van groep met te verwijderen problemen</param>
        public void BivakStatusResetten(int groepID)
        {
            _cache.Remove(String.Format(BIVAKSTATUSCACHEKEY, groepID));
            _cache.Remove(String.Format(BIVAKSTATUSAANTALCACHEKEY, groepID));           
        }

        /// <summary>
        /// Haalt de problemen ivm de bivakaangifte op.
        /// </summary>
        /// <param name="groepID">ID van een groep</param>
        /// <returns>Info over wat ontbreekt mbt de bivakaangifte</returns>
        public BivakAangifteLijstInfo BivakStatusHuidigWerkjaarOphalen(int groepID)
        {
            int? telling = (int?) _cache.Get(String.Format(BIVAKSTATUSAANTALCACHEKEY, groepID));
            var resultaat = (BivakAangifteLijstInfo) _cache.Get(String.Format(BIVAKSTATUSCACHEKEY, groepID));

            if (telling == null)
            {
                resultaat =
                    ServiceHelper.CallService<IUitstappenService, BivakAangifteLijstInfo>(
                        g => g.BivakStatusOphalen(groepID));
                if (resultaat.Bivakinfos == null)
                {
                    telling = 0;
                    _cache.Remove(String.Format(BIVAKSTATUSCACHEKEY, groepID));
                }
                else
                {
                    telling = resultaat.Bivakinfos.Count();
                    _cache.Add(
                        String.Format(BIVAKSTATUSCACHEKEY, groepID),
                        resultaat,
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(2, 0, 0),
                        CacheItemPriority.NotRemovable,
                        null);
                }

                _cache.Add(
                    String.Format(BIVAKSTATUSAANTALCACHEKEY, groepID),
                    telling,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(2, 0, 0),
                    CacheItemPriority.NotRemovable,
                    null);
            }

            return resultaat;
        }

        #endregion

        #region Woonplaatsen

        /// <summary>
        /// Haalt WoonPlaatsInfo op voor woonplaatsen met gegeven <paramref name="postNummer"/>
        /// </summary>
        /// <param name="postNummer">Postnummer waarvan de woonplaatsen gevraagd zijn</param>
        /// <returns>WoonPlaatsInfo voor woonplaatsen met gegeven <paramref name="postNummer"/></returns>
        public IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen(int postNummer)
        {
            return (from g in WoonPlaatsenOphalen()
                    where g.PostNummer == postNummer
                    orderby g.Naam
                    select g).ToArray();
        }

        /// <summary>
        /// Levert een lijst op van alle woonplaatsen
        /// </summary>
        /// <returns>Een lijst met alle beschikbare woonplaatsen</returns>
        public IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen()
        {
            var result = (IEnumerable<WoonPlaatsInfo>)_cache.Get(WOONPLAATSENCACHEKEY);

            if (result == null)
            {
                // Cache geexpired, opnieuw opzoeken en cachen.

                result = ServiceHelper.CallService<IGroepenService,
                    IEnumerable<WoonPlaatsInfo>>(g => g.GemeentesOphalen());

                _cache.Add(
                    WOONPLAATSENCACHEKEY,
                    result,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(1, 0, 0, 0) /* bewaar 1 dag */,
                    CacheItemPriority.High /* niet te gauw wissen; grote kost */,
                    null);
            }
            return result;
        }

        /// <summary>
        /// Haalt alle landen op van de backend
        /// </summary>
        /// <returns>De landinfo van alle gekende landen</returns>
        public IEnumerable<LandInfo> LandenOphalen()
        {
            var result = (IEnumerable<LandInfo>)_cache.Get(LANDENCACHEKEY);

            if (result == null)
            {
                // Cache geëxpired; opnieuw ophalen en cachen.

                result = ServiceHelper.CallService<IGroepenService, IEnumerable<LandInfo>>(g => g.LandenOphalen());

                _cache.Add(
                    LANDENCACHEKEY,
                    result,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(1, 0, 0, 0) /* bewaar 1 dag */,
                    CacheItemPriority.High /* niet te gauw wissen; grote kost */,
                    null);
            }
            return result;
        }

        #endregion

        #region Allerlei

        /// <summary>
        /// Als de gav met gegeven <paramref name="login"/> gav is van precies 1 groep, dan
        /// levert deze method het ID van die groep op.  Zo niet, is het resultaat
        /// <c>0</c>.
        /// </summary>
        /// <param name="login">Login van de GAV</param>
        /// <returns>GroepID van unieke groep van gegeven GAV, anders <c>0</c></returns>
        public int UniekeGroepGav(string login)
        {
            int? id = (int?)_cache.Get(String.Format(UNIEKEGROEPCACHEKEY, login));
            if (id == null)
            {
                var mijnGroepen = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
                    (g => g.MijnGroepenOphalen());

                id = mijnGroepen.Count() == 1 ? mijnGroepen.First().ID : 0;

                _cache.Add(String.Format(UNIEKEGROEPCACHEKEY, login),
                          id,
                          null,
                          Cache.NoAbsoluteExpiration,
                          new TimeSpan(2, 0, 0),
                          CacheItemPriority.Normal,
                          null);
            }

            return (int)id;
        }

        /// <summary>
        /// Indien <c>true</c> werken we in de liveomgeving, anders in de testomgeving.
        /// </summary>
        /// <returns><c>true</c> als we live bezig zijn</returns>
        public bool IsLive()
        {
            bool? isLive = (bool?)_cache.Get(ISLIVECACHEKEY);
            if (isLive == null)
            {
                isLive = ServiceHelper.CallService<IGroepenService, bool>(svc => svc.IsLive());
                _cache.Add(
                    ISLIVECACHEKEY,
                    isLive,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(2, 0, 0),
                    CacheItemPriority.High, null);
            }

            return (bool)isLive;
        }
        #endregion
    }
}
