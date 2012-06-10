// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. categorieën bevat
    /// </summary>
    public class CategorieenManager
    {
        private readonly ICategorieenDao _dao;
        private readonly IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// Creëert een CategorieenManager
        /// </summary>
        /// <param name="dao">
        /// Repository voor categorieën
        /// </param>
        /// <param name="auMgr">
        /// Worker die autorisatie regelt
        /// </param>
        public CategorieenManager(ICategorieenDao dao, IAutorisatieManager auMgr)
        {
            _dao = dao;
            _autorisatieMgr = auMgr;
        }

        #region Proxy naar DAO

        /// <summary>
        /// Een categorie ophalen op basis van zijn <paramref name="categorieID"/>, inclusief groep.
        /// </summary>
        /// <param name="categorieID">
        /// ID van op te halen categorie
        /// </param>
        /// <returns>
        /// Opgehaalde categorie, met gekoppelde groep
        /// </returns>
        public Categorie Ophalen(int categorieID)
        {
            return Ophalen(categorieID, false); // personen interesseren ons niet
        }

        /// <summary>
        /// Een categorie ophalen op basis van zijn <paramref name="categorieID"/>, inclusief groep.
        /// </summary>
        /// <param name="categorieID">
        /// ID van op te halen categorie
        /// </param>
        /// <param name="metPersonen">
        /// Indien <c>true</c>, worden ook de gekoppelde gelieerde 
        /// personen en persoonsinfo mee opgehaald
        /// </param>
        /// <returns>
        /// Opgehaalde categorie, met gekoppelde groep
        /// </returns>
        public Categorie Ophalen(int categorieID, bool metPersonen)
        {
            if (_autorisatieMgr.IsGavCategorie(categorieID))
            {
                if (metPersonen)
                {
                    return _dao.Ophalen(categorieID, cat => cat.GelieerdePersoon.First().Persoon);
                }
                else
                {
                    return _dao.Ophalen(categorieID);
                }
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Persisteert de <paramref name="categorie"/> in de database.
        /// </summary>
        /// <param name="categorie">
        /// Te persisteren categorie
        /// </param>
        /// <returns>
        /// Gepersisteerde categorie (met correct ID)
        /// </returns>
        public Categorie Bewaren(Categorie categorie)
        {
            if (_autorisatieMgr.IsGavCategorie(categorie.ID))
            {
                return _dao.Bewaren(categorie);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Zoekt een categorie op op basis van <paramref name="groepID"/> en
        /// <paramref name="code"/>.
        /// </summary>
        /// <param name="groepID">
        /// ID van groep waaraan de te zoeken categorie gekoppeld moet zijn
        /// </param>
        /// <param name="code">
        /// Code van de te zoeken categorie
        /// </param>
        /// <returns>
        /// De gevonden categorie; <c>null</c> indien niet gevonden
        /// </returns>
        public Categorie Ophalen(int groepID, string code)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.Ophalen(groepID, code);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Haalt alle categorieen op van de groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">
        /// ID van groep waarvan categorieen gevraagd
        /// </param>
        /// <returns>
        /// Lijst met categorie-info van alle categorieen van de groep
        /// </returns>
        public IList<Categorie> AllesOphalen(int groepID)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.AllesOphalen(groepID);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Bewaart de categorie, met daaraan gekopped de personen
        /// </summary>
        /// <param name="cat">
        /// Te bewaren categorie
        /// </param>
        /// <returns>
        /// Een kloon van de categorie, met eventueel geupdatete ID's of verwijderde
        /// personen.
        /// </returns>
        public Categorie BewarenMetPersonen(Categorie cat)
        {
            if (_autorisatieMgr.IsGavCategorie(cat.ID))
            {
                // TODO (#116): (bug ) Eigenlijk zou de lambda-expressie hieronder
                // cgrie => cgrie.GelieerdePersoon.First().WithoutUpdate()
                // moeten zijn.  Maar met WithoutUpdate worden blijkbaar de
                // TeVerwijderen categorieën genegeerd.
                return _dao.Bewaren(cat, cgrie => cgrie.GelieerdePersoon.First());
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Verwijdert een categorie (PERSISTEERT!)
        /// </summary>
        /// <param name="categorie">
        /// Te verwijderen categorie, inclusief gelieerde personen
        /// </param>
        /// <param name="forceren">
        /// Indien <c>true</c> wordt de categorie ook verwijderd als er
        /// personen in de categorie zitten.  Anders krijg je een exception.
        /// </param>
        /// <remarks>
        /// Deze method gaat ervan uit dat de categorie zijn leden bevat.
        /// </remarks>
        public void Verwijderen(Categorie categorie, bool forceren)
        {
            // Gelieerde personen moeten gekoppeld zijn
            // (null verschilt hier expliciet van een lege lijst)
            Debug.Assert(categorie.GelieerdePersoon != null);

            if (!forceren && categorie.GelieerdePersoon.Count > 0)
            {
                throw new BlokkerendeObjectenException<GelieerdePersoon>(
                    objecten: categorie.GelieerdePersoon.ToArray(), 
                    aantalTotaal: categorie.GelieerdePersoon.Count(), 
                    message: Resources.CategorieNietLeeg);
            }

            LeegMaken(categorie); // verwijdert alle gel. personen, en persisteert

            categorie.TeVerwijderen = true; // nu de categorie zelf nog
            Bewaren(categorie);
        }

        #endregion

        /// <summary>
        /// Verwijdert alle gelieerde personen uit de categorie <paramref name="c"/>, en persisteert
        /// </summary>
        /// <param name="c">
        /// Leeg te maken categorie
        /// </param>
        /// <returns>
        /// De categorie zonder personen
        /// </returns>
        public Categorie LeegMaken(Categorie c)
        {
            if (_autorisatieMgr.IsGavCategorie(c.ID))
            {
                foreach (GelieerdePersoon gp in c.GelieerdePersoon)
                {
                    gp.TeVerwijderen = true;

                    // dit verwijdert enkel de link naar de gelieerde persoon
                }

                return _dao.Bewaren(c, cat => cat.GelieerdePersoon);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }
    }
}