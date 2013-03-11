// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Klasse om veel gebruikte zaken mee op te vragen, die dan ook gecachet kunnen worden.
    /// Enkel als iets niet gecachet is, wordt de data access aangeroepen
    /// </summary>
    public class VeelGebruikt : IVeelGebruikt
    {
        private const string GROEPSWERKJAARCACHEKEY = "gwj{0}";
        private const string NATIONALEFUNCTIESCACHEKEY = "natfun";
        private const string GROEPIDCACHEKEY = "gid_{0}";

        private readonly Cache _cache = HttpRuntime.Cache;

        /// <summary>
        /// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/>
        /// uit de cache.
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep waarvan groepswerkjaarcache te resetten
        /// </param>
        public void GroepsWerkJaarResetten(int groepID)
        {
            _cache.Remove(string.Format(GROEPSWERKJAARCACHEKEY, groepID));
        }

        /// <summary>
        /// Haalt van de groep met gegeven <paramref name="groepID"/> het ID van het recentste groepswerkjaar op.
        /// </summary>
        /// <param name="groepID">
        ///     ID van de groep waarvan groepswerkjaarID gevraagd
        /// </param>
        /// <returns>
        /// Het ID van het recentste groespwerkjaar van de groep
        /// </returns>
        public int GroepsWerkJaarIDOphalen(int groepID)
        {
            var gwj = (GroepsWerkJaar)_cache.Get(string.Format(GROEPSWERKJAARCACHEKEY, groepID));

            if (gwj == null)
            {
                throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
                // gwj = _groepsWerkJaarDao.RecentsteOphalen(groepID, gwjr => gwjr.Groep);

                //_cache.Add(
                //    string.Format(GROEPSWERKJAARCACHEKEY, groepID),
                //    gwj,
                //    null,
                //    Cache.NoAbsoluteExpiration,
                //    new TimeSpan(2, 0, 0),
                //    CacheItemPriority.Normal,
                //    null);
            }

            return gwj.ID;
        }

        /// <summary>
        /// Haalt alle nationale functies op
        /// </summary>
        /// <param name="functieRepo">Repository die deze worker kan gebruiken om functies
        ///     op te vragen.</param>
        /// <returns>
        /// Lijstje nationale functies
        /// </returns>
        /// <remarks>
        /// De repository wordt bewust niet geregeld door de constructor van deze klasse,
        /// omdat we moeten vermijden dat de IOC-container hiervoor een nieuwe context aanmaakt.
        /// </remarks>
        public List<Functie> NationaleFunctiesOphalen(IRepository<Functie> functieRepo)
        {
            if (_cache[NATIONALEFUNCTIESCACHEKEY] == null)
            {
                var nationaleFuncties = from fn in functieRepo.Select()
                                        where fn.IsNationaal
                                        select fn;

                _cache.Add(
                    NATIONALEFUNCTIESCACHEKEY,
                    nationaleFuncties.ToList(),
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(1, 0, 0, 0) /* bewaar 1 dag */,
                    CacheItemPriority.Low,
                    null);
            }

            return _cache[NATIONALEFUNCTIESCACHEKEY] as List<Functie>;
        }

        /// <summary>
        /// Haalt het groepID van de groep met gegeven stamnummer op uit de cache.
        /// </summary>
        /// <param name="code">
        /// Stamnummer van groep waarvan groepID te bepalen is
        /// </param>
        /// <returns>
        /// GroepID van de groep met stamnummer <paramref name="code"/>.
        /// </returns>
        public int CodeNaarGroepID(string code)
        {
            var groepID = (int?)_cache.Get(string.Format(GROEPIDCACHEKEY, code));

            if (groepID == null || groepID == 0)
            {
                throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
                //groepID = _groepenDao.Ophalen(code).ID;

                //_cache.Add(
                //    string.Format(GROEPIDCACHEKEY, code),
                //    groepID,
                //    null,
                //    Cache.NoAbsoluteExpiration,
                //    new TimeSpan(2, 0, 0),
                //    CacheItemPriority.Normal,
                //    null);
            }

            return groepID ?? 0;
        }
    }
}