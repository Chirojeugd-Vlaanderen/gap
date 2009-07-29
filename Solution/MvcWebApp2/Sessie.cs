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
        private const string GroepSessieIDString = "MijnGroepID";
        private const string MultiGavString = "MultiGav";
        private const string LijstNaamString = "LaatsteLijst";
        private const string LijstPaginaString = "LaatsteLijstPagina";

        public static int GroepID
        {
            get
            {
                int? groepID = HttpContext.Current.Session[GroepSessieIDString] as int?;
                return groepID == null ? 0 : (int)groepID;
            }
            set
            {
                HttpContext.Current.Session[GroepSessieIDString] = value;
            }
        }

        public static Boolean IsMultiGav
        {
            get
            {
                bool? isMultiGav = HttpContext.Current.Session[MultiGavString] as bool?;
                return isMultiGav == null ? false : (bool)isMultiGav;
            }
            set
            {
                HttpContext.Current.Session[MultiGavString] = value;
            }
        }

        public static String LaatsteLijst
        {
            get
            {
                String lijstnaam = HttpContext.Current.Session[LijstNaamString] as String;
                return lijstnaam == null ? string.Empty : (String)lijstnaam;
            }
            set
            {
                HttpContext.Current.Session[LijstNaamString] = value;
            }
        }

        public static int LaatstePagina
        {
            get
            {
                int? paginanummer = HttpContext.Current.Session[LijstPaginaString] as int?;
                return paginanummer == null ? 1 : (int)paginanummer;
            }
            set
            {
                HttpContext.Current.Session[LijstPaginaString] = value;
            }
        }
    }
}
