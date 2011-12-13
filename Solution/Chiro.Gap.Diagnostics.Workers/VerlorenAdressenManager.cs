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
    public class VerlorenAdressenManager
    {
        private readonly IVerlorenAdressenDao _verlorenAdressenDao;

        /// <summary>
        /// Standaardconstructor.  Data access wordt geinjecteerd.
        /// </summary>
        /// <param name="verlorenAdressenDao">Data access voor verloren adressen</param>
        public VerlorenAdressenManager(IVerlorenAdressenDao verlorenAdressenDao)
        {
            _verlorenAdressenDao = verlorenAdressenDao;
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
    }
}
