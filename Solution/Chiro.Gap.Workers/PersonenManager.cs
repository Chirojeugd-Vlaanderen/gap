// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. personen bevat
	/// </summary>
	public class PersonenManager
	{
		private IPersonenDao _dao;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Creëert een PersonenManager
		/// </summary>
		/// <param name="dao">Repository voor personen</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		public PersonenManager(IPersonenDao dao, IAutorisatieManager autorisatieMgr)
		{
			_dao = dao;
			_autorisatieMgr = autorisatieMgr;
		}

		/// <summary>
		/// Haalt personen op die een adres gemeen hebben met de 
		/// GelieerdePersoon
		/// met gegeven ID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van 
		/// GelieerdePersoon waarvan huisgenoten
		/// gevraagd zijn</param>
		/// <returns>Lijstje met personen</returns>
		/// <remarks>Parameter: GelieerdePersoonID, return value: Personen!</remarks>
		public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				// Haal alle huisgenoten op

				IList<Persoon> allen = _dao.HuisGenotenOphalen(gelieerdePersoonID);

				// Welke mag ik zien?

				IList<int> selectie = _autorisatieMgr.EnkelMijnPersonen(
					(from p in allen select p.ID).ToList());

				// Enkel de geselecteerde personen afleveren.

				var resultaat = from p in allen
								where selectie.Contains(p.ID)
								select p;

				return resultaat.ToList();
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Persoon, Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
		/// </summary>
		/// <param name="verhuizer">Te verhuizen GelieerdePersoon</param>
		/// <param name="oudAdres">Oud adres</param>
		/// <param name="nieuwAdres">Nieuw adres</param>
		/// <param name="adresType">Adrestype voor de nieuwe koppeling persoon-adres</param>
		/// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
		/// zal hij of zij ook niet verhuizen</remarks>
		public void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
		{
			if (_autorisatieMgr.IsGavPersoon(verhuizer.ID))
			{
				PersoonsAdres persoonsadres
					= (from PersoonsAdres pa in verhuizer.PersoonsAdres
					   where pa.Adres.ID == oudAdres.ID
					   select pa).FirstOrDefault();

				if (oudAdres.PersoonsAdres != null)
				{
					oudAdres.PersoonsAdres.Remove(persoonsadres);
				}

				persoonsadres.AdresType = adresType;
				persoonsadres.Adres = nieuwAdres;

				if (nieuwAdres.PersoonsAdres != null)
				{
					nieuwAdres.PersoonsAdres.Add(persoonsadres);
				}
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Persoon, Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Koppelt het gegeven Adres via een nieuw PersoonsAdresObject
		/// aan de gegeven GelieerdePersoon.  Persisteert niet.
		/// </summary>
		/// <param name="p">GelieerdePersoon die er een adres bij krijgt</param>
		/// <param name="adres">Toe te voegen adres</param>
		/// <param name="adrestype">Het adrestype (thuis, kot, enz.)</param>
		public void AdresToevoegen(Persoon p, Adres adres, AdresTypeEnum adrestype)
		{
			if (_autorisatieMgr.IsGavPersoon(p.ID))
			{
				PersoonsAdres pa = new PersoonsAdres { Adres = adres, Persoon = p, AdresType = adrestype };
				p.PersoonsAdres.Add(pa);
				adres.PersoonsAdres.Add(pa);
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Persoon, Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Een collectie personen ophalen van wie de ID's opgegeven zijn
		/// </summary>
		/// <param name="personenIDs">De ID's van de personen die in de collectie moeten zitten</param>
		/// <returns>Een collectie met de gevraagde personen</returns>
		public IList<Persoon> LijstOphalen(List<int> personenIDs)
		{
			AutorisatieManager authMgr = Factory.Maak<AutorisatieManager>();

			return _dao.LijstOphalen(authMgr.EnkelMijnPersonen(personenIDs));
		}
	}
}
