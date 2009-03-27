using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm
{
    public partial class Adres: IBasisEntiteit
    {
        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }
}
