using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cg2.Adf.ServiceModel;
using Cg2.ServiceContracts;

namespace MvcWebApp2
{
    public static class Sessie
    {
        private const string GroepSessieID = "MijnGroepID";
        private const string MultiGav = "MultiGav";
        private const string LijstNaam = "LaatsteLijst";
        private const string LijstPagina = "LaatsteLijstPagina";

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

        public static Boolean IsMultiGav
        {
            get
            {
                Boolean isMultiGav = Convert.ToBoolean(HttpContext.Current.Session[MultiGav].ToString());
                return isMultiGav;
            }
            set
            {
                HttpContext.Current.Session[MultiGav] = value;
            }
        }

        public static String LaatsteLijst
        {
            get
            {
                String lijstnaam = HttpContext.Current.Session[LijstNaam] as String;
                return lijstnaam == null ? string.Empty : (String)lijstnaam;
            }
            set
            {
                HttpContext.Current.Session[LijstNaam] = value;
            }
        }

        public static int LaatstePagina
        {
            get
            {
                int? paginanummer = HttpContext.Current.Session[LijstPagina] as int?;
                return paginanummer == null ? 1 : (int)paginanummer;
            }
            set
            {
                HttpContext.Current.Session[LijstPagina] = value;
            }
        }
    }
}
