using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Workers
{
    public class GroepenManager
    {
        private IGroepenDao _dao;

        public IGroepenDao Dao
        {
            get { return _dao; }
        }

        /// <summary>
        /// In de standaardconstructor wordt een standaard repository
        /// (GroepenDao) aangemaakt.
        /// </summary>
        public GroepenManager()
        {
            _dao = new GroepenDao();
        }

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de groepen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="dao">Alternatieve dao</param>
        public GroepenManager(IGroepenDao dao)
        {
            _dao = dao;
        }
    }
}
