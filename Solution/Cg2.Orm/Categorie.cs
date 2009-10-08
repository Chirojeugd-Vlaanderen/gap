using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.EfWrapper;
using System.Diagnostics;

namespace Cg2.Orm
{
    public partial class Categorie: IBasisEntiteit
    {
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
        #region Identity en equality

        public override int GetHashCode()
        {
            int result;

            if (ID != 0)
            {
                // de ID bepaalt op unieke manier de identiteit
                result = ID.GetHashCode();
            }
            else
            {
                // Als er geen ID is, dan doen we een fallback naar de
                // GetHashCode van de parent, wat eigenlijk niet helemaal
                // correct is.
                result = base.GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            var andere = obj as Categorie;
            // Als obj geen Groep is, wordt andere null.

            if (andere == null)
            {
                return false;
            }
            else if (ID == 0 || andere.ID == 0)
            {
                return base.Equals(andere);
            }
            else
            {
                return ID.Equals(andere.ID);
            }
        }

        #endregion
    }
}
