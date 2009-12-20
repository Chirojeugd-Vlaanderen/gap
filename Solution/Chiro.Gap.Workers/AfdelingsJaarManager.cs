using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    public class AfdelingsJaarManager
    {
        private IAfdelingsJarenDao _dao;
        private IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de groepen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="dao">Alternatieve dao</param>
        public AfdelingsJaarManager(IAfdelingsJarenDao dao, IAutorisatieManager autorisatieMgr)
        {
            _dao = dao;
            _autorisatieMgr = autorisatieMgr;
        }

        public AfdelingsJaar Ophalen(int afdelingsJaarID)
        {
            AfdelingsJaar aj = _dao.Ophalen(afdelingsJaarID, a => a.Afdeling, a => a.Leiding, a => a.Kind, a => a.OfficieleAfdeling);

            if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
            {
                return aj;
            }
            else
            {
                throw new GeenGavException(Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Verwijdert AfdelingsJaar uit database
        /// </summary>
        /// <param name="id">afdelingsJaarID</param>
        /// <returns>true on successful</returns>
        public bool Verwijderen(int afdelingsJaarID)
        {
            AfdelingsJaar aj = _dao.Ophalen(afdelingsJaarID, a => a.Afdeling, a => a.Leiding, a => a.Kind);
            
            if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
            {
                if (aj.Kind.Count != 0 || aj.Leiding.Count != 0)
                {
                    throw new InvalidOperationException("AfdelingsJaar kan niet verwijderd worden omdat er nog leden of leiding in deze afdeling zitten.");
                }
                else
                {
                    aj.TeVerwijderen = true;
                    _dao.Bewaren(aj);
                    return true;
                }
            }
            else
            {
                throw new GeenGavException(Resources.GeenGavGroep);
            }
            return false;
        }

        public void Bewaren(AfdelingsJaar aj)
        {
            if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
            {
                _dao.Bewaren(aj);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavLid);
            }
        }

    }
}
