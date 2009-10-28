using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp
{
    public static class Sessie
    {
        private const string LijstNaamString = "LaatsteLijst";
        private const string LijstPaginaString = "LaatsteLijstPagina";


        /// <summary>
        /// Bevat de naam van de controller van de laatst opgevraagde lijst
        /// </summary>
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

        /// <summary>
        /// Bevat de waarde van het ID-gedeelte van de url van de laatste lijst
        /// (indirectie alom ;))
        /// </summary>
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
