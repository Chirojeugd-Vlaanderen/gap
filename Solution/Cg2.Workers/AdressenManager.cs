using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Workers
{
    public class AdressenManager
    {
        private IAdressenDao _dao;

        public AdressenManager()
        {
            _dao = new AdressenDao();
        }

        public AdressenManager(IAdressenDao dao)
        {
            _dao = dao;
        }

        #region proxy naar data acces

        public Adres AdresMetBewonersOphalen(int adresID, IList<int> gelieerdAan)
        {
            return _dao.AdresMetBewonersOphalen(adresID, gelieerdAan);
        }

        #endregion

    }
}
