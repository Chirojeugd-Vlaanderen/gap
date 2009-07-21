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

        // haalt alle AfdelingsJaren op bij een gegeven Groep en GroepsWerkJaar
        // Groep en GroepsWerkJaar zijn allebei parameters omdat GroepsWerkJaar.Groep soms null is
        // maakt gebruik van OphalenMetAfdelingen, filtert dan de afdelingsjaren
        // die overeenkomen met het gegeven GroepsWerkJaar
        public IList<AfdelingsJaar> OphalenAfdelingsJaren(Groep groep, GroepsWerkJaar gwj)
        {
            IList<AfdelingsJaar> result = new List<AfdelingsJaar>();
            Groep g = Dao.OphalenMetAfdelingen(groep.ID);

            int afdelingCnt = g.Afdeling.Count;

            foreach (Afdeling a in g.Afdeling) {

                int jaarCnt = a.AfdelingsJaar.Count;

                foreach (AfdelingsJaar j in a.AfdelingsJaar) {
                    if (j.GroepsWerkJaar.ID == gwj.ID) {
                        result.Add(j);
                    }
                }
            }
            return result;
        }
    }
}
