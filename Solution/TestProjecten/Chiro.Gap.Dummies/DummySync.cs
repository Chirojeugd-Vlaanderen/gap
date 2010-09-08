using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Gauw een klasse die gebruikt kan worden om eender welke Sync te mocken.
	/// </summary>
	public class DummySync: IAdressenSync, ICommunicatieSync, IPersonenSync, ILedenSync, IDubbelpuntSync, IVerzekeringenSync
	{
		public void StandaardAdressenBewaren(IEnumerable<Chiro.Gap.Orm.PersoonsAdres> persoonsAdressen)
		{
		}

		public void Verwijderen(Chiro.Gap.Orm.CommunicatieVorm communicatieVorm)
		{
		}

		public void Toevoegen(CommunicatieVorm commvorm)
		{
		}

		public void Bewaren(Chiro.Gap.Orm.GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie)
		{
		}

		public void CommunicatieUpdaten(GelieerdePersoon gp)
		{
		}

		public void Bewaren(Lid l)
		{
		}

		public void FunctiesUpdaten(Lid lid)
		{
		}

		public void AfdelingenUpdaten(Lid lid)
		{
		}

		public void TypeUpdaten(Lid lid)
		{
		}

		public void Abonneren(GelieerdePersoon gp)
		{
		}

		public void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj)
		{
		}
	}
}
