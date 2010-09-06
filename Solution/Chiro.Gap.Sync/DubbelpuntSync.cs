using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

using Persoon = Chiro.Gap.Orm.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse voor de synchronisatie van dubbelpuntabonnementen
	/// </summary>
	public class DubbelpuntSync : IDubbelpuntSync
	{
		private readonly ISyncPersoonService _svc;
		private readonly IGroepsWerkJaarDao _groepsWerkJaarDao;

		/// <summary>
		/// Creeert een nieuwe DubbelpuntSync
		/// </summary>
		/// <param name="groepsWerkJaarDao">Data access object voor groepsgerelateerde zaken</param>
		/// <param name="service">Proxy naar de syncrhonisatieservice</param>
		public DubbelpuntSync(ISyncPersoonService service, IGroepsWerkJaarDao groepsWerkJaarDao)
		{
			_svc = service;
			_groepsWerkJaarDao = groepsWerkJaarDao;
		}

		/// <summary>
		/// Synct een Dubbelpuntabonnement voor het huidige groepswerkjaar naar Kipadmin.
		/// </summary>
		/// <param name="gp">Gelieerde persoon die een abonnement wil voor dit werkjaar</param>
		public void Abonneren(GelieerdePersoon gp)
		{
			var huidigWj = _groepsWerkJaarDao.RecentsteOphalen(gp, gwj => gwj.Groep);

			_svc.DubbelpuntBestellen(
				Mapper.Map<Persoon, SyncService.Persoon>(gp.Persoon),
				huidigWj.Groep.Code,
				huidigWj.WerkJaar);
		}
	}
}
