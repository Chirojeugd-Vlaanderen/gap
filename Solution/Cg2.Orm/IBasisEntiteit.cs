using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Cg2.Orm
{
    public interface IBasisEntiteit: IEntityWithKey
    {
        int ID { get; set; }
        byte[] Versie { get; set; }
        string VersieString { get; }
    }
}
