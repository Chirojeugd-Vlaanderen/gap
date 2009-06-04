using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebApp2
{
    public static class Sessie
    {
        private const string GroepSessieID = "MijnGroepID";

        public static int GroepID
        {
            get
            {
                int? groepID = HttpContext.Current.Session[GroepSessieID] as int?;
                return groepID == null ? 0 : (int)groepID;
            }
            set
            {
                HttpContext.Current.Session[GroepSessieID] = value;
            }
        }
    }
}
