﻿using System.Diagnostics;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Regelt de synchronisatie van communicatiemiddelen naar Kipadmin
	/// </summary>
	public class CommunicatieSync : ICommunicatieSync
	{
		private readonly ISyncPersoonService _svc;

		/// <summary>
		/// Constructor voor CommunicatieSync
		/// </summary>
		/// <param name="svc">Te gebruiken service</param>
		public CommunicatieSync(ISyncPersoonService svc)
		{
			_svc = svc;	
		}


		/// <summary>
		/// Verwijdert een communicatievorm uit Kipadmin
		/// </summary>
		/// <param name="communicatieVorm">Te verwijderen communicatievorm, gekoppeld aan een gelieerde persoon 
		/// met ad-nummer.</param>
		public void Verwijderen(CommunicatieVorm communicatieVorm)
		{
			Debug.Assert(communicatieVorm.GelieerdePersoon != null);
			Debug.Assert(communicatieVorm.GelieerdePersoon.Persoon != null);
			Debug.Assert(communicatieVorm.GelieerdePersoon.Persoon.AdNummer != null);

			_svc.CommunicatieVerwijderen(
				(int)communicatieVorm.GelieerdePersoon.Persoon.AdNummer,
				new CommunicatieMiddel
				{
					Type = (SyncService.CommunicatieType)communicatieVorm.CommunicatieType.ID,
					Waarde = communicatieVorm.Nummer
				});
		}
	}
}