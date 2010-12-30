using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

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

		private readonly IGroepsWerkJaarDao _groepsWerkJaarDao;
		private readonly IFunctiesDao _functiesDao;

		private readonly Cache _cache = HttpRuntime.Cache;

		/// <summary>
		/// Standaardconstructor.  Data access objects worden typisch geregeld via
		/// inversion of control.
		/// </summary>
		/// <param name="groepsWerkJaarDao">zorgt voor data access ivm groepswerkjaren</param>
		/// <param name="functiesDao">data access voor functies</param>
		public VeelGebruikt(IGroepsWerkJaarDao groepsWerkJaarDao, IFunctiesDao functiesDao)
		{
			_groepsWerkJaarDao = groepsWerkJaarDao;
			_functiesDao = functiesDao;
		}

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
		/// Haalt van de groep met gegeven <paramref name="groepID"/> het recentste
		/// groepswerkjaar op, inclusief de groep zelf.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan groepswerkjaar gevraagd</param>
		/// <returns>Het groepswerkjaar, met daaraan gekoppeld de groep</returns>
		public GroepsWerkJaar GroepsWerkJaarOphalen(int groepID)
		{
			var gwj = (GroepsWerkJaar) _cache.Get(String.Format(GROEPSWERKJAARCACHEKEY, groepID));

			if (gwj == null)
			{
				gwj = _groepsWerkJaarDao.RecentsteOphalen(groepID, gwjr => gwjr.Groep);

				_cache.Add(
					String.Format(GROEPSWERKJAARCACHEKEY, groepID), 
					gwj, 
					null, 
					Cache.NoAbsoluteExpiration, 
					new TimeSpan(2, 0, 0), 
					CacheItemPriority.Normal, 
					null);
			}

			return gwj;
		}

		/// <summary>
		/// Haalt alle nationale functies op
		/// </summary>
		/// <returns>Lijstje nationale functies</returns>
		public IEnumerable<Functie> NationaleFunctiesOphalen()
		{

			if (_cache[NATIONALEFUNCTIESCACHEKEY] == null)
			{
				_cache.Add(
					NATIONALEFUNCTIESCACHEKEY,
					_functiesDao.NationaalBepaaldeFunctiesOphalen(),
					null,
					Cache.NoAbsoluteExpiration,
					new TimeSpan(1, 0, 0, 0) /* bewaar 1 dag */,
					CacheItemPriority.Low,
					null);
			}

			return _cache[NATIONALEFUNCTIESCACHEKEY] as IEnumerable<Functie>;
		}
	}

}
