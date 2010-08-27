// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Interface voor synchronisatie van lidinfo naar Kipadmin
	/// </summary>
	public class LedenSync : ILedenSync
	{
		private readonly ISyncPersoonService _svc;

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
		public LedenSync(ISyncPersoonService svc)
		{
			_svc = svc;
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

			if (l.GelieerdePersoon.Persoon.AdNummer != null)
			{
				_svc.LidBewaren(
					(int) l.GelieerdePersoon.Persoon.AdNummer,
					l.GroepsWerkJaar.Groep.Code,
					l.GroepsWerkJaar.WerkJaar,
					l is Kind ? LidTypeEnum.Kind : LidTypeEnum.Leiding,
					nationaleFuncties,
					officieleAfdelingen);
			}
			else
			{
				// Ook persoonsgegevens meesturen
				throw new NotImplementedException();
			}
		}
	}
}
