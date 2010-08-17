using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

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
		/// <param name="gp">gelieerde persoon, persoonsinfo</param>
		/// <param name="metStandaardAdres">stuurt ook het standaardadres mee (moet dan wel gekoppeld zijn)</param>
		/// <param name="metCommunicatie">stuurt ook communicatie mee.  Hiervoor wordt expliciet alle
		/// communicatie-info opgehaald, omdat de workers typisch niet toestaan dat de gebruiker alle
		/// communicatie ophaalt.</param>
		public void Bewaren(GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie)
		{
			// Wijzigingen van personen met ad-nummer worden doorgesluisd
			// naar Kipadmin.

			Debug.Assert(gp.Persoon.AdNummer != null);

			var syncPersoon = new SyncService.Persoon
			                  	{
			                  		AdNr = gp.Persoon.AdNummer,
							Geboortedatum = gp.Persoon.GeboorteDatum,
							Geslacht = (SyncService.GeslachtsEnum)gp.Persoon.Geslacht,
							ID = gp.Persoon.ID,
							Naam = gp.Persoon.Naam,
							Sterfdatum = gp.Persoon.SterfDatum,
							Voornaam = gp.Persoon.VoorNaam
			                  	};

			_svc.PersoonUpdaten(syncPersoon);

			if (metStandaardAdres && gp.PersoonsAdres != null)
			{
				// Adressen worden mee bewaard.  Update standaardadres, als dat er is
				// (standaardadres is gelieerdePersoon.PersoonsAdres;
				// alle adressen in gelieerdePersoon.Persoon.PersoonsAdres).

				// TODO (#238): Buitenlandse adressen!
				var syncAdres = new SyncService.Adres
				{
					Bus = gp.PersoonsAdres.Adres.Bus,
					HuisNr = gp.PersoonsAdres.Adres.HuisNr,
					Land = "",
					PostNr = gp.PersoonsAdres.Adres.StraatNaam.PostNummer,
					Straat = gp.PersoonsAdres.Adres.StraatNaam.Naam,
					WoonPlaats = gp.PersoonsAdres.Adres.WoonPlaats.Naam
				};

				var syncBewoner = new SyncService.Bewoner
				{
					AdNummer = (int)gp.Persoon.AdNummer,
					AdresType = (SyncService.AdresTypeEnum)gp.PersoonsAdres.AdresType
				};

				_svc.StandaardAdresBewaren(syncAdres, new List<Bewoner> { syncBewoner });
			}
			if (metCommunicatie)
			{
				// Haal expliciet alle communicatievormen op, want de aanroepende method
				// kent waarschijnlijk enkel de communicatievormen van 1 gelieerde persoon

				var syncCommunicatie = from comm in _cVormDao.ZoekenOpPersoon(gp.Persoon.ID)
						       select new SyncService.CommunicatieMiddel
						       {
							       GeenMailings = !comm.IsVoorOptIn,
							       Type = (SyncService.CommunicatieType)comm.CommunicatieType.ID,
							       Waarde = comm.Nummer
						       };
				_svc.CommunicatieBewaren((int)gp.Persoon.AdNummer, syncCommunicatie.ToList());
			}
		}
	}
}
