using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.EfWrapper;
using Chiro.Cdf.EfWrapper.Entity;
using System.Diagnostics;

namespace Chiro.Gap.Orm
{
    [AssociationEndBehavior("Afdeling", Owned = true)]
    public partial class Groep : IBasisEntiteit 
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
            return 7;
        }

        public override bool Equals(object obj)
        {
            var andere = obj as Groep;
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
