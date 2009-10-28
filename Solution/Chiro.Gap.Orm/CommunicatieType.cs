using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.EfWrapper;

namespace Chiro.Gap.Orm
{
    public partial class CommunicatieType: IBasisEntiteit
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

        public override int GetHashCode()
        {
            return 14;
        }
    }
}
