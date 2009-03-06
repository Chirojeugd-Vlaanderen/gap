using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm
{
    public partial class Lid: IBasisEntiteit
    {
        public Lid()
        {
            BusinessKey = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return this.MyEquals(obj);
        }

        public override int GetHashCode()
        {
            return this.MyGetHashCode();
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }
}
