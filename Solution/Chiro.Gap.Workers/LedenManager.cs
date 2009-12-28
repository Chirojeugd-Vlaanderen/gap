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
		private IAutorisatieManager _authorisatieMgr;

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
            _authorisatieMgr = autorisatie;
        }

		/// <summary>
		/// 'Opwaarderen' van een gelieerd persoon tot een lid.
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <param name="gwj">Groepswerkjaar, met gekoppelde afdelingsjaren</param>
		/// <returns>Lidobject, niet gepersisteerd</returns>
		/// <remarks>Normaalgezien worden er alleen leden bijgemaakt in het 
		/// huidige werkjaar.
		/// </remarks>
		public Lid LidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			if (!_authorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
			if (!_authorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}

			// JOHAN: Ik zou precies de afdelingsjaren van de groep in het
			// gegeven GroepsWerkJaar meegeven via de parameters.  Op die manier
			// moet deze businessmethod geen Data Access gaan doen.

			// TODO: geslacht in rekening brengen bij automatisch keuze afdeling
			// TODO: controle of lid nog niet bestaat
			// TODO: berekening EindeInstapPeriode
			// TODO: controle of groepswerkjaar van zelfde groep als gelieerde persoon.

			Lid lid;

			// De persoonsgegevens zijn nodig om de afdeling te bepalen.  Haal ze op als ze ontbreken.

			GelieerdePersoon gpMetDetails;

			if (gp.Persoon == null)
			{
				GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();
				gpMetDetails = gpm.DetailsOphalen(gp.ID);
			}
			else
			{
				gpMetDetails = gp;
			}

            var x = (from l in gpMetDetails.Lid
                    where l.GroepsWerkJaar.ID == gwj.ID
                    select l
                    ).FirstOrDefault();

            if (x != null) //was dus al lid
            {
                throw new BestaatAlException();
            }

			// Geschikte afdeling zoeken
			// Als er geen geschikte afdeling is, dan null (wordt Leiding)
			AfdelingsJaar aj = null;
			if (gwj.AfdelingsJaar.Count > 0)
			{
				int geboorte = 0;
				if (gpMetDetails.Persoon.GeboorteDatum != null)
				{
					geboorte = ((DateTime)gpMetDetails.Persoon.GeboorteDatum).Year - gp.ChiroLeefTijd;
				}
				foreach (AfdelingsJaar jaar in gwj.AfdelingsJaar)
				{
					if (jaar.GeboorteJaarVan <= geboorte && geboorte <= jaar.GeboorteJaarTot)
					{
						aj = jaar;
					}
				}
			}

			// Specifieke dingen voor Leiding of Kind
			if (aj == null)
			{
				Leiding leiding = new Leiding();
				foreach (AfdelingsJaar ajw in gwj.AfdelingsJaar)
				{
					Random seed = new Random();
					double r = seed.NextDouble();
					if (r < 0.2)
					{
						leiding.AfdelingsJaar.Add(ajw);
						ajw.Leiding.Add(leiding);
					}
				}
				lid = leiding;
			}
			else
			{
				Kind kind = new Kind();
				kind.AfdelingsJaar = aj;
				aj.Kind.Add(kind);
				lid = kind;
			}

			// GroepsWerkJaar en GelieerdePersoon invullen
			lid.GroepsWerkJaar = gwj;
			lid.GelieerdePersoon = gp;
			gp.Lid.Add(lid);
			gwj.Lid.Add(lid);

			return lid;
		}



		/// <summary>
		/// Maakt gelieerde persoon lid voor recentste werkjaar
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <returns>Nieuw lidobject, niet gepersisteerd</returns>
		public Lid LidMaken(GelieerdePersoon gp)
		{
			if (_authorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				GroepenManager gm = Factory.Maak<GroepenManager>();

				if (gp.Groep == null)
				{
					_daos.GelieerdePersoonDao.GroepLaden(gp);
				}

				GroepsWerkJaar gwj = gm.RecentsteGroepsWerkJaarGet(gp.Groep.ID);
				return LidMaken(gp, gm.RecentsteGroepsWerkJaarGet(gp.Groep.ID));
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Verwijdert lid uit database
		/// </summary>
		/// <param name="id">LidID</param>
		/// <returns>true on successful</returns>
		public Boolean LidVerwijderen(int id)
		{
			if (_authorisatieMgr.IsGavLid(id))
			{
				// TODO: controleren huidige werkjaar
				// TODO: controleren instapperiode
				// TODO: fallback naar LidOpNonactiefZetten?
				// TODO: controleren of verwijderen effectief gelukt is
				// TODO: probleem oplossen met Leiding bewaren 

				Lid lid = _daos.LedenDao.OphalenMetDetails(id);
				lid.TeVerwijderen = true;

				Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");

				// voor een Kind is _dao.Bewaren(lid) voldoende
				if (lid is Kind)
				{
					_daos.LedenDao.Bewaren(lid);
				}
				// voor Leiding moet er blijkbaar meer gebeuren
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
        /// <param name="pagina">paginanummer (>=1)</param>
        /// <param name="paginaGrootte">grootte van de pagina's</param>
        /// <param name="aantalTotaal">totaal aantal personen in de groep</param>
        /// <returns>Lijst met een pagina leden uit het gevraagde groepswerkjaar.</returns>
        public IList<Lid> PaginaOphalen(int groepswerkjaarID, out int paginas)
        {
            GroepsWerkJaar gwj = _daos.GroepenDao.GroepsWerkJaarOphalen(groepswerkjaarID);
            paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
            if (_authorisatieMgr.IsGavGroepsWerkJaar(groepswerkjaarID))
            {
                IList<Lid> list = _daos.LedenDao.AllesOphalen(groepswerkjaarID);
                list = list.OrderBy(e => e.GelieerdePersoon.Persoon.Naam).ThenBy(e => e.GelieerdePersoon.Persoon.VoorNaam).ToList();
                return list;
            }
            else
            {
                throw new GeenGavException("Dit GroepsWerkJaar hoort niet bij je groep(en).");
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
            GroepsWerkJaar gwj = _daos.GroepenDao.GroepsWerkJaarOphalen(groepsWerkJaarID);
            paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
			if (_authorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				return _daos.LedenDao.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID);
			}
			else
			{
				throw new GeenGavException("Dit GroepsWerkJaar hoort niet bij je groep(en).");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lid"></param>
		public void LidOpNonactiefZetten(Lid lid)
		{
			if (_authorisatieMgr.IsGavLid(lid.ID))
			{
				lid.NonActief = true;
				//TODO save
				// JOHAN: Bewaren is niet per se nodig in deze method.  Wel
				// goed documenteren of bewaard moet worden of niet.

				throw new NotImplementedException();
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lid"></param>
		public void LidActiveren(Lid lid)
		{
			if (_authorisatieMgr.IsGavLid(lid.ID))
			{
				lid.NonActief = false;
				//TODO er moet betaald worden + save
				// JOHAN: Bewaren is niet per se nodig in deze method.  Wel
				// goed documenteren of bewaard moet worden of niet.
				throw new NotImplementedException();
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
		}

		public void LidBewaren(Lid lid)
		{
			if (_authorisatieMgr.IsGavLid(lid.ID))
			{
				Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");
				if (lid is Kind)
				{
					_daos.KindDao.Bewaren((Kind)lid);
				}
				else if (lid is Leiding)
				{
					_daos.LeidingDao.Bewaren((Leiding)lid);
				}
				else
				{
					// hier komen we in principe nooit (zie Assert)
					_daos.LedenDao.Bewaren(lid);
				}
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
            if (_authorisatieMgr.IsGavLid(lidID))
            {
                return _daos.LedenDao.OphalenMetDetails(lidID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavLid);
            }
        }

        public void UpdatenAfdelingen(Lid l, IList<int> afdelingsIDs)
        {
            if(l is Kind)
            {
                Kind l2 = (Kind)l;
                if(afdelingsIDs.Count>1)
                {
                    throw new ArgumentException("Een kind mag maar in 1 afdeling zitten tegelijk");
                }
                if (afdelingsIDs.Count == 1 && l2.AfdelingsJaar.Afdeling.ID != afdelingsIDs[0])
                {
                    //verwijder het kind uit zijn huidige afdelingsjaar
                    AfdelingsJaar ajoud = l2.AfdelingsJaar;
                    //ajoud.Kind.FirstOrDefault(e => e.ID == l2.ID).TeVerwijderen = true;
                    l2.AfdelingsJaar.TeVerwijderen = true;

                    AfdelingsJaar ajnieuw = l2.GroepsWerkJaar.AfdelingsJaar.FirstOrDefault(e => e.Afdeling.ID == afdelingsIDs[0]);
                    if (ajnieuw == null)
                    {
                        //TODO mag niet voorkomen?
                        throw new ArgumentException("Er is zo geen afdelingsjaar");
                    }
                    ajnieuw.Kind.Add(l2);
                    l2.AfdelingsJaar = ajnieuw;
                }
            }else
            {
                Leiding l2 = (Leiding)l;
                var afdelingsjaren = l2.GroepsWerkJaar.AfdelingsJaar.ToList();
                foreach (AfdelingsJaar aj in afdelingsjaren)
                {
                    if (afdelingsIDs.Contains(aj.Afdeling.ID))
                    {
                        if (l2.AfdelingsJaar.FirstOrDefault(e => e.Afdeling.ID == aj.Afdeling.ID)==null)
                        {
                            l2.AfdelingsJaar.Add(aj);
                            aj.Leiding.Add(l2);
                        }
                    }
                    else
                    {
                        if (l2.AfdelingsJaar.FirstOrDefault(e => e.Afdeling.ID == aj.Afdeling.ID) != null)
                        {
                            l2.AfdelingsJaar.FirstOrDefault(e => e.ID == aj.ID).TeVerwijderen = true;
                            //aj.Leiding.FirstOrDefault(e => e.ID == l2.ID).TeVerwijderen = true;
                        }
                    }
                }
            }
        }
	}
}
