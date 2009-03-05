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

        public static int MyGetHashCode(this IBasisEntiteit be)
        {
            return be.BusinessKey.GetHashCode();
        }

        public static bool MyEquals(this IBasisEntiteit ik, object jij)
        {
            IBasisEntiteit gij = jij as IBasisEntiteit;

            return gij != null
                && (
                    ik.ID == gij.ID
                    || (ik.ID == DefaultID || gij.ID == DefaultID)
                        && ik.BusinessKey == gij.BusinessKey
                    ) && ik.GetType() == jij.GetType();
        }

        public static string VersieStringGet(this IBasisEntiteit be)
        {
            return be.Versie == null ? "" : Convert.ToBase64String(be.Versie);
        }

        public static void VersieStringSet(this IBasisEntiteit be, String ver)
        {
            be.Versie = Convert.FromBase64String(ver);
        }

    }
}
