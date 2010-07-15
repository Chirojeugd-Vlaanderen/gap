using System;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Manager voor alwat met verzekeringen te maken heeft.
	/// TODO: Dit was misschien beter een 'PersoonsVerzekeringenManager' geweest?
	/// </summary>
	public class VerzekeringenManager
	{
		private IDao<VerzekeringsType> _verzekeringenDao;
		private IDao<PersoonsVerzekering> _persoonsVerzekeringenDao;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Construeert een nieuwe verzekeringenmanager
		/// </summary>
		/// <param name="vdao">Data Access Object voor verzekeringstypes</param>
		public VerzekeringenManager(
			IDao<VerzekeringsType> vdao, 
			IDao<PersoonsVerzekering> pvdao,
			IAutorisatieManager auMgr)
		{
			_verzekeringenDao = vdao;
			_persoonsVerzekeringenDao = pvdao;
			_autorisatieMgr = auMgr;
		}


		/// <summary>
		/// Haalt een verzekeringstype op uit de database
		/// </summary>
		/// <param name="verzekering"></param>
		/// <returns></returns>
		public VerzekeringsType Ophalen(Verzekering verzekering)
		{
			// GAV-rechten zijn hier irrelevant.
			return _verzekeringenDao.Ophalen((int) verzekering);
		}

		/// <summary>
		/// Verzekert een lid
		/// </summary>
		/// <param name="l">Te verzekeren lid, met daaraan gekoppeld alle verzekeringen</param>
		/// <param name="verz">Type van de verzekering</param>
		/// <param name="beginDatum">Begindatum van de verzekering; moet in de toekomst liggen.</param>
		/// <param name="eindDatum">Einddatum van de verzekering</param>
		/// <returns>Het gecreeerde PersoonsVerzekringsobject.</returns>
		public PersoonsVerzekering Verzekeren(Lid l, VerzekeringsType verz, DateTime beginDatum, DateTime eindDatum)
		{
			if (!_autorisatieMgr.IsGavLid(l.ID))
			{
				throw new GeenGavException();
			}

			if (beginDatum > eindDatum)
			{
				throw new FoutNummerException(FoutNummer.CronologieFout, Properties.Resources.FouteDatumVolgorde);
			}

			if (verz.TotEindeWerkJaar && eindDatum != GroepsWerkJaarManager.EindDatum(l.GroepsWerkJaar))
			{
				throw new FoutNummerException(
					FoutNummer.ValidatieFout, 
					Properties.Resources.OngeldigeEindDatumVerzekering);
			}

			// Onderstaande controle op 'al bestaande' verzekering, gebeurt niet door de database, omdat
			// dit meer inhoudt dan een unique index/unique constraint.

			var query = from v in l.GelieerdePersoon.Persoon.PersoonsVerzekering
				    where v.VerzekeringsType.ID == verz.ID
					  && (v.Tot >= beginDatum || v.Van <= eindDatum)
				    select v;


			var bestaande = query.FirstOrDefault();

			if (bestaande != null)
			{
				throw new BlokkerendeObjectenException<PersoonsVerzekering>(
					bestaande, 
					Properties.Resources.OverlappendeVerzekering);
			}

			PersoonsVerzekering pv = new PersoonsVerzekering
			                         	{
			                         		Van = beginDatum,
			                         		Tot = eindDatum,
			                         		Persoon = l.GelieerdePersoon.Persoon,
			                         		VerzekeringsType = verz
			                         	};

			l.GelieerdePersoon.Persoon.PersoonsVerzekering.Add(pv);
			verz.PersoonsVerzekering.Add(pv);

			return pv;
		}

		/// <summary>
		/// Persisteert een persoonsverzekering, inclusief koppeling naar persoon en verzekeringstype
		/// </summary>
		/// <param name="verzekering">Te persisteren persoonsverzekering</param>
		/// <returns>De bewaarde versie van de persoonsverzekering</returns>
		public PersoonsVerzekering PersoonsVerzekeringBewaren(PersoonsVerzekering verzekering)
		{
			if (_autorisatieMgr.IsGavPersoon(verzekering.Persoon.ID))
			{
				return _persoonsVerzekeringenDao.Bewaren(verzekering,
				                                  pv => pv.Persoon.WithoutUpdate(),
				                                  pv => pv.VerzekeringsType.WithoutUpdate());
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}
	}
}
