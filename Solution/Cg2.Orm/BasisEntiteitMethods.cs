using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm
{
    /// <summary>
    /// Klasse met extension methods voor basisentiteiten.
    /// </summary>
    public static class BasisEntiteitMethods
    {
        const int DefaultID = 0;

        /// <summary>
        /// Sql rowversion als string, om gemakkelijk te kunnen gebruiken
        /// met MVC model binding in forms
        /// </summary>
        /// <param name="be">deze entiteit</param>
        /// <returns>stringrepresentatie van de rowversion</returns>
        public static string VersieStringGet(this IBasisEntiteit be)
        {
            return be.Versie == null ? "" : Convert.ToBase64String(be.Versie);
        }

        /// <summary>
        /// Omgekeerde bewerking van VersieStringGet
        /// </summary>
        /// <param name="be">deze entiteit</param>
        /// <param name="ver">stringrepresentatie van rowversion</param>
        public static void VersieStringSet(this IBasisEntiteit be, String ver)
        {
            be.Versie = Convert.FromBase64String(ver);
        }

    }
}
