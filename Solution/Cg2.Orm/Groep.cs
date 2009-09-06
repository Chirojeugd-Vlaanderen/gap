using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cg2.EfWrapper;
using Cg2.EfWrapper.Entity;

namespace Cg2.Orm
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
            if (ID != 0)
            {
                // de ID bepaalt op unieke manier de identiteit
                return ID.GetHashCode();
            }
            else
            {
                // Als er geen ID is, dan doen we een fallback naar de
                // GetHashCode van de parent, wat eigenlijk niet helemaal
                // correct is.
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            IBasisEntiteit andere = obj as Groep;
            // Als obj geen Groep is, wordt andere null.

            return andere != null && (ID != 0) && (ID == andere.ID)
                || (ID == 0 || andere.ID == 0) && base.Equals(andere);

            // Is obj geen Groep, dan is de vergelijking altijd vals.
            // Hebben beide objecten een ID verschillend van 0, en zijn deze
            // ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
            // base.Equals, wat eigenlijk niet helemaal correct is.
        }

        #endregion
    }
}
