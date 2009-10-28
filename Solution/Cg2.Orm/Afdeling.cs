using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.EfWrapper;

namespace Cg2.Orm
{
    public partial class Afdeling : IBasisEntiteit 
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
            return 17;
        }

        public override bool Equals(object obj)
        {
            IBasisEntiteit andere = obj as Afdeling;
            // Als obj geen Afdeling is, wordt andere null.

            return andere != null && (ID != 0) && (ID == andere.ID)
                || (ID == 0 || andere.ID == 0) && base.Equals(andere);

            // Is obj geen Afdeling, dan is de vergelijking altijd vals.
            // Hebben beide objecten een ID verschillend van 0, en zijn deze
            // ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
            // base.Equals, wat eigenlijk niet helemaal correct is.
        }

        #endregion
    }
}
