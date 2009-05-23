using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using System.Diagnostics;

namespace Cg2.Workers
{
    public class WerkjaarManager
    {

        private IGroepenDao _dao;

        public IGroepenDao Dao
        {
            get { return _dao; }
        }

        #region Constructors

        public WerkjaarManager(IGroepenDao groepenDao)
        {
            _dao = groepenDao;
        }

        #endregion

        public int OphalenHuidigGroepsWerkjaar(int groepID)
        {

            var begindatumnieuwwerkjaar = Properties.Settings.Default.WerkjaarStartNationaal;
            var deadlinenieuwwerkjaar = Properties.Settings.Default.WerkjaarVerplichteOvergang;
            var huidigedatum = System.DateTime.Today;

            if (compare(huidigedatum.Day, huidigedatum.Month, begindatumnieuwwerkjaar.Day, begindatumnieuwwerkjaar.Month) < 0)
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
                    int werkjaar = _dao.OphalenHuidigeGroepsWerkjaar(groepID).WerkJaar;
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
    }
}
