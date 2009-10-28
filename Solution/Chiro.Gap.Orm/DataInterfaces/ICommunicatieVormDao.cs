using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Orm.DataInterfaces
{
    public interface ICommunicatieVormDao: IDao<CommunicatieVorm>
    {
        IList<CommunicatieVorm> ZoekenOpNummer(string zoekString);
    }
}
