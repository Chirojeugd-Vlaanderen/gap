// <copyright company="Chirojeugd-Vlaanderen vzw" file="">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>
// <summary>
//   Worker class met operaties op afdelingen
// </summary>

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker class met operaties op afdelingen
    /// </summary>
    public class AfdelingenManager
    {
        private readonly IAfdelingenDao _afdelingenDao;
        private readonly IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// De constructor krijgt via dependency injection de data access mee, en een worker
        /// class voor autorisatie.
        /// </summary>
        /// <param name="afdelingenDao">Data access wat betreft afdelingen</param>
        /// <param name="autorisatieMgr">Klasse met methods voor autorisatie</param>
        public AfdelingenManager(IAfdelingenDao afdelingenDao, IAutorisatieManager autorisatieMgr)
        {
            _afdelingenDao = afdelingenDao;
            _autorisatieMgr = autorisatieMgr;
        }

        /// <summary>
        /// Verwijdert Afdeling uit database
        /// </summary>
        /// <param name="afdelingID">ID van de Afdeling</param>
        /// <returns><c>True</c> on successful</returns>
        public bool Verwijderen(int afdelingID)
        {
            Afdeling afd = _afdelingenDao.Ophalen(afdelingID);

            if (!_autorisatieMgr.IsGavAfdeling(afdelingID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            afd.TeVerwijderen = true;
            _afdelingenDao.Bewaren(afd);
            return true;
        }
    }
}
