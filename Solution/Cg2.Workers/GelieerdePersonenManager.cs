using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;

namespace Cg2.Workers
{
    public class GelieerdePersonenManager
    {
        private IGelieerdePersonenDao _dao;
        private IGroepenDao _groepenDao;

        public IGelieerdePersonenDao Dao
        {
            get { return _dao; }
        }

        public GelieerdePersonenManager()
        {
            _dao = new GelieerdePersonenDao();
        }

        public GelieerdePersonenManager(IGelieerdePersonenDao dao, IGroepenDao groepenDao)
        {
            _dao = dao;
            _groepenDao = groepenDao;
        }


    }
}
