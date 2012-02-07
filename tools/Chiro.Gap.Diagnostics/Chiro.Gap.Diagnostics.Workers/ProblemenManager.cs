using System;
using System.Collections.Generic;

using Chiro.Cdf.Data;
using Chiro.Gap.Diagnostics.Data;
using Chiro.Gap.Diagnostics.Orm;
using Chiro.Gap.Diagnostics.Orm.DataInterfaces;

namespace Chiro.Gap.Diagnostics.Workers
{
    /// <summary>
    /// Backend-functionaliteit voor diagnostische doeleinden
    /// </summary>
    public class ProblemenManager
    {
        private readonly IVerlorenAdressenDao _verlorenAdressenDao;
        private readonly IFunctieProblemenDao _functieProblemenDao;

        /// <summary>
        /// Standaardconstructor.  Data access wordt geinjecteerd.
        /// </summary>
        /// <param name="verlorenAdressenDao">Data access voor verloren adressen</param>
        /// <param name="functieProblemenDao">Data access voor functieproblemen</param>
        public ProblemenManager(IVerlorenAdressenDao verlorenAdressenDao, IFunctieProblemenDao functieProblemenDao)
        {
            _verlorenAdressenDao = verlorenAdressenDao;
            _functieProblemenDao = functieProblemenDao;
        }

        /// <summary>
        /// Haalt de personen op die wel een adres hebben in Kipadmin, maar geen adres
        /// in GAP.
        /// </summary>
        /// <returns>Info over alle personen waarvan het adres verloren ging</returns>
        public IEnumerable<VerlorenAdres> VerdwenenAdressenOphalen()
        {
            return _verlorenAdressenDao.AllesOphalen();
        }

        /// <summary>
        /// Haalt alle functie-inconsistenties op voor het huidige werkjaar
        /// </summary>
        /// <returns>Rijen informatie voor inconsistenties tussen functies in het GAP
        /// en functies in Kipadmin.</returns>
        public IEnumerable<FunctieProbleem> FunctieProblemenOphalen()
        {
            return _functieProblemenDao.AllesOphalen();
        }
    }
}
