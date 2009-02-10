using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm
{
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
    }
}
