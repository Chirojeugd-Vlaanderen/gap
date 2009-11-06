using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    public interface IAfdelingsJarenDao : IDao<AfdelingsJaar>
    {
        AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID);
    }
}
