// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Bevat de businesslogica voor bivakplaatsen.
    /// </summary>
    public class PlaatsenManager
    {
        private readonly IAutorisatieManager _autorisatieManager;
        private readonly IPlaatsenDao _plaatsenDao;
        private readonly IAdressenDao _adressenDao;
        private readonly IGroepenDao _groepenDao;

        /// <summary>
        /// Standaardconstructor.  De parameters worden gebruikt voor dependency injection.
        /// </summary>
        /// <param name="plDao">Repository voor plaatsen</param>
        /// <param name="adDao">Repository voor adressen</param>
        /// <param name="grDao">Repository voor groepen</param>
        /// <param name="auMgr">Autorisatiemanager</param>
        public PlaatsenManager(IPlaatsenDao plDao, IAdressenDao adDao, IGroepenDao grDao, IAutorisatieManager auMgr)
        {
            _plaatsenDao = plDao;
            _autorisatieManager = auMgr;
            _adressenDao = adDao;
            _groepenDao = grDao;
        }

        /// <summary>
        /// Zoekt of maakt een bivakplaats
        /// </summary>
        /// <param name="groepID">ID van ingevende groep</param>
        /// <param name="plaatsNaam">Naam van de plaats</param>
        /// <param name="adresID">ID van het adres van de plaats</param>
        /// <returns>De gezochte of gemaakte plaats, met daaraan gekoppeld alle uitstappen én adres. Persisteert</returns>
        public Plaats ZoekenOfMaken(int groepID, string plaatsNaam, int adresID)
        {
            if (!_autorisatieManager.IsGavGroep(groepID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            else
            {
                Debug.Assert(groepID != 0);
                Debug.Assert(adresID != 0);

                var plaats = _plaatsenDao.Zoeken(groepID, plaatsNaam, adresID, pl => pl.Uitstap, pl => pl.Adres);

                if (plaats == null)
                {
                    var adres = _adressenDao.Ophalen(adresID, adr => adr.BivakPlaats);
                    var groep = _groepenDao.Ophalen(groepID, grp => grp.BivakPlaats);

                    plaats = Maken(plaatsNaam, adres, groep);
                    plaats = Bewaren(plaats);
                }

                return plaats;
            }
        }

        /// <summary>
        /// Bewaart een plaats, en zijn koppeling met groep en adres.
        /// </summary>
        /// <param name="plaats">Te bewaren plaats</param>
        /// <returns>De bewaarde plaats, eventueel met nieuw ID</returns>
        private Plaats Bewaren(Plaats plaats)
        {
            return _plaatsenDao.Bewaren(plaats, pl => pl.Groep.WithoutUpdate(), pl => pl.Adres.WithoutUpdate());
        }

        /// <summary>
        /// Maakt een bivakplaats op basis van de naam <paramref name="plaatsNaam"/>, het
        /// <paramref name="adres"/> van de bivakplaats, en de ingevende
        /// <paramref name="groep"/>.
        /// </summary>
        /// <param name="plaatsNaam">Naam van de bivakplaats</param>
        /// <param name="adres">Adres van de bivakplaats</param>
        /// <param name="groep">Groep die de bivakplaats ingeeft</param>
        /// <returns>De nieuwe plaats; niet gepersisteerd.</returns>
        private Plaats Maken(string plaatsNaam, Adres adres, Groep groep)
        {
            var resultaat = new Plaats { Naam = plaatsNaam, Adres = adres, Groep = groep, ID = 0 };

            adres.BivakPlaats.Add(resultaat);
            groep.BivakPlaats.Add(resultaat);

            return resultaat;
        }
    }
}
