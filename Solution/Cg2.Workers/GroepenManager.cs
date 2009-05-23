using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;
using System.Diagnostics;

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
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de groepen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="dao">Alternatieve dao</param>
        public GroepenManager(IGroepenDao dao)
        {
            _dao = dao;
        }

        
        public void ToevoegenAfdeling(int groepID, string naam, string afkorting)
        {            
            Dao.ToevoegenAfdeling(groepID, naam, afkorting);
        }

        public void ToevoegenAfdelingsJaar(Groep g, Afdeling aj, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind)
        {
            if (geboortejaarbegin < System.DateTime.Today.Year - 20
                || geboortejaarbegin > geboortejaareind
                || geboortejaareind > System.DateTime.Today.Year - 5)
            {
                throw new InvalidOperationException("Ongeldige geboortejaren voor het afdelingsjaar");
            }
            Dao.ToevoegenAfdelingsJaar(g, aj, oa, geboortejaarbegin, geboortejaareind);
        }

        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            return Dao.OphalenOfficieleAfdelingen();
        }
    }
}
