// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

using Adres = Chiro.Gap.Orm.Adres;
using CommunicatieType = Chiro.Gap.Sync.SyncService.CommunicatieType;
using Persoon = Chiro.Gap.Orm.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse die de synchronisatie van persoonsgegevens naar Kipadmin regelt
	/// </summary>
	public class PersonenSync : IPersonenSync
	{
		private readonly ISyncPersoonService _svc;
		private readonly ICommunicatieVormDao _cVormDao;

		/// <summary>
		/// Constructor voor PersonenSync
		/// </summary>
		/// <param name="svc">Service voor synchronsatie met Kipadmin</param>
		/// <param name="cVormDao">Data access object voor communicatievormen</param>
		public PersonenSync(ISyncPersoonService svc, ICommunicatieVormDao cVormDao)
		{
			_svc = svc;
			_cVormDao = cVormDao;
		}

		/// <summary>
		/// Stuurt de persoonsgegevens, samen met eventueel adressen en/of communicatie, naar Kipadmin
		/// </summary>
		/// <param name="gp">Gelieerde persoon, persoonsinfo</param>
		/// <param name="metStandaardAdres">Stuurt ook het standaardadres mee (moet dan wel gekoppeld zijn)</param>
		/// <param name="metCommunicatie">Stuurt ook communicatie mee.  Hiervoor wordt expliciet alle
		/// communicatie-info opgehaald, omdat de workers typisch niet toestaan dat de gebruiker alle
		/// communicatie ophaalt.</param>
		public void Bewaren(GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie)
		{
			// Wijzigingen van personen met ad-nummer worden doorgesluisd
			// naar Kipadmin.

			Debug.Assert(gp.Persoon.AdNummer != null || gp.Persoon.AdInAanvraag);

			var syncPersoon = Mapper.Map<Persoon, SyncService.Persoon>(gp.Persoon);
			_svc.PersoonUpdaten(syncPersoon);

			if (metStandaardAdres && gp.PersoonsAdres != null)
			{
				// Adressen worden mee bewaard.  Update standaardadres, als dat er is
				// (standaardadres is gelieerdePersoon.PersoonsAdres;
				// alle adressen in gelieerdePersoon.Persoon.PersoonsAdres).

				// TODO (#238): Buitenlandse adressen!
				var syncAdres = Mapper.Map<Adres, SyncService.Adres>(gp.PersoonsAdres.Adres);

				var syncBewoner = new Bewoner
				{
					Persoon = Mapper.Map<Persoon, SyncService.Persoon>(gp.Persoon),
					AdresType = (AdresTypeEnum)gp.PersoonsAdres.AdresType
				};

				_svc.StandaardAdresBewaren(syncAdres, new List<Bewoner> { syncBewoner });
			}
			if (metCommunicatie)
			{
				// Haal expliciet alle communicatievormen op, want de aanroepende method
				// kent waarschijnlijk enkel de communicatievormen van 1 gelieerde persoon

				var syncCommunicatie = Mapper.Map<IEnumerable<CommunicatieVorm>, List<CommunicatieMiddel>>(_cVormDao.ZoekenOpPersoon(gp.Persoon.ID));

				_svc.AlleCommunicatieBewaren(syncPersoon, syncCommunicatie);
			}
		}

		/// <summary>
		/// Stuurt *alle* communicatie van de persoon gekoppeld aan <paramref name="gp"/> naar Kipadmin 
		/// (dus ook de communicatie gekend bij andere groepen)
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		public void CommunicatieUpdaten(GelieerdePersoon gp)
		{
			var syncPersoon = Mapper.Map<Persoon, SyncService.Persoon>(gp.Persoon);
			var syncCommunicatie = Mapper.Map<IEnumerable<CommunicatieVorm>, List<CommunicatieMiddel>>(_cVormDao.ZoekenOpPersoon(gp.Persoon.ID));

			_svc.AlleCommunicatieBewaren(syncPersoon, syncCommunicatie);
		}
	}
}
