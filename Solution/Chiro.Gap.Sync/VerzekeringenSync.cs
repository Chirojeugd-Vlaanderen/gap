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

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Deze klasse staat in voor het overzetten van verzekeringsgegevens naar Kipadmin.
	/// </summary>
	public class VerzekeringenSync : IVerzekeringenSync
	{
		private readonly ISyncPersoonService _svc;
		private readonly IGelieerdePersonenDao _gelieerdePersonenDao;
		private readonly IPersonenDao _personenDao;

		/// <summary>
		/// Creert een nieuwe VerzekeringenSync
		/// </summary>
		/// <param name="svc">Proxy naar de synchronisatieservice</param>
		/// <param name="gelieerdePersonenDao">Data access object voor gelieerde personen</param>
		/// <param name="personenDao">Data access object voor personen</param>
		public VerzekeringenSync(
			ISyncPersoonService svc, 
			IGelieerdePersonenDao gelieerdePersonenDao,
			IPersonenDao personenDao)
		{
			_svc = svc;
			_gelieerdePersonenDao = gelieerdePersonenDao;
			_personenDao = personenDao;
		}

		/// <summary>
		/// Zet de gegeven <paramref name="persoonsVerzekering"/> over naar Kipadmin.
		/// </summary>
		/// <param name="persoonsVerzekering">over te zetten persoonsverzekering</param>
		/// <param name="gwj">Bepaalt werkjaar en groep die factuur zal krijgen</param>
		public void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj)
		{
			if (persoonsVerzekering.Persoon.AdNummer != null)
			{
				// Verzekeren op basis van AD-nummer
				_svc.LoonVerliesVerzekeren(
					(int) persoonsVerzekering.Persoon.AdNummer,
					gwj.Groep.Code,
					gwj.WerkJaar);
			}
			else
			{
				// Verzekeren op basis van details.  
				// Hoewel een verzekering loonverlies
				// enkel mogelijk is voor leden, mogen we er niet vanuit gaan dat het AD-nummer al
				// in aanvraag is.  Het kan namelijk zijn dat het lid nog in zijn probeerperiode is.
				// Vraag dus sowieso een AD-nummer aan.

				persoonsVerzekering.Persoon.AdInAanvraag = true;
				_personenDao.Bewaren(persoonsVerzekering.Persoon);
				
				// Haal even de gelieerde persoon op, om gemakkelijk de details te kunnen mappen.
				// TODO (#754): Op die manier verliezen we mogelijk communicatiemiddelen

				var gp = _gelieerdePersonenDao.Ophalen(
					persoonsVerzekering.Persoon.ID,
					gwj.Groep.ID,
					gelp => gelp.Persoon,
					gelp => gelp.PersoonsAdres.Adres.StraatNaam,
					gelp => gelp.PersoonsAdres.Adres.WoonPlaats,
					gelp => gelp.Communicatie.First().CommunicatieType);

				_svc.LoonVerliesVerzekerenAdOnbekend(
					Mapper.Map<GelieerdePersoon, SyncService.PersoonDetails>(gp),
					gwj.Groep.Code,
					gwj.WerkJaar);
			}
		}
	}
}
