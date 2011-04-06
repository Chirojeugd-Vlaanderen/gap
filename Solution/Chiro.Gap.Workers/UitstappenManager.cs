using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Businesslogica wat betreft uitstappen en bivakken
	/// </summary>
	public class UitstappenManager
	{
		private readonly IGroepsWerkJaarDao _groepsWerkJaarDao;
		private readonly IAutorisatieManager _autorisatieManager;
		private readonly IDao<Uitstap> _uitstappenDao;


		/// <summary>
		/// Constructor.  De parameters moeten 'ingevuld' via dependency inejction
		/// </summary>
		/// <param name="groepsWerkJaarDao">Data access voor groepswerkjaren</param>
		/// <param name="autorisatieManager">Businesslogica voor autorisatie</param>
		public UitstappenManager(
			IDao<Uitstap> uitstappenDao, 
			IGroepsWerkJaarDao groepsWerkJaarDao, 
			IAutorisatieManager autorisatieManager)
		{
			_uitstappenDao = uitstappenDao;
			_groepsWerkJaarDao = groepsWerkJaarDao;
			_autorisatieManager = autorisatieManager;
		}

		/// <summary>
		/// Koppelt een uitstap aan een groepswerkjaar.  Dit moet typisch
		/// enkel gebeuren bij een nieuwe uitstap.
		/// </summary>
		/// <param name="uitstap">Te koppelen uitstap</param>
		/// <param name="gwj">Te koppelen groepswerkjaar</param>
		/// <returns><paramref name="uitstap"/>, gekoppeld aan <paramref name="gwj"/>.</returns>
		public Uitstap Koppelen(Uitstap uitstap, GroepsWerkJaar gwj)
		{
			if (!_autorisatieManager.IsGavGroepsWerkJaar(gwj.ID) || !_autorisatieManager.IsGavUitstap(uitstap.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else if (uitstap.GroepsWerkJaar != null && uitstap.GroepsWerkJaar != gwj)
			{
				throw new BlokkerendeObjectenException<GroepsWerkJaar>(
					uitstap.GroepsWerkJaar,
				        Properties.Resources.UitstapAlGekoppeld);
			}
			else if (!_groepsWerkJaarDao.IsRecentste(gwj.ID))
			{
				throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietBeschikbaar, Properties.Resources.GroepsWerkJaarVoorbij);
			}
			else
			{
				uitstap.GroepsWerkJaar = gwj;
				gwj.Uitstap.Add(uitstap);
				return uitstap;
			}
		}

		/// <summary>
		/// Haalt de uitstap met gegeven <paramref name="uitstapID"/> op, inclusief
		/// het gekoppelde groepswerkjaar.
		/// </summary>
		/// <param name="uitstapID">ID op te halen uitstap</param>
		/// <returns>De uitstap, met daaraan gekoppeld het groepswerkjaar.</returns>
		public Uitstap Ophalen(int uitstapID)
		{
			if (!_autorisatieManager.IsGavUitstap(uitstapID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				return _uitstappenDao.Ophalen(uitstapID, u => u.GroepsWerkJaar);
			}

		}

		/// <summary>
		/// Bewaart de uitstap met het gekoppelde groepswerkjaar
		/// </summary>
		/// <param name="uitstap"></param>
		public Uitstap Bewaren(Uitstap uitstap)
		{
			if (!_autorisatieManager.IsGavUitstap(uitstap.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else if (!_groepsWerkJaarDao.IsRecentste(uitstap.GroepsWerkJaar.ID))
			{
				throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietBeschikbaar, Properties.Resources.GroepsWerkJaarVoorbij);
			}
			else
			{
				return _uitstappenDao.Bewaren(uitstap, u => u.GroepsWerkJaar.WithoutUpdate());
			}
		}
	}
}
