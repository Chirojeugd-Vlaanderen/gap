using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm
{
    public interface IBasisEntiteit
    {
        int ID { get; set; }
        byte[] Versie { get; set; }
        string VersieString { get; }
        Guid BusinessKey { get; set; }
    }
}
