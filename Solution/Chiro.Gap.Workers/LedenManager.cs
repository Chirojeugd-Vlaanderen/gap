using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers
{
	public class LedenManager
	{
		private LedenDaoCollectie _daos;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Maakt een nieuwe ledenmanager aan
		/// </summary>
		/// <param name="daos">Een hele reeks van IDao-objecten, nodig
		/// voor data access.</param>
		/// <param name="authorisatie">een IAuthorisatieManager, die
		/// de GAV-permissies van de huidige user controleert.</param>
		public LedenManager(LedenDaoCollectie daos, IAutorisatieManager autorisatie)
		{
			_daos = daos;
			_autorisatieMgr = autorisatie;
		}

		/// <summary>
		/// Doet de basic aanpassingen aan het lid object (alle andere zijn specifiek voor kind of leiding).
		/// </summary>
		/// <param name="lid"></param>
		/// <param name="gp"></param>
		/// <param name="gwj"></param>
		private void LidMaken(Lid lid, GelieerdePersoon gp, GroepsWerkJaar gwj)
		{	
			// GroepsWerkJaar en GelieerdePersoon invullen
			lid.GroepsWerkJaar = gwj;
			lid.GelieerdePersoon = gp;
			gp.Lid.Add(lid);
			gwj.Lid.Add(lid);
		}

		/// <summary>
		/// Doet alle nodige checks om na te kijken dat de gegeven gelieerde persoon lid gemaakt kan worden in het
		/// gegeven groepswerkjaar. In dat geval wordt er true teruggeven
		/// </summary>
		/// <param name="gp"></param>
		/// <param name="gwj"></param>
		/// <param name="gpMetDetails">De gelieerde persoon aangevuld met details</param>
		/// <returns>True als de persoon lid kan worden</returns>
		private bool kanLidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, out GelieerdePersoon gpMetDetails)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}

			// De persoonsgegevens zijn nodig om de afdeling te bepalen.  Haal ze op als ze ontbreken.
			gpMetDetails = gp;
			if (gp.Persoon == null)
			{
				GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();
				gpMetDetails = gpm.DetailsOphalen(gp.ID);
			}

			//CONTROLES
			//Lid bestaat al
			var x = (from l in gpMetDetails.Lid
					 where l.GroepsWerkJaar.ID == gwj.ID
					 select l).FirstOrDefault();

			if (x != null) //was dus al lid
			{
				throw new BestaatAlException();
			}

			//Er zijn afdelingen
			if (gwj.AfdelingsJaar.Count == 0)
			{
				throw new OngeldigeActieException("Je kan geen lid maken als de groep geen afdelingen heeft!");
			}

			//Zelfde groep (persoon en werkjaar)
			if (gpMetDetails.Groep.ID != gwj.Groep.ID)
			{
				throw new FoutieveGroepException(Properties.Resources.FoutiefGroepsWerkJaar);
			}

			return true;
		}

		/// <summary>
		/// Maakt gelieerde persoon een kind (lid) voor het recentste werkjaar
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <returns>Nieuw kindobject, niet gepersisteerd</returns>
		public Kind KindMaken(GelieerdePersoon gp)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				if (gp.Groep == null)
				{
					_daos.GelieerdePersoonDao.GroepLaden(gp);
				}

				GroepsWerkJaar gwj = _daos.GroepenDao.RecentsteGroepsWerkJaarGet(gp.Groep.ID);

				return KindMaken(gp, gwj);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Maakt gelieerde persoon een kind (lid) voor het gegeven werkjaar
		/// </summary>
		/// <param name="gp">Gelieerde persoon, gekoppeld aan groep</param>
		/// <param name="gwj">Groepswerkjaar waarin lid te maken</param>
		/// <returns>Nieuw kindobject, niet gepersisteerd</returns>
		/// <remarks>De user zal nooit zelf mogen kiezen in welk groepswerkjaar een kind lid wordt.  Maar 
		/// om testdata voor unit tests op te bouwen, hebben we deze functionaliteit wel nodig.
		/// </remarks>
		public Kind KindMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				GelieerdePersoon gpMetDetails;
				if (!kanLidMaken(gp, gwj, out gpMetDetails))
				{
					throw new OngeldigeActieException("De gegeven persoon kan geen lid worden in het huidige werkjaar.");
				}

				//Geboortedatum is verplicht als je lid wilt worden
				if (gpMetDetails.Persoon.GeboorteDatum == null)
				{
					throw new OngeldigeActieException("Je kan geen lid maken als je de geboortedatum van de persoon niet hebt ingegeven");
				}

				//Afdeling automatisch bepalen
				int geboortejaar = gpMetDetails.Persoon.GeboorteDatum.Value.Year;
				geboortejaar += gpMetDetails.ChiroLeefTijd;	 //aanpassen aan chiroleeftijd

				//relevante afdelingsjaar opzoeken
				AfdelingsJaar afdelingsjaar =
						(from a in gwj.AfdelingsJaar
						 where a.GeboorteJaarVan <= geboortejaar
							&& geboortejaar <= a.GeboorteJaarTot
							&& (a.Geslacht == GeslachtsType.Gemengd
								|| a.Geslacht == gpMetDetails.Persoon.Geslacht)
						 select a).FirstOrDefault();
				// (eerste is goed)

				if (afdelingsjaar == null)
				{
					throw new OngeldigeActieException(Properties.Resources.GeenGeschikteAfdeling);
				}


				Kind kind = new Kind();
				kind.AfdelingsJaar = afdelingsjaar;
				afdelingsjaar.Kind.Add(kind);
				kind.EindeInstapPeriode = DateTime.Today.AddDays(int.Parse(Properties.Resources.InstapPeriode));
				
				LidMaken(kind, gpMetDetails, gwj);

				return kind;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Maakt de gelieerdepersoon leiding in het huidige werkjaar.
		/// </summary>
		/// <param name="gp"></param>
		/// <returns></returns>
		public Leiding LeidingMaken(GelieerdePersoon gp)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				GroepsWerkJaarManager mgr = Factory.Maak<GroepsWerkJaarManager>();

				if (gp.Groep == null)
				{
					_daos.GelieerdePersoonDao.GroepLaden(gp);
				}

				GroepsWerkJaar gwj = _daos.GroepenDao.RecentsteGroepsWerkJaarGet(gp.Groep.ID);

				GelieerdePersoon gpMetDetails;
				if (!kanLidMaken(gp, gwj, out gpMetDetails))
				{
					throw new OngeldigeActieException("De gegeven persoon kan geen lid worden in het huidige werkjaar.");
				}

				Leiding leiding = new Leiding();

				LidMaken(leiding, gpMetDetails, gwj);
				return leiding;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Verwijdert lid uit database
		/// Dit kan enkel als het een lid uit het huidige werkjaar is en als de instapperiode niet verstreken is voor een kind
		/// </summary>
		/// <param name="id">LidID</param>
		/// <returns>true on successful</returns>
		public Boolean LidVerwijderen(int id)
		{
			if (_autorisatieMgr.IsGavLid(id))
			{
				Lid lid = _daos.LedenDao.OphalenMetDetails(id);

				//checks:
				if (lid.GroepsWerkJaar != _daos.GroepenDao.RecentsteGroepsWerkJaarGet(lid.GroepsWerkJaar.Groep.ID))
				{
					throw new OngeldigeActieException("Een lid verwijderen mag enkel als het een lid uit het huidige werkjaar is.");
				}
				if(lid is Kind && DateTime.Compare(((Kind)lid).EindeInstapPeriode.Value, DateTime.Today)<0)
				{
					throw new OngeldigeActieException("Een kind verwijderen kan niet meer nadat de instapperiode verstreken is.");
				}

				lid.TeVerwijderen = true;

				Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");

				// voor een Kind is _dao.Bewaren(lid) voldoende
				if (lid is Kind)
				{
					_daos.LedenDao.Bewaren(lid);
				}
				// TODO voor Leiding moet er blijkbaar meer gebeuren
				// onderstaande code werkt niet
				else if (lid is Leiding)
				{
					_daos.LedenDao.Bewaren(lid);
				}

				return true;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
		}

		/// <summary>
		/// Haal een pagina op met leden van een groepswerkjaar.
		/// </summary>
		/// <param name="groepswerkjaarID">groepswerkjaarID</param>
		/// <param name="pagina's">totaal aantal groepswerkjaren van de groep</param>
		/// <returns>Lijst met alle leden uit het gevraagde groepswerkjaar.</returns>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, out int paginas)
		{
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				GroepsWerkJaar gwj = _daos.GroepsWerkJaarDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep);
				paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
				IList<Lid> list = _daos.LedenDao.AllesOphalen(groepsWerkJaarID);
				list = list.OrderBy(e => e.GelieerdePersoon.Persoon.Naam).ThenBy(e => e.GelieerdePersoon.Persoon.VoorNaam).ToList();
				return list;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}
		}

		/// <summary>
		/// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
		/// <param name="afdelingsID">ID gevraagde afdeling</param>
		/// <returns></returns>
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas)
		{
			GroepsWerkJaar gwj = _daos.GroepsWerkJaarDao.Ophalen(groepsWerkJaarID, grwj=>grwj.Groep);
			paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				return _daos.LedenDao.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID);
			}
			else
			{
				throw new GeenGavException("Dit GroepsWerkJaar hoort niet bij je groep(en).");
			}
		}

		public Lid LidBewaren(Lid lid)
		{
			if (_autorisatieMgr.IsGavLid(lid.ID))
			{
				Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");
				if (lid is Kind)
				{
					return _daos.KindDao.Bewaren((Kind)lid);
				}
				else if (lid is Leiding)
				{
					return _daos.LeidingDao.Bewaren((Leiding)lid);
				}
				//hier mag de code nooit komen (zie assert)
				return null;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
		}

		/// <summary>
		/// Haalt lid op met afdelingsjaren, afdelingen en gelieerdepersoon
		/// </summary>
		/// <param name="gelieerdePersoonID">ID gevraagde lid</param>
		/// <returns>Lid met afdelingsjaren, afdelingen en gelieerdepersoon.</returns>
		public Lid OphalenMetAfdelingen(int lidID)
		{
			if (_autorisatieMgr.IsGavLid(lidID))
			{
				return _daos.LedenDao.OphalenMetDetails(lidID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
		}

		/// <summary>
		/// De afdelingen van het gegeven lid worden aangepast van whatever momenteel zijn afdelingen zijn naar
		/// de gegeven lijst nieuwe afdelingen.
		/// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
		/// </summary>
		/// <param name="l">Lid, geladen met groepswerkjaar met afdelingsjaren</param>
		/// <param name="afdelingsIDs">De ids van de AFDELING waarvan het kind lid is</param>
        public void AanpassenAfdelingenVanLid(Lid l, IList<int> afdelingsIDs)
        {
            if(l is Kind)
            {
                Kind kind = (Kind)l;
                if(afdelingsIDs.Count!=1)
                {
                    throw new OngeldigeActieException("Een kind moet in exact 1 afdeling zitten.");
                }
				
                if (kind.AfdelingsJaar.Afdeling.ID != afdelingsIDs[0]) //anders verandert er niets
                {
                    //verwijder het kind uit zijn huidige afdelingsjaar
                    kind.AfdelingsJaar.TeVerwijderen = true;

					AfdelingsJaar ajnieuw = kind.GroepsWerkJaar.AfdelingsJaar.FirstOrDefault(e => e.Afdeling.ID == afdelingsIDs[0]);
					if (ajnieuw == null)
					{
						throw new OngeldigeActieException("De gekozen afdeling is geen afdeling van de groep in het gekozen werkjaar.");
					}
					ajnieuw.Kind.Add(kind);
					kind.AfdelingsJaar = ajnieuw;
				}
			}
			else
			{
				Leiding leiding = (Leiding)l;
				var afdelingsjaren = leiding.GroepsWerkJaar.AfdelingsJaar.ToList();
				foreach (AfdelingsJaar aj in afdelingsjaren)
				{
					//als het een afdelingsjaar waar de leider in moet komen
					if (afdelingsIDs.Contains(aj.Afdeling.ID))
					{
						//en hij zit er nog niet in
						if (leiding.AfdelingsJaar.FirstOrDefault(e => e.Afdeling.ID == aj.Afdeling.ID) == null)
						{
							//voeg het dan toe
							leiding.AfdelingsJaar.Add(aj);
							aj.Leiding.Add(leiding);
						}
						afdelingsIDs.Remove(aj.Afdeling.ID);
					}
					else //de leider mag niet meer in de afdeling zitten
					{
						//en hij zit er wel in
						if (leiding.AfdelingsJaar.FirstOrDefault(e => e.Afdeling.ID == aj.Afdeling.ID) != null)
						{
							//verwijder hem eruit
							leiding.AfdelingsJaar.FirstOrDefault(e => e.ID == aj.ID).TeVerwijderen = true;
						}
					}
				}
				if (afdelingsIDs.Count != 0)
				{
					throw new OngeldigeActieException("Niet alle gekozen afdelingen zijn afdelingen van de groep in het gekozen werkjaar.");
				}
			}
		}
	}
}
