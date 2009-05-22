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
            Dao.ToevoegenAfdelingsJaar(g, aj, oa, geboortejaarbegin, geboortejaareind);
        }
  
        public int OphalenGroepsWerkjaar(int groepID, DateTime begindatumnieuwwerkjaar, DateTime deadlinenieuwwerkjaar)
        {
            var huidigedatum = System.DateTime.Today;

            if ( compare(huidigedatum.Day, huidigedatum.Month, begindatumnieuwwerkjaar.Day, begindatumnieuwwerkjaar.Month)<0)
            {
                return huidigedatum.Year;
            }
            else
            {
                if (compare(deadlinenieuwwerkjaar.Day, deadlinenieuwwerkjaar.Month, huidigedatum.Day, huidigedatum.Month) < 0)
                {
                    return huidigedatum.Year;
                }
                else
                {
                    int werkjaar = Dao.OphalenNieuwsteGroepsWerkjaar(groepID).WerkJaar;
                    Debug.Assert(huidigedatum.Year == werkjaar || werkjaar + 1 == huidigedatum.Year);
                    return werkjaar;
                }
            }
        }

        private int compare(int dag1, int maand1, int dag2, int maand2)
        {
            if (maand1 < maand2 || (maand1 == maand2 && dag1 < dag2))
            {
                return -1;
            }
            else
            {
                if (maand1 > maand2 || (maand1 == maand2 && dag1 > dag2))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            return Dao.OphalenOfficieleAfdelingen();
        }
    }
}
