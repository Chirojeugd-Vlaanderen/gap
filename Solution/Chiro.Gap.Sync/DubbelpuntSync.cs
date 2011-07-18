using System.Diagnostics;
using System.Linq;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse voor de synchronisatie van dubbelpuntabonnementen
	/// </summary>
	public class DubbelpuntSync : IDubbelpuntSync
	{
		private readonly ISyncPersoonService _svc;
		private readonly IGroepsWerkJaarDao _groepsWerkJaarDao;
		private readonly IPersonenDao _personenDao;
		private readonly IGelieerdePersonenDao _gelieerdePersonenDao;

		/// <summary>
		/// Creeert een nieuwe DubbelpuntSync
		/// </summary>
		/// <param name="groepsWerkJaarDao">Data access object voor groepsgerelateerde zaken</param>
		/// <param name="service">Proxy naar de syncrhonisatieservice</param>
		/// <param name="personenDao">Data access object voor persoonsgerelateerde zaken</param>
		/// <param name="gelieerdePersonenDao">Data access object voor gelieerde personen</param>
		public DubbelpuntSync(
			ISyncPersoonService service, 
			IGroepsWerkJaarDao groepsWerkJaarDao,
			IPersonenDao personenDao,
			IGelieerdePersonenDao gelieerdePersonenDao)
		{
			_svc = service;
			_groepsWerkJaarDao = groepsWerkJaarDao;
			_personenDao = personenDao;
			_gelieerdePersonenDao = gelieerdePersonenDao;
		}

	    /// <summary>
	    /// Synct een dubbelpuntabonnement naar Kipadmin
	    /// </summary>
        /// <param name="abonnement">Te syncen abonnement</param>
	    public void Abonneren(Abonnement abonnement)
		{
			Debug.Assert(abonnement.GelieerdePersoon != null);
			Debug.Assert(abonnement.GelieerdePersoon.Persoon != null);
            Debug.Assert(abonnement.GroepsWerkJaar != null);
            Debug.Assert(abonnement.GroepsWerkJaar.Groep != null);

	        var gp = abonnement.GelieerdePersoon;
	        var huidigWj = abonnement.GroepsWerkJaar;

			if (gp.Persoon.AdNummer != null)
			{
				// Jeeej! We hebben een ad-nummer! Dubbelpuntabonnement is een fluitje van een cent.

				_svc.DubbelpuntBestellen(
					(int)gp.Persoon.AdNummer,
					huidigWj.Groep.Code,
					huidigWj.WerkJaar);
			}
			else
			{
				// Geen AD-Nummer; dan moet er met het abonnementsverzoek meteen alle info meegestuurd
				// worden om dat AD-nummer te maken.  Markeer persoon als persoon met ad-nummer in aanvraag.

				gp.Persoon.AdInAanvraag = true;
				_personenDao.Bewaren(gp.Persoon);

				// Haal gelieerde persoon opnieuw op, met zo veel mogelijk info voor identificatie aan
				// Kipadminkant.

				var gpMetDetails = _gelieerdePersonenDao.Ophalen(
					gp.Persoon.ID,
					huidigWj.Groep.ID,
					true,
					gelp => gelp.Persoon,
					gelp => gelp.Communicatie.First().CommunicatieType);

				_svc.DubbelpuntBestellenNieuwePersoon(
					Mapper.Map<GelieerdePersoon, PersoonDetails>(gpMetDetails),
					huidigWj.Groep.Code,
					huidigWj.WerkJaar);
			}
		}
	}
}
