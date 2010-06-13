// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.KipSync;

using Adres = Chiro.Gap.Orm.Adres;
using AdresTypeEnum = Chiro.Gap.Domain.AdresTypeEnum;
using CommunicatieType = Chiro.Gap.Orm.CommunicatieType;
using Persoon = Chiro.Gap.Orm.Persoon;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. gelieerde personen bevat
	/// </summary>
	public class GelieerdePersonenManager
	{
		private readonly IGelieerdePersonenDao _gelieerdePersonenDao;
		private readonly IGroepenDao _groepenDao;
		private readonly ICategorieenDao _categorieenDao;
		private readonly IPersonenDao _personenDao;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly ISyncPersoonService _sync;

		/// <summary>
		/// Creëert een GelieerdePersonenManager
		/// </summary>
		/// <param name="gelieerdePersonenDao">Repository voor gelieerde personen</param>
		/// <param name="groepenDao">Repository voor groepen</param>
		/// <param name="categorieenDao">Repository voor categorieën</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		/// <param name="typedao">Repository voor communicatietypes</param>
		/// <param name="commdao">Repository voor communicatievormen</param>
		/// <param name="pDao">Repository voor personen</param>
		/// <param name="sync">Sync Service met KipAdmin</param>
		public GelieerdePersonenManager(
			IGelieerdePersonenDao gelieerdePersonenDao,
			IGroepenDao groepenDao,
			ICategorieenDao categorieenDao,
			IAutorisatieManager autorisatieMgr,
			IDao<CommunicatieType> typedao,
			IDao<CommunicatieVorm> commdao,
			IPersonenDao pDao,
            ISyncPersoonService sync)
		{
			_gelieerdePersonenDao = gelieerdePersonenDao;
			_groepenDao = groepenDao;
			_categorieenDao = categorieenDao;
			_autorisatieMgr = autorisatieMgr;
			_personenDao = pDao;
		    _sync = sync;
		}

		#region proxy naar data access

		// Wel altijd rekening houden met authorisatie!

		/// <summary>
		/// Haalt gelieerde persoon met gekoppelde persoon op.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon.</param>
		/// <returns>Gelieerde persoon, met gekoppeld persoonsobject.</returns>
		/// <remarks>De groepsinfo wordt niet mee opgehaald, omdat we die in de
		/// meeste gevallen niet nodig zullen hebben.</remarks>
		public GelieerdePersoon Ophalen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				// ENKEL informatie gelieerde persoon en persoon ophalen.
				// Als je ook informatie wil van de groep, maak dan een aparte
				// method, of gebruik een lambda-expressie
				//
				// In de meeste gevallen zullen we bij het opvragen van een
				// gelieerde persoon de groepsinfo niet nodig hebben, aangezien
				// er aan die groepsinfo toch niets zal wijzigen.  Vandaar dat
				// die groepsinfo hier niet mee komt.

				return _gelieerdePersonenDao.Ophalen(gelieerdePersoonID, foo => foo.Persoon);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een gelieerde persoon op, met de gevraagde 'extra's'.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID op te halen gelieerde persoon</param>
		/// <param name="extras">geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
		/// <returns>De gevraagde gelieerde persoon, met de gevraagde gekoppelde entiteiten.</returns>
		public GelieerdePersoon Ophalen(int gelieerdePersoonID, PersoonsExtras extras)
		{
			return Ophalen(new List<int> {gelieerdePersoonID}, extras).FirstOrDefault();
		}

		/// <summary>
		/// Een gelieerde persoon ophalen met al zijn/haar communicatievormen
		/// </summary>
		/// <param name="gelieerdePersoonID">De ID van de gelieerde persoon</param>
		/// <returns>Een gelieerde persoon met al zijn/haar communicatievormen</returns>
		public GelieerdePersoon OphalenMetCommVormen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				return _gelieerdePersonenDao.Ophalen(gelieerdePersoonID, 
					foo => foo.Persoon, 
					foo => foo.Groep, 
					foo => foo.Communicatie,
					foo => foo.Communicatie.First().CommunicatieType);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een lijst op van gelieerde personen.
		/// </summary>
		/// <param name="gelieerdePersonenIDs">ID's van de op te vragen
		/// gelieerde personen.</param>
		/// <returns>Lijst met gelieerde personen</returns>
		public IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs)
		{
			return _gelieerdePersonenDao.Ophalen(_autorisatieMgr.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs));
		}

		/// <summary>
		/// Haalt gelieerde persoon op met persoonsgegevens, adressen en
		/// communicatievormen.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID gevraagde gelieerde persoon</param>
		/// <returns>GelieerdePersoon met persoonsgegevens, adressen, categorieën, lidgegevens en 
		/// communicatievormen.</returns>
		public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				return _gelieerdePersonenDao.DetailsOphalen(gelieerdePersoonID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Bewaart een gelieerde persoon, zonder koppelingen
		/// </summary>
		/// <param name="p">Te bewaren gelieerde persoon</param>
		/// <returns>De bewaarde gelieerde persoon</returns>
		public GelieerdePersoon Bewaren(GelieerdePersoon p)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(p.ID))
			{
				// Hier mapping gebruiken om te vermijden dat het AD-nummer
				// overschreven wordt, lijkt me wat overkill.  Ik vergelijk
				// hiet nieuwe AD-nummer gewoon met het bestaande.

				// Er mag niet gepoterd worden met PersoonID en AdNummer

				var origineel = _gelieerdePersonenDao.Ophalen(p.ID, foo => foo.Persoon);

				if (origineel == null || origineel.Persoon.AdNummer == p.Persoon.AdNummer)
				{
                    // TODO: De transacties aanschakelen, nu gaat dat niet omdat we de in een werkgroep zitten

                    //using (var tx = new TransactionScope())
                    //{
                    var q = _gelieerdePersonenDao.Bewaren(p);

                    // Map to KipSync and send
                    AutoMapper.Mapper.CreateMap<Persoon, KipSync.Persoon>();
                    var syncPersoon = AutoMapper.Mapper.Map<Persoon, KipSync.Persoon>(q.Persoon);
				    
                    //Debug.WriteLine(syncPersoon);

                    _sync.PersoonUpdated(syncPersoon);
                    
                    return q;
                    //}
				}
				else
				{
					throw new InvalidOperationException(Properties.Resources.AdNummerNietWijzigen);
				}
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public GelieerdePersoon BewarenMetCommVormen(GelieerdePersoon p)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(p.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			// Hier mapping gebruiken om te vermijden dat het AD-nummer
			// overschreven wordt, lijkt me wat overkill.  Ik vergelijk
			// hiet nieuwe AD-nummer gewoon met het bestaande.

			GelieerdePersoon origineel = _gelieerdePersonenDao.Ophalen(p.ID, e => e.Persoon, e => e.Communicatie.First().CommunicatieType);
			if (origineel.Persoon.AdNummer == p.Persoon.AdNummer)
			{
				return _gelieerdePersonenDao.Bewaren(p, e => e.Persoon, e => e.Communicatie.First().CommunicatieType.WithoutUpdate());
			}
			else
			{
				throw new InvalidOperationException(Properties.Resources.AdNummerNietWijzigen);
			}
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="gp"></param>
		/// <returns></returns>
		public GelieerdePersoon BewarenMetPersoonsAdressen(GelieerdePersoon gp)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			// Hier mapping gebruiken om te vermijden dat het AD-nummer
			// overschreven wordt, lijkt me wat overkill.  Ik vergelijk
			// hiet nieuwe AD-nummer gewoon met het bestaande.

			GelieerdePersoon origineel = _gelieerdePersonenDao.Ophalen(gp.ID, e => e.Persoon);
			if (origineel.Persoon.AdNummer == gp.Persoon.AdNummer)
			{
				return _gelieerdePersonenDao.Bewaren(gp, e => e.Persoon, e => e.PersoonsAdres, e => e.Persoon.PersoonsAdres.First());
			}
			else
			{
				throw new InvalidOperationException(Properties.Resources.AdNummerNietWijzigen);
			}
		}

		/// <summary>
		/// Haal een lijst op met alle gelieerde personen van een groep, inclusief persoons- en lidinfo.
		/// </summary>
		/// <param name="groepID">GroepID van gevraagde groep</param>
		/// <returns>Lijst met alle gelieerde personen, inclusief persoons- en lidinfo</returns>
		/// <remarks>Opgelet! Dit kan een zware query zijn!</remarks>
		public IList<GelieerdePersoon> AllenOphalen(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _gelieerdePersonenDao.AllenOphalen(groepID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haal een pagina op met gelieerde personen van een groep.
		/// </summary>
		/// <param name="groepID">GroepID gevraagde groep</param>
		/// <param name="pagina">Paginanummer (>=1)</param>
		/// <param name="paginaGrootte">Grootte van de pagina's</param>
		/// <param name="aantalTotaal">Totaal aantal personen in de groep</param>
		/// <returns>Lijst met een pagina aan gelieerde personen.</returns>
		public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				IList<GelieerdePersoon> list = _gelieerdePersonenDao.PaginaOphalen(groepID, e => e.Groep.ID, pagina, paginaGrootte, out aantalTotaal);
				list.OrderBy(e => e.Persoon.Naam).ThenBy(e => e.Persoon.VoorNaam);
				return list;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een pagina op met gelieerde personen van een groep,
		/// inclusief eventuele lidobjecten voor deze groep
		/// </summary>
		/// <param name="groepID">GroepID gevraagde groep</param>
		/// <param name="pagina">Paginanummer (>=1)</param>
		/// <param name="paginaGrootte">Aantal personen per pagina</param>
		/// <param name="aantalTotaal">Outputparameter voor totaal aantal
		/// personen in de groep</param>
		/// <returns>Lijst met GelieerdePersonen</returns>
		public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _gelieerdePersonenDao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een pagina op met gelieerde personen van een groep die tot de categorie behoren,
		/// inclusief eventuele lidobjecten voor deze groep
		/// </summary>
		/// <param name="categorieID">ID gevraagde categorie</param>
		/// <param name="pagina">Paginanummer (minstens 1)</param>
		/// <param name="paginaGrootte">Aantal personen per pagina</param>
		/// <param name="aantalTotaal">Outputparameter voor totaal aantal
		/// personen in de groep</param>
		/// <returns>Lijst met GelieerdePersonen</returns>
		public IList<GelieerdePersoon> PaginaOphalenMetLidInfoVolgensCategorie(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			if (_autorisatieMgr.IsGavCategorie(categorieID))
			{
				return _gelieerdePersonenDao.PaginaOphalenMetLidInfoVolgensCategorie(categorieID, pagina, paginaGrootte, out aantalTotaal);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Koppelt het relevante groepsobject aan de gegeven
		/// gelieerde persoon.
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <returns>Diezelfde gelieerde persoon, met zijn of haar groep 
		/// eraan gekoppeld.</returns>
		public GelieerdePersoon GroepLaden(GelieerdePersoon gp)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				return _gelieerdePersonenDao.GroepLaden(gp);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">Te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
		/// <returns>Lijst met gevonden matches</returns>
		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _gelieerdePersonenDao.ZoekenOpNaamOngeveer(groepID, naam, voornaam);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		#endregion

		/// <summary>
		/// Maak een GelieerdePersoon voor gegeven persoon en groep
		/// </summary>
		/// <param name="persoon">Te liëren persoon</param>
		/// <param name="groep">Groep waaraan te liëren</param>
		/// <param name="chiroLeeftijd">Chiroleeftijd gelieerde persoon</param>
		/// <returns>Een nieuwe GelieerdePersoon</returns>
		public GelieerdePersoon Koppelen(Persoon persoon, Groep groep, int chiroLeeftijd)
		{
			if (_autorisatieMgr.IsGavGroep(groep.ID))
			{
				var resultaat = new GelieerdePersoon();

				resultaat.Persoon = persoon;
				resultaat.Groep = groep;
				resultaat.ChiroLeefTijd = chiroLeeftijd;

				persoon.GelieerdePersoon.Add(resultaat);
				groep.GelieerdePersoon.Add(resultaat);

				return resultaat;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Maakt GelieerdePersoon, gekoppelde Persoon, Adressen en Communicatie allemaal
		/// te verwijderen.  Persisteert!
		/// </summary>
		/// <param name="gp">Te verwijderen gelieerde persoon</param>
		public void VolledigVerwijderen(GelieerdePersoon gp)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				gp.TeVerwijderen = true;
				gp.Persoon.TeVerwijderen = true;

				foreach (PersoonsAdres pa in gp.Persoon.PersoonsAdres)
				{
					pa.TeVerwijderen = true;
				}

				foreach (CommunicatieVorm cv in gp.Communicatie)
				{
					cv.TeVerwijderen = true;
				}

				_gelieerdePersonenDao.Bewaren(gp, gpers => gpers.Persoon.PersoonsAdres, gpers => gpers.Communicatie);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="personenLijst"></param>
		public void Bewaren(IList<GelieerdePersoon> personenLijst)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="gelieerdePersoonID"></param>
		/// <returns></returns>
		public bool IsLid(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Koppelt een gelieerde persoon aan een categorie, en persisteert dan de aanpassingen
		/// </summary>
		/// <param name="gelieerdePersonen">Te koppelen gelieerde persoon</param>
		/// <param name="c">Te koppelen categorie</param>
		public void CategorieKoppelen(IList<GelieerdePersoon> gelieerdePersonen, Categorie c)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?
			if (gelieerdePersonen.Any(x => !_autorisatieMgr.IsGavGelieerdePersoon(x.ID)))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (!_autorisatieMgr.IsGavCategorie(c.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			foreach (GelieerdePersoon x in gelieerdePersonen)
			{
				if (!x.Groep.Equals(c.Groep))
				{
					throw new FoutNummerException(
						FoutNummer.CategorieNietVanGroep, 
						Properties.Resources.FoutieveGroepCategorie);
				}
				x.Categorie.Add(c);
				c.GelieerdePersoon.Add(x);
			}
		}

		/// <summary>
		/// Verwijdert de gelieerde personen uit de categorie, en persisteert
		/// </summary>
		/// <remarks>De methode is reentrant, als er bepaalde personen niet gelinkt zijn aan de categorie, 
		/// gebeurt er niets met die personen, ook geen error.
		/// </remarks>
		/// <param name="gelieerdePersonenIDs">Gelieerde persoon IDs</param>
		/// <param name="categorie">Te verwijderen categorie MET gelinkte gelieerdepersonen </param>
		/// <returns>Een kloon van de categorie, waaruit de gevraagde personen verwijderd zijn</returns>
		public Categorie CategorieLoskoppelen(IList<int> gelieerdePersonenIDs, Categorie categorie)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?
			if (gelieerdePersonenIDs.Any(x => !_autorisatieMgr.IsGavGelieerdePersoon(x)))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (!_autorisatieMgr.IsGavCategorie(categorie.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			IList<GelieerdePersoon> gel =
					(from gp in categorie.GelieerdePersoon
					 where gelieerdePersonenIDs.Contains(gp.ID)
					 select gp).ToList();

			foreach (GelieerdePersoon gp in gel)
			{
				gp.TeVerwijderen = true;
			}

			return _categorieenDao.Bewaren(categorie, cat => cat.GelieerdePersoon);
		}

		/// <summary>
		/// Een categorie ophalen op basis van de opgegeven ID
		/// </summary>
		/// <param name="catID">De ID van de categorie die je nodig hebt</param>
		/// <returns>De categorie met de opgegeven ID</returns>
		public Categorie OphalenCategorie(int catID)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?

			if (!_autorisatieMgr.IsGavCategorie(catID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			return _categorieenDao.Ophalen(catID);
		}

		/// <summary>
		/// Zoekt naar gelieerde personen die gelijkaardig zijn aan een gegeven
		/// <paramref name="persoon"/>.
		/// </summary>
		/// <param name="persoon">Persoon waarmee vergeleken moet worden</param>
		/// <param name="groepID">ID van groep waarin te zoeken</param>
		/// <returns>Lijstje met gelijkaardige personen</returns>
		public IList<GelieerdePersoon> ZoekGelijkaardig(Persoon persoon, int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				// Momenteel wordt er enkel op 'naam ongeveer' gezocht, maar
				// ik kan me voorstellen dat deze functie in de toekomst wat
				// gesofisticeerder wordt.

				return _gelieerdePersonenDao.ZoekenOpNaamOngeveer(groepID, persoon.Naam, persoon.VoorNaam);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een rij gelieerde personen op, eventueel met extra info
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's op te halen gelieerde personen</param>
		/// <param name="extras">geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
		/// <returns>De gevraagde rij gelieerde personen.  De personen komen sowieso mee.</returns>
		public IEnumerable<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersoonIDs, PersoonsExtras extras)
		{
			var paths = new List<Expression<Func<GelieerdePersoon, object>>>();

			paths.Add(gp => gp.Persoon);

			if ((extras & PersoonsExtras.Adressen) != 0)
			{
				paths.Add(gp => gp.Persoon.PersoonsAdres.First().Adres);
			}

			if ((extras & PersoonsExtras.Groep) != 0)
			{
				paths.Add(gp => gp.Groep);
			}

			return _gelieerdePersonenDao.Ophalen(_autorisatieMgr.EnkelMijnGelieerdePersonen(gelieerdePersoonIDs), paths.ToArray());
		}

		/// <summary>
		/// Maakt het persoonsAdres <paramref name="voorkeur"/> het voorkeursadres van de gelieerde persoon
		/// <paramref name="gp"/>
		/// </summary>
		/// <param name="gp">Gelieerde persoon die een nieuw voorkeursadres moet krijgen</param>
		/// <param name="voorkeur">Persoonsadres dat voorkeursadres moet worden van <paramref name="gp"/>.</param>
		public void VoorkeurInstellen(GelieerdePersoon gp, PersoonsAdres voorkeur)
		{
			VoorkeurInstellen(gp, voorkeur, true);
		}

		/// <summary>
		/// Maakt het persoonsAdres <paramref name="voorkeur"/> het voorkeursadres van de gelieerde persoon
		/// <paramref name="gp"/>
		/// </summary>
		/// <param name="gp">Gelieerde persoon die een nieuw voorkeursadres moet krijgen</param>
		/// <param name="voorkeur">Persoonsadres dat voorkeursadres moet worden van <paramref name="gp"/>.</param>
		/// <param name="checkGav">Indien <paramref name="checkGav"/> <c>false</c> is, mag je het voorkeursadres
		/// van een gelieerde persoon ook wijzigen als je geen GAV bent van die gelieerde persoon.  (Je moet altijd
		/// sowieso GAV zijn van het persoonsadres.)  Dat is nodig in uitzonderlijke gevallen,
		/// bijv. als je iemand een eerste adres geeft, moet dat ook het voorkeursadres worden van de gelieerde personen
		/// in andere groepen, ook al ben je geen GAV van deze groepen.</param>
		/// <remarks>Deze method is private, en dat moet zo blijven.  Het is niet de bedoeling dat het gemakkelijk is
		/// om zonder GAV-rechten een adresvoorkeur te veranderen.</remarks>
		private void VoorkeurInstellen(GelieerdePersoon gp, PersoonsAdres voorkeur, bool checkGav)
		{
			Debug.Assert(gp.Persoon != null);
			Debug.Assert(voorkeur.Persoon != null);

			if (checkGav && !_autorisatieMgr.IsGavGelieerdePersoon(gp.ID) || !_autorisatieMgr.IsGavPersoonsAdres(voorkeur.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Kijk na of gp en voorkeur wel betrekking hebben op dezelfde persoon.

			if (gp.Persoon.ID != voorkeur.Persoon.ID)
			{
				throw new InvalidOperationException(Properties.Resources.PersonenKomenNietOvereen);
			}

			if (gp.PersoonsAdres != null)
			{
				// Als het huidige voorkeursadres van de gelieerde persoon gegeven is
				// verwijder dan de gelieerde persoon
				// uit de collectie gelieerde personen die dat voorkeursadres hebben.

				gp.PersoonsAdres.GelieerdePersoon.Remove(gp);

				// noot: Aangezien we identity en equality niet goed geimplementeerd hebben,
				// kunnen we de check of het voorkeursadres mogelijk het bestaande adres is,
				// hier niet betrouwbaar uitvoeren.  Nogal dikwijls werken we met de ID's, 
				// maar dat kan hier niet, omdat in pratkijk een nieuw adres ook het voorkeuradres
				// kan zijn.  (Nieuwe adressen hebben geen geldig ID.)
			}

			gp.PersoonsAdres = voorkeur;
		}

		/// <summary>
		/// Koppelt het gegeven Adres via nieuwe PersoonsAdresObjecten
		/// aan de Personen gekoppeld aan de gelieerde personen <paramref name="gelieerdePersonen"/>.  
		/// Persisteert niet.
		/// </summary>
		/// <param name="gelieerdePersonen">Gelieerde  die er een adres bij krijgen, met daaraan gekoppeld hun huidige
		/// adressen, en de gelieerde personen waarop de gebruiker GAV-rechten heeft.</param>
		/// <param name="adres">Toe te voegen adres</param>
		/// <param name="adrestype">Het adrestype (thuis, kot, enz.)</param>
		/// <param name="voorkeur">Indien true, wordt het nieuwe adres voorkeursadres van de gegeven gelieerde personen</param>
		/// <remarks>TODO: Dit lijkt nog te hard op AdresToevoegen in PersonenManager.</remarks>
		public void AdresToevoegen(IEnumerable<GelieerdePersoon> gelieerdePersonen, Adres adres, AdresTypeEnum adrestype, bool voorkeur)
		{
			var gpersIDs = (from p in gelieerdePersonen
				       select p.ID).ToList();
			var mijngPersIDs = _autorisatieMgr.EnkelMijnGelieerdePersonen(gpersIDs);

			if (gpersIDs.Count() != mijngPersIDs.Count())
			{
				// stiekem personen niet gelieerd aan eigen groep bij in lijst opgenomen.  Geen
				// tijd aan verspillen; gewoon een GeenGavException.

				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Vind personen waaraan het adres al gekoppeld is.
			// (We hebben chance dat we hier in praktijk nooit komen met een nieuw adres, anders
			// zou onderstaande problemen geven.)

			var bestaand = gelieerdePersonen.Select(gp=>gp.Persoon).SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == adres.ID));

			if (bestaand.FirstOrDefault() != null)
			{
				// Sommige personen hebben het adres al.  Geef een exception met daarin de
				// betreffende persoonsadres-objecten.

				throw new BlokkerendeObjectenException<PersoonsAdres>(
					bestaand,
					bestaand.Count(),
					Properties.Resources.WonenDaarAl);
			}

			// En dan nu het echte werk:
			foreach (var gelieerdePersoon in gelieerdePersonen)
			{
				// Maak PersoonsAdres dat het adres aan de persoon koppelt.

				var pa = new PersoonsAdres { Adres = adres, Persoon = gelieerdePersoon.Persoon, AdresType = adrestype };
				gelieerdePersoon.Persoon.PersoonsAdres.Add(pa);
				adres.PersoonsAdres.Add(pa);

				if (gelieerdePersoon.Persoon.PersoonsAdres.Count() == 1)
				{
					// Eerste adres van de gelieerde persoon.  Dit moet bij elke gelieerde persoon het voorkeursadres
					// worden.

					foreach (var gp2 in gelieerdePersoon.Persoon.GelieerdePersoon)
					{
						VoorkeurInstellen(gp2, pa, false);
						// De extra parameter 'false' laat toe het voorkeursadres te wijzigen van
						// een gelieerde persoon waarvoor je geen GAV bent.
					}
				}
                                else if (voorkeur)
				{
					VoorkeurInstellen(gelieerdePersoon, pa);
				}
			}
		}

		/// <summary>
		/// Haalt gelieerde personen op die een adres gemeen hebben met de 
		/// GelieerdePersoon met gegeven ID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van GelieerdePersoon waarvan huisgenoten
		/// gevraagd zijn</param>
		/// <returns>Lijstje met gelieerde personen</returns>
		/// <remarks>Enkel de huisgenoten die gelieerd zijn aan de groep van de originele gelieerde persoon worden
		/// mee opgehaald, ongeacht of er misschien nog huisgenoten zijn in een andere groep waar de gebruiker ook
		/// GAV-rechten op heeft.</remarks>
		public IList<GelieerdePersoon> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				// Haal alle huisgenoten op

				IList<GelieerdePersoon> resultaat = _gelieerdePersonenDao.HuisGenotenOphalenZelfdeGroep(gelieerdePersoonID);

				return resultaat.ToList();
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Verwijder persoonsadressen, en persisteer.  Als ergens een voorkeuradres wegvalt, dan wordt een willekeurig
		/// ander adres voorkeuradres van de gelieerde persoon.
		/// </summary>
		/// <param name="persoonsAdressen">Te verwijderen persoonsadressen</param>
		/// <remarks>Deze method staat wat vreemd onder GelieerdePersonenManager, maar past wel voorkeursadressen
		/// van gelieerde personen aan.</remarks>
		public void AdresVerwijderen(IEnumerable<PersoonsAdres> persoonsAdressen)
		{
			if (!_autorisatieMgr.IsGavPersoonsAdressen(from pa in persoonsAdressen select pa.ID))
			{
				throw new GeenGavException();
			}

			var personen = from pa in persoonsAdressen select pa.Persoon;
			var gelieerdePersonen = personen.SelectMany(p => p.GelieerdePersoon);

			// overloop te verwijderen persoonsadressen

			foreach (PersoonsAdres pa in persoonsAdressen)
			{
				if (pa.GelieerdePersoon.FirstOrDefault() != null)
				{
					// persoonsadres is voorkeuradres van sommige gelieerde personen.
					// Voor die gelieerde personen moet
					// een nieuw voorkeursadres gekozen worden.  Dit gebeurt willekeurig uit de overige
					// adressen.  Als er geen andere adressen zijn, is er ook geen voorkeuradres meer.

					PersoonsAdres nieuwVoorkeursAdres;

					var alleAdressen = pa.GelieerdePersoon.First().Persoon.PersoonsAdres;

					// Probeer eerste en laatste adres...

					if (alleAdressen.First().ID != pa.ID)
					{
						nieuwVoorkeursAdres = alleAdressen.First();
					}
					else if (alleAdressen.Last().ID != pa.ID)
					{
						nieuwVoorkeursAdres = alleAdressen.Last();
					}
					else
					{
						// Als zowel eerste als laatste PersoonsAdres het te verwijderen adres is,
						// dan had de persoon maar 1 adres.  Aangezien dat wordt verwijderd, komt er
						// geen voorkeursadres.

						nieuwVoorkeursAdres = null;
					}


					foreach (var pineut in pa.GelieerdePersoon.ToArray())
					{
						if (nieuwVoorkeursAdres == null)
						{
							pineut.PersoonsAdres = null;

							// 'Vergeet' even het voorkeursadres, zodat we geen conflicten krijgen
							// bij het bewaren.
						}
						else
						{
							VoorkeurInstellen(pineut, nieuwVoorkeursAdres, false);	
						}
					}
				}

				pa.TeVerwijderen = true;
			}

			// TODO: Bewaren in 1 transactie

			// bewaar al dan niet aangepaste voorkeursadres
			_gelieerdePersonenDao.Bewaren(gelieerdePersonen, gp => gp.PersoonsAdres);

			// verwijder te verwijderen persoonsadres
			_personenDao.Bewaren(personen, p => p.PersoonsAdres.First().GelieerdePersoon);

		}
	}
}
