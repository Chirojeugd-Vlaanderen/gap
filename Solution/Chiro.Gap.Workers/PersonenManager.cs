// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Linq.Expressions;
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
		/// <param name="oudAdres">Oud adres, met personen gekoppeld</param>
		/// <param name="nieuwAdres">Nieuw adres, met personen gekoppeld</param>
		/// <param name="adresType">Adrestype voor de nieuwe koppeling persoon-adres</param>
		/// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
		/// zal hij of zij ook niet verhuizen</remarks>
		public void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
		{
			Verhuizen(new Persoon[] { verhuizer }, oudAdres, nieuwAdres, adresType);
		}

		/// <summary>
		/// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
		/// </summary>
		/// <param name="verhuizers">Te verhuizen personen</param>
		/// <param name="oudAdres">Oud adres, met personen gekoppeld</param>
		/// <param name="nieuwAdres">Nieuw adres, met personen gekoppeld</param>
		/// <param name="adresType">Adrestype voor de nieuwe koppeling persoon-adres</param>
		/// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
		/// zal hij of zij ook niet verhuizen</remarks>
		public void Verhuizen(IEnumerable<Persoon> verhuizers, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
		{
			var persIDs = (from p in verhuizers
				       select p.ID).ToArray();
			var mijnPersIDs = _autorisatieMgr.EnkelMijnPersonen(persIDs);

			if (persIDs.Count() == mijnPersIDs.Count())
			{
				// Vind personen waarvan het adres al gekoppeld is.

				var bestaand = verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == nieuwAdres.ID));

				if (bestaand.FirstOrDefault() != null)
				{
					// Geef een exception met daarin de persoonsadresobjecten die al bestaan

					throw new BlokkerendeObjectenException<BestaatAlFoutCode, PersoonsAdres>(
						BestaatAlFoutCode.PersoonBestaatAl,
						bestaand.ToArray());
				}

				var oudePersoonsAdressen = verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == oudAdres.ID));

				foreach (var pa in oudePersoonsAdressen)
				{
					// verwijder koppeling oud adres->persoonsadres

					pa.Adres.PersoonsAdres.Remove(pa);

					// adrestype
					
					pa.AdresType = adresType;

					// koppel persoonsadres aan nieuw adres

					pa.Adres = nieuwAdres;

					nieuwAdres.PersoonsAdres.Add(pa);
				}
			}
			else
			{
				// Minstens een persoon waarvoor de user geen GAV is.  Zo'n gepruts verdient
				// een onverbiddellijke geen-gav-exception.

				throw new GeenGavException(GeenGavFoutCode.Persoon, Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Koppelt het gegeven Adres via een nieuw PersoonsAdresObject
		/// aan de gegeven Persoon.  Persisteert niet.
		/// </summary>
		/// <param name="p">Persoon die er een adres bij krijgt, met daaraan gekoppeld zijn huidige
		/// adressen.</param>
		/// <param name="adres">Toe te voegen adres</param>
		/// <param name="adrestype">Het adrestype (thuis, kot, enz.)</param>
		public void AdresToevoegen(Persoon p, Adres adres, AdresTypeEnum adresType)
		{
			AdresToevoegen(new List<Persoon> { p }, adres, adresType);
		}

		/// <summary>
		/// Koppelt het gegeven Adres via nieuwe PersoonsAdresObjecten
		/// aan de gegeven Personen.  Persisteert niet.
		/// </summary>
		/// <param name="personen">Personen die er een adres bij krijgen, met daaraan gekoppeld hun huidige
		/// adressen.</param>
		/// <param name="adres">Toe te voegen adres</param>
		/// <param name="adrestype">Het adrestype (thuis, kot, enz.)</param>
		public void AdresToevoegen(IEnumerable<Persoon> personen, Adres adres, AdresTypeEnum adrestype)
		{
			var persIDs = (from p in personen
				       select p.ID).ToList();
			var mijnPersIDs = _autorisatieMgr.EnkelMijnPersonen(persIDs);

			if (persIDs.Count() == mijnPersIDs.Count())
			{
				// Vind personen waaraan het adres al gekoppeld is.

				var bestaand = personen.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == adres.ID));

				if (bestaand.FirstOrDefault() != null)
				{
					// Sommige personen hebben het adres al.  Geef een exception met daarin de
					// betreffende persoonsadres-objecten.

					var bestaandePersoonsAdressen = bestaand.ToList();

					throw new BlokkerendeObjectenException<BestaatAlFoutCode, PersoonsAdres>(
						BestaatAlFoutCode.PersoonsAdresBestaatAl, 
						bestaandePersoonsAdressen);
				}

				foreach (Persoon p in personen)
				{
					PersoonsAdres pa = new PersoonsAdres { Adres = adres, Persoon = p, AdresType = adrestype };
					p.PersoonsAdres.Add(pa);
					adres.PersoonsAdres.Add(pa);
				}
			}
			else
			{
				// stiekem personen niet gelieerd aan eigen groep bij in lijst opgenomen.  Geen
				// tijd aan verspillen; gewoon een GeenGavException.

				throw new GeenGavException(GeenGavFoutCode.Persoon, Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Een collectie personen ophalen van wie de ID's opgegeven zijn
		/// </summary>
		/// <param name="personenIDs">De ID's van de personen die in de collectie moeten zitten</param>
		/// <param name="extras">Geeft aan welke gerelateerde entiteiten mee opgehaald moeten worden</param>
		/// <returns>Een collectie met de gevraagde personen</returns>
		public IList<Persoon> LijstOphalen(IEnumerable<int> personenIDs, PersoonsExtras extras)
		{
			AutorisatieManager authMgr = Factory.Maak<AutorisatieManager>();

			var paths = new List<Expression<Func<Persoon, object>>>();

			if ((extras & PersoonsExtras.Adressen) != 0)
			{
				paths.Add(p => p.PersoonsAdres.First().Adres);
			}

			return _dao.Ophalen(authMgr.EnkelMijnPersonen(personenIDs), paths.ToArray());
		}
	}
}
