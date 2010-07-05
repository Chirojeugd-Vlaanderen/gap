// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. personen bevat
	/// </summary>
	public class PersonenManager
	{
		private readonly IPersonenDao _dao;
		private readonly IAutorisatieManager _autorisatieMgr;

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
				throw new GeenGavException(Properties.Resources.GeenGav);
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

					throw new BlokkerendeObjectenException<PersoonsAdres>(
						bestaand,
						bestaand.Count(),
						Properties.Resources.WonenDaarAl);
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

				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Koppelt het gegeven Adres via nieuwe PersoonsAdresObjecten
		/// aan de gegeven Personen.  Persisteert niet.
		/// </summary>
		/// <param name="personen">Personen die er een adres bij krijgen, met daaraan gekoppeld hun huidige
		/// adressen, en de gelieerde personen waarop de gebruiker GAV-rechten heeft.</param>
		/// <param name="adres">Toe te voegen adres</param>
		/// <param name="adrestype">Het adrestype (thuis, kot, enz.)</param>
		/// <remarks>Gebruik GelieerdePersonenManager.AdresToevoegen, zodat bij een eerste adres het standaardadres
		/// ook in orde komt voor gelieerde personen van andere groepen.</remarks>
		[Obsolete]
		public void AdresToevoegen(IEnumerable<Persoon> personen, Adres adres, AdresTypeEnum adrestype)
		{
			var persIDs = (from p in personen
						   select p.ID).ToList();
			var mijnPersIDs = _autorisatieMgr.EnkelMijnPersonen(persIDs);

			if (persIDs.Count() != mijnPersIDs.Count())
			{
				// stiekem personen niet gelieerd aan eigen groep bij in lijst opgenomen.  Geen
				// tijd aan verspillen; gewoon een GeenGavException.

				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Vind personen waaraan het adres al gekoppeld is.

			var bestaand = personen.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == adres.ID));

			if (bestaand.FirstOrDefault() != null)
			{
				// Sommige personen hebben het adres al.  Geef een exception met daarin de
				// betreffende persoonsadres-objecten.

				var bestaandePersoonsAdressen = bestaand.ToList();

				throw new BlokkerendeObjectenException<PersoonsAdres>(
					bestaand,
					bestaand.Count(),
					Properties.Resources.WonenDaarAl);
			}

			// En dan nu het echte werk:
			foreach (Persoon p in personen)
			{
				// Maak PersoonsAdres dat het adres aan de persoon koppelt.

				var pa = new PersoonsAdres { Adres = adres, Persoon = p, AdresType = adrestype };
				p.PersoonsAdres.Add(pa);
				adres.PersoonsAdres.Add(pa);
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
			var paths = new List<Expression<Func<Persoon, object>>>();

			if ((extras & PersoonsExtras.Adressen) != 0)
			{
				paths.Add(p => p.PersoonsAdres.First().Adres);
			}

			if ((extras & PersoonsExtras.Groep) != 0)
			{
				paths.Add(p => p.GelieerdePersoon.First().Groep);
			}
			else if ((extras & PersoonsExtras.AlleGelieerdePersonen) != 0)
			{
				paths.Add(p => p.GelieerdePersoon);
			}

			// TODO: dit is nogal veel dubbel werk.  EnkelMijnPersonen laadt alle gelieerde personen,
			// om te kijken welke personen overeen komen met 'mijn' personen.  Daarna worden, indien
			// 'extras|PersoonsExtras.MijnGelieerdePersonen' gezet is, nog eens dezelfde gelieerde
			// persoonsextra's opgehaald.
			return _dao.Ophalen(
				_autorisatieMgr.EnkelMijnPersonen(personenIDs),
				paths.ToArray());
		}

		/// <summary>
		/// Haalt een lijst op van personen, op basis van een lijst <paramref name="gelieerdePersoonIDs"/>.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van *GELIEERDE* personen, waarvan de corresponderende persoonsobjecten
		/// opgehaald moeten worden.</param>
		/// <param name="extras">Bepaalt welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>De gevraagde personen</returns>
		public IEnumerable<Persoon> LijstOphalenViaGelieerdePersoon(IEnumerable<int> gelieerdePersoonIDs, PersoonsExtras extras)
		{
			var paths = new List<Expression<Func<Persoon, object>>>();

			if ((extras & PersoonsExtras.Adressen) != 0)
			{
				paths.Add(p => p.PersoonsAdres.First().Adres);
			}

			if ((extras & PersoonsExtras.Groep) != 0)
			{
				paths.Add(p => p.GelieerdePersoon.First().Groep);
			}
			else if ((extras & PersoonsExtras.AlleGelieerdePersonen) != 0)
			{
				paths.Add(p => p.GelieerdePersoon);
			}

			return _dao.OphalenViaGelieerdePersoon(gelieerdePersoonIDs, paths.ToArray());
		}
	}
}
