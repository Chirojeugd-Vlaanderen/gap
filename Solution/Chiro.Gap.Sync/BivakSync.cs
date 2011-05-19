using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

using Adres = Chiro.Gap.Orm.Adres;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// synchronisatie van bivakaangifte naar Kipadmin
	/// </summary>
	public class BivakSync: IBivakSync
	{
		private readonly ISyncPersoonService _svc;
		private readonly IAdressenDao _adressenDao;
		private readonly IGelieerdePersonenDao _gelieerdePersonenDao;
		private readonly IDeelnemersDao _deelnemersDao;

		/// <summary>
		/// Standaardconstructor
		/// </summary>
		/// <param name="svc">proxy naar syncservice</param>
		/// <param name="adressenDao">Data access voor adressen</param>
		/// <param name="gelieerdePersonenDao">data access voor gelieerde personen</param>
		/// <param name="deelnemersDao">data access voor deelnemers</param>
		public BivakSync(
			ISyncPersoonService svc, 
			IAdressenDao adressenDao, 
			IGelieerdePersonenDao gelieerdePersonenDao,
			IDeelnemersDao deelnemersDao)
		{
			_svc = svc;
			_adressenDao = adressenDao;
			_gelieerdePersonenDao = gelieerdePersonenDao;
			_deelnemersDao = deelnemersDao;
		}

		/// <summary>
		/// Bewaart de uitstap <paramref name="uitstap"/> in Kipadmin als bivak.  Zonder contactpersoon
		/// of plaats.
		/// </summary>
		/// <param name="uitstap">Te bewaren uitstap</param>
		public void Bewaren(Uitstap uitstap)
		{
			// TODO: Dit zijn waarschijnlijk te veel databasecalls

			var teSyncen = Mapper.Map<Uitstap, Bivak>(uitstap);
			_svc.BivakBewaren(teSyncen);

			GelieerdePersoon contactPersoon;

			if (uitstap.ContactDeelnemer != null)
			{
				// Er is een contactdeelnemer.  Is de persoon nog geladen?

				if (uitstap.ContactDeelnemer.GelieerdePersoon == null || uitstap.ContactDeelnemer.GelieerdePersoon.Persoon == null)
				{
					// nee, haal deelnemer opnieuw op, met contactpersoon

					var deelnemer = _deelnemersDao.Ophalen(uitstap.ContactDeelnemer.ID, d => d.GelieerdePersoon.Persoon);
					contactPersoon = deelnemer.GelieerdePersoon;
				}
				else
				{
					contactPersoon = uitstap.ContactDeelnemer.GelieerdePersoon;
				}
			}
			else
			{
				contactPersoon = null;
			}

			if (uitstap.Plaats != null && uitstap.Plaats.Adres != null)
			{
				// Haal adres opnieuw op, zodat we zeker gemeente of land mee hebben.

				var adres = _adressenDao.Ophalen(uitstap.Plaats.Adres.ID);
				_svc.BivakPlaatsBewaren(uitstap.ID, uitstap.Plaats.Naam, Mapper.Map<Adres, SyncService.Adres>(adres));
			}

			if (contactPersoon != null)
			{

				if (contactPersoon.Persoon.AdNummer != null)
				{
					// AD-nummer gekend: gewoon koppelen via AD-nummer
					_svc.BivakContactBewaren(
						uitstap.ID, 
						(int)contactPersoon.Persoon.AdNummer);
				}
				else
				{
					// Als we geen AD-nummer hebben, haal dan ook de communicatie en het
					// voorkeursadres op, want die zullen we nodig hebben om te identificeren.

					var gelPersoon = _gelieerdePersonenDao.Ophalen(
						contactPersoon.ID, PersoonsExtras.Communicatie|PersoonsExtras.VoorkeurAdres);

					// Geef door met gegevens ipv ad-nummer.  Registreer dat ad-nummer
					// in aanvraag is.
					gelPersoon.Persoon.AdInAanvraag = true;
					_gelieerdePersonenDao.Bewaren(gelPersoon, gp => gp.Persoon);

					_svc.BivakContactBewarenAdOnbekend(
						uitstap.ID,
						Mapper.Map<GelieerdePersoon, PersoonDetails>(gelPersoon));
				}
			}
		}

		/// <summary>
		/// Verwijdert uitstap met <paramref name="UitstapID"/> uit kipadmin
		/// </summary>
		/// <param name="uitstapID">ID te verwijderen uitstap</param>
		public void Verwijderen(int uitstapID)
		{
			_svc.BivakVerwijderen(uitstapID);
		}
	}
}
