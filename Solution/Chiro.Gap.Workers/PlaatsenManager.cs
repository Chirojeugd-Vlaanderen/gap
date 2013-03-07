// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Bevat de businesslogica voor bivakplaatsen.
    /// </summary>
    public class PlaatsenManager
    {
        private readonly IAutorisatieManager _autorisatieManager;

        public PlaatsenManager(IAutorisatieManager auMgr)
        {
            _autorisatieManager = auMgr;
        }

        /// <summary>
        /// Maakt een bivakplaats op basis van de naam <paramref name="plaatsNaam"/>, het
        /// <paramref name="adres"/> van de bivakplaats, en de ingevende
        /// <paramref name="groep"/>.
        /// </summary>
        /// <param name="plaatsNaam">
        /// Naam van de bivakplaats
        /// </param>
        /// <param name="adres">
        /// Adres van de bivakplaats
        /// </param>
        /// <param name="groep">
        /// Groep die de bivakplaats ingeeft
        /// </param>
        /// <returns>
        /// De nieuwe plaats; niet gepersisteerd.
        /// </returns>
        private Plaats Maken(string plaatsNaam, Adres adres, Groep groep)
        {
            var resultaat = new Plaats { Naam = plaatsNaam, Adres = adres, Groep = groep, ID = 0 };

            adres.BivakPlaats.Add(resultaat);
            groep.BivakPlaats.Add(resultaat);

            return resultaat;
        }
    }
}