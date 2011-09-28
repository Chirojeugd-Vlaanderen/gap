using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Manager die informatie ophaalt over gebruikersrechten van personen waar jij 
    /// gebruikersrecht op hebt.
    /// </summary>
    public class GebruikersRechtenManager
    {
        private readonly IAutorisatieDao _autorisatieDao;
        private readonly IAutorisatieManager _autorisatieManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autorisatieDao">DAO die gebruikt zal worden om gegevens ivm gebruikersrechten op te zoeken</param>
        /// <param name="autorisatieManager">Autorisatiemanager die gebruikt zal worden om te controleren of de user wel
        /// rechten genoeg heeft om de gevraagde gegevens op te halen</param>
        public GebruikersRechtenManager(IAutorisatieDao autorisatieDao, IAutorisatieManager autorisatieManager)
        {
            _autorisatieDao = autorisatieDao;
            _autorisatieManager = autorisatieManager;
        }


        /// <summary>
        /// Als een gelieerde persoon een gebruikersrecht heeft/had voor zijn eigen groep, dan
        /// levert deze call dat gebruikersrecht op.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van een gelieerde persoon</param>
        /// <returns>Gebruikersrecht van de gelieerde persoon met ID <paramref name="gelieerdePersoonID"/>
        /// op zijn eigen groep (if any, anders null)</returns>
        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            if (!_autorisatieManager.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            return _autorisatieDao.GebruikersRechtGelieerdePersoon(gelieerdePersoonID);
        }

        /// <summary>
        /// Als een gelieerde persoon een gebruikersrecht heeft/had voor zijn eigen groep, dan
        /// levert deze call dat gebruikersrecht op.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van een gelieerde persoon</param>
        /// <returns>Gebruikersrecht van de gelieerde persoon met ID <paramref name="gelieerdePersoonID"/>
        /// op zijn eigen groep (if any, anders null)</returns>
        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersrecht.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            return gebruikersrecht.VervalDatum == null
                       ? false
                       : ((DateTime) gebruikersrecht.VervalDatum).AddMonths(
                           Properties.Settings.Default.MaandenGebruikersRechtVerlengbaar) > DateTime.Now;
        }
    }
}
