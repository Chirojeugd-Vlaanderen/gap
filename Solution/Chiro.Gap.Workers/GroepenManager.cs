using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Fouten.Exceptions;
using System.Diagnostics;

namespace Chiro.Gap.Workers
{
    public class GroepenManager
    {
        private IGroepenDao _dao;
        private IDao<AfdelingsJaar> _afdao;
        private IAutorisatieManager _autorisatieMgr;
		private ICategorieenDao _categorieenDao;

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de groepen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="dao">Alternatieve dao</param>
        public GroepenManager(IGroepenDao dao, IDao<AfdelingsJaar> afdao, ICategorieenDao categorieenDao, IAutorisatieManager autorisatieMgr)
        {
            _dao = dao;
            _afdao = afdao;
            _autorisatieMgr = autorisatieMgr;
            _categorieenDao = categorieenDao;
        }

        /// <summary>
        /// Haalt groep op, op basis van GroepID
        /// </summary>
        /// <param name="groepID">ID op te halen groep</param>
        /// <returns>gevraagde groep</returns>
        public Groep Ophalen(int groepID)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.Ophalen(groepID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Haalt recentste groepswerkjaar voor een groep op.
        /// </summary>
        /// <param name="groepID">GroepID gevraagde groep</param>
        /// <returns>Groepswerkjaar</returns>
        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.RecentsteGroepsWerkJaarGet(groepID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }
       

        /// <summary>
        /// Maakt een nieuwe afdeling voor een groep, en persisteert
        /// </summary>
        /// <param name="groepID">GroepID van gevraagde groep</param>
        /// <param name="naam">naam van de afdeling</param>
        /// <param name="afkorting">handige afkorting voor in schemaatjes</param>
        public void AfdelingToevoegen(int groepID, string naam, string afkorting)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                Afdeling a = new Afdeling();
                Groep g = _dao.Ophalen(groepID);

                a.AfdelingsNaam = naam;
                a.Afkorting = afkorting;
                a.Groep = g;
                g.Afdeling.Add(a);

                _dao.Bewaren(g);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Maakt een afdelingsjaar voor een groep en een afdeling
        /// </summary>
        /// <param name="g">Groep voor nieuw afdelingsjaar</param>
        /// <param name="a">Afdeling voor nieuw afdelingsjaar</param>
        /// <param name="oa">Te koppelen officiele afdeling</param>
        /// <param name="geboorteJaarBegin">Geboortejaar van</param>
        /// <param name="geboorteJaarEind">Geboortejaar tot</param>
        public void AfdelingsJaarToevoegen(Groep g, Afdeling a, OfficieleAfdeling oa, int geboorteJaarBegin, int geboorteJaarEind)
        {
            if (!_autorisatieMgr.IsGavGroep(g.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }

            if (!_autorisatieMgr.IsGavAfdeling(a.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavAfdeling);
            }

            if (geboorteJaarBegin < System.DateTime.Today.Year - 20
                || geboorteJaarBegin > geboorteJaarEind
                || geboorteJaarEind > System.DateTime.Today.Year - 5)
            {
                throw new InvalidOperationException("Ongeldige geboortejaren voor het afdelingsjaar");
            }

            AfdelingsJaar afdelingsJaar = new AfdelingsJaar();
            GroepsWerkJaar huidigWerkJaar = _dao.RecentsteGroepsWerkJaarGet(g.ID);

            if (!a.Groep.Equals(g))
            {
                throw new FoutieveGroepException(String.Format("Afdeling {0} is geen afdeling van Groep {1}" , a.AfdelingsNaam, g.Naam));
            }

            // TODO: test of de officiele afdeling bestaat, heb
            // ik voorlopig even weggelaten.  Als de afdeling niet
            // bestaat, zal er bij het bewaren toch een exception
            // optreden, aangezien het niet de bedoeling is dat
            // een officiele afdeling bijgemaakt wordt.

            //TODO check if no conflicts with existing afdelingsjaar

            afdelingsJaar.OfficieleAfdeling = oa;
            afdelingsJaar.Afdeling = a;
            afdelingsJaar.GroepsWerkJaar = huidigWerkJaar;
            afdelingsJaar.GeboorteJaarVan = geboorteJaarBegin;
            afdelingsJaar.GeboorteJaarTot = geboorteJaarEind;

            a.AfdelingsJaar.Add(afdelingsJaar);
            oa.AfdelingsJaar.Add(afdelingsJaar);
            huidigWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            //TODO hier zou ook nog withoutupdate achter de lambdas moeten staan, maar dat is niet mogelijk
            //buiten de chirogroepentities extension
            _afdao.Bewaren(afdelingsJaar, aj => aj.OfficieleAfdeling,
                                     aj => aj.Afdeling,
                                     aj => aj.GroepsWerkJaar);
        }

        /// <summary>
        /// Haalt lijst officiele afdelingen op.
        /// </summary>
        /// <returns>Lijst officiele afdelingen</returns>
        public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
        {
            // Iedereen heeft het recht deze op te halen.
            return _dao.OphalenOfficieleAfdelingen();
        }

        /// <summary>
        /// haalt alle AfdelingsJaren op bij een gegeven GroepsWerkJaar
        /// </summary>
        /// <remarks>TODO: Authorisatie!</remarks>
        /// <param name="gwj">Groepswerkjaar waarvoor afdelingsjaren op te halen</param>
        /// <returns>Lijst van afdelingsjaren bij een groepswerkjaar</returns>
        public IList<AfdelingsJaar> AfdelingsJarenOphalen(GroepsWerkJaar gwj)
        {
            if (_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
            {
                IList<AfdelingsJaar> result = new List<AfdelingsJaar>();
                Groep g = _dao.OphalenMetAfdelingen(gwj.ID);
                // Aan g hangt nu slechts 1 groepswerkjaar, met name
                // (een kopie van) gwj.

                return g.GroepsWerkJaar.First().AfdelingsJaar.ToList();
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
            }
        }


        /// <summary>
        /// Haalt recentste groepswerkjaar op voor gegeven groep
        /// </summary>
        /// <param name="p">ID van gegeven groep</param>
        /// <returns>Gevraagde groepswerkjaar</returns>
        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(Groep g)
        {
            if (_autorisatieMgr.IsGavGroep(g.ID))
            {
                return _dao.RecentsteGroepsWerkJaarGet(g.ID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Ophalen van groep met zijn afdelingen
        /// </summary>
        /// <param name="groepid"></param>
        /// <returns></returns>
        public Groep OphalenMetAfdelingen(int groepID)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.Ophalen(groepID, e=>e.Afdeling);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        public Groep OphalenMetCategorieen(int groepID)
        {
				if (_autorisatieMgr.IsGavGroep(groepID))
				{
					 return _dao.Ophalen(groepID, e => e.Categorie);
				}
				else
				{
					 throw new GeenGavException(Properties.Resources.GeenGavGroep);
				}
        }

        public Groep OphalenMetFuncties(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Groep OphalenMetVrijeVelden(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Groep OphalenMetAdressen(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Persisteert groep in de database
        /// </summary>
        /// <param name="g">Te persisteren groep</param>
        /// <returns>De bewaarde groep</returns>
        public Groep Bewaren(Groep g)
        {
            if (_autorisatieMgr.IsGavGroep(g.ID))
            {
                return _dao.Bewaren(g);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }


		  /// <summary>
		  /// Persisteert wel
		  /// </summary>
		  /// <param name="c"></param>
		  /// <param name="gID"></param>
		  public void CategorieToevoegen(Categorie c, int gID)
		  {
				if (_autorisatieMgr.IsGavGroep(gID))
				{
					 Groep g = _dao.Ophalen(gID, l => l.Categorie);
					 c.Groep = g;
					 g.Categorie.Add(c);
				}
				else
				{
					 throw new GeenGavException(Properties.Resources.GeenGavGroep);
				}
				
		  }

		  /// <summary>
		  /// Persisteert wel
		  /// </summary>
		  /// <param name="categorieID"></param>
		  public void CategorieVerwijderen(int categorieID)
		  {
				Categorie c = _categorieenDao.Ophalen(categorieID);
				if (_autorisatieMgr.IsGavGroep(c.Groep.ID))
				{					 
					 c.TeVerwijderen = true;
					 _categorieenDao.Bewaren(c);
				}
				else
				{
					 throw new GeenGavException(Properties.Resources.GeenGavGroep);
				}
		  }
    }
}
