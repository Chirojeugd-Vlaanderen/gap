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
    }
}
