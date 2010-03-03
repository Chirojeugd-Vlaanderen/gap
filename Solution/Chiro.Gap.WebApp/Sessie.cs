// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp
{
    public static class Sessie
    {
        private const string LijstNaamString = "LaatsteLijst";
        private const string ActieIDString = "LaatsteActieID";
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

        /// <summary>
        /// Bevat de waarde van het actie gedeelte van de url van de laatste lijst
        /// (indirectie alom ;))
        /// </summary>
        public static int LaatsteActieID
        {
            get
            {
                int ? i = HttpContext.Current.Session[ActieIDString] as int?;
                return i == null ? 0 : (int)i;
            }
            set
            {
                HttpContext.Current.Session[ActieIDString] = value;
            }
        }
    }
}
