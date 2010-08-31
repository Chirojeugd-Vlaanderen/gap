// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AutoMapper;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

using Adres = Chiro.Gap.Orm.Adres;
using AdresTypeEnum = Chiro.Gap.Sync.SyncService.AdresTypeEnum;
using Persoon = Chiro.Gap.Orm.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Interface voor synchronisatie van lidinfo naar Kipadmin
	/// </summary>
	public class LedenSync : ILedenSync
	{
		private readonly ISyncPersoonService _svc;
		private readonly ILedenDao _ledenDao;
		private readonly ICommunicatieVormDao _cVormDao;
		private readonly IPersonenDao _personenDao;

		// TODO: Dit gaat waarschijnlijk ook met AutoMapper
		private readonly Dictionary<NationaleFunctie, SyncService.FunctieEnum> _functieVertalng =
			new Dictionary<NationaleFunctie, FunctieEnum>
				{
					{NationaleFunctie.ContactPersoon, FunctieEnum.ContactPersoon},
					{NationaleFunctie.FinancieelVerantwoordelijke, FunctieEnum.FinancieelVerantwoordelijke},
					{NationaleFunctie.GroepsLeiding, FunctieEnum.GroepsLeiding},
					{NationaleFunctie.JeugdRaad, FunctieEnum.JeugdRaad},
					{NationaleFunctie.KookPloeg, FunctieEnum.KookPloeg},
					{NationaleFunctie.Proost, FunctieEnum.Proost}
				};

		private readonly Dictionary<NationaleAfdeling, SyncService.AfdelingEnum> _afdelingVertaling =
			new Dictionary<NationaleAfdeling, AfdelingEnum>
				{
					{NationaleAfdeling.Ribbels, AfdelingEnum.Ribbels},
					{NationaleAfdeling.Speelclub, AfdelingEnum.Speelclub},
					{NationaleAfdeling.Rakwis, AfdelingEnum.Rakwis},
					{NationaleAfdeling.Titos, AfdelingEnum.Titos},
					{NationaleAfdeling.Ketis, AfdelingEnum.Ketis},
					{NationaleAfdeling.Aspis, AfdelingEnum.Aspis},
					{NationaleAfdeling.Speciaal, AfdelingEnum.Speciaal}
				};

		/// <summary>
		/// Creeert nieuwe klasse voor ledensynchronisatie, die met Kipadmin zal communiceren via
		/// <paramref name="svc"/>
		/// </summary>
		/// <param name="svc">te gebruiken KipSyncService voor communicatie met Kipadmn</param>
		/// <param name="cVormDao">Data access object voor communicatievormen</param>
		/// <param name="ledenDao">Data access object voor leden</param>
		/// <param name="personenDao">Data access object voor personen</param>
		public LedenSync(ISyncPersoonService svc, ICommunicatieVormDao cVormDao, ILedenDao ledenDao, IPersonenDao personenDao)
		{
			_svc = svc;
			_cVormDao = cVormDao;
			_ledenDao = ledenDao;
			_personenDao = personenDao;
		}

		/// <summary>
		/// Stuurt een lid naar Kipadmin
		/// </summary>
		/// <param name="l">Te bewaren lid</param>
		/// <remarks>voor het gemak gaan we ervan uit dat persoonsgegevens, adressen, afdelingen en functies al
		/// gekoppeld zijn.  Communicatie moeten we sowieso opnieuw ophalen, want kan ook gekoppeld
		/// zijn aan andere gelieerde personen.</remarks>
		public void Bewaren(Lid l)
		{
			Debug.Assert(l.GelieerdePersoon != null);
			Debug.Assert(l.GelieerdePersoon.Persoon != null);
			Debug.Assert(l.GroepsWerkJaar != null);
			Debug.Assert(l.GroepsWerkJaar.Groep != null);

			var nationaleFuncties = (from f in l.Functie
			                         where f.IsNationaal
			                         select _functieVertalng[(NationaleFunctie) f.ID]).ToList();

			List<AfdelingEnum> officieleAfdelingen;

			if (l is Kind)
			{
				officieleAfdelingen = new List<AfdelingEnum> { _afdelingVertaling[(NationaleAfdeling)((l as Kind).AfdelingsJaar.OfficieleAfdeling.ID)] };
			}
			else
			{
				officieleAfdelingen = (from a in (l as Leiding).AfdelingsJaar
				                       select _afdelingVertaling[(NationaleAfdeling) a.OfficieleAfdeling.ID]).ToList();
			}

			var lidGedoe = new LidGedoe
			               	{
			               		StamNummer = l.GroepsWerkJaar.Groep.Code,
			               		WerkJaar = l.GroepsWerkJaar.WerkJaar,
			               		LidType = l is Kind ? LidTypeEnum.Kind : LidTypeEnum.Leiding,
			               		NationaleFuncties = nationaleFuncties,
			               		OfficieleAfdelingen = officieleAfdelingen
			               	};

			if (l.GelieerdePersoon.Persoon.AdNummer != null)
			{
				_svc.LidBewaren((int) l.GelieerdePersoon.Persoon.AdNummer, lidGedoe);
			}
			else
			{
				// Markeer AD-nummer als zijnde 'in aanvraag'

				l.GelieerdePersoon.Persoon.AdInAanvraag = true;
				_personenDao.Bewaren(l.GelieerdePersoon.Persoon);

				// Ook persoonsgegevens meesturen

				var syncPersoon = Mapper.Map<Persoon, SyncService.Persoon>(l.GelieerdePersoon.Persoon);
				SyncService.Adres syncAdres = null;
				SyncService.AdresTypeEnum syncAdresType = AdresTypeEnum.ANDER;

				// Laat ons hopen dat onze persoon een adres heeft.

				if (l.GelieerdePersoon.PersoonsAdres != null)
				{
					syncAdres = Mapper.Map<Adres, SyncService.Adres>(l.GelieerdePersoon.PersoonsAdres.Adres);
					syncAdresType = (SyncService.AdresTypeEnum) l.GelieerdePersoon.PersoonsAdres.AdresType;
				}

				// Communicatie

				var syncCommunicatie = Mapper.Map<IEnumerable<CommunicatieVorm>, List<CommunicatieMiddel>>(_cVormDao.ZoekenOpPersoon(l.GelieerdePersoon.Persoon.ID));

				_svc.NieuwLidBewaren(
					syncPersoon,
					syncAdres,
					syncAdresType,
					syncCommunicatie,
					lidGedoe);
			}
		}

		/// <summary>
		/// Updatet de functies van het lid in Kipadmin
		/// </summary>
		/// <param name="lid">Lid met functies</param>
		/// <remarks>Als geen persoonsgegevens meegeleverd zijn, dan zoeken we die wel even op in de database.</remarks>
		public void FunctiesUpdaten(Lid lid)
		{
			Lid l;

			if (lid.GelieerdePersoon == null || lid.GelieerdePersoon.Persoon == null)
			{
				// Als we geen persoonsgegevens hebben, halen we die op.  (AD-nummer nodig)
				l = _ledenDao.Ophalen(lid.ID, ld => ld.GelieerdePersoon.Persoon, ld=>ld.Functie);
			}
			else
			{
				l = lid;
			}

			var kipFunctieIDs = (from f in l.Functie
			                     where f.IsNationaal
			                     select _functieVertalng[(NationaleFunctie) f.ID]).ToArray();


			throw new NotImplementedException();
		}
	}
}
