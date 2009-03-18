using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    public interface IAuthorisatieDao: IDao<GebruikersRecht>
    {
        GebruikersRecht GebruikersRechtOphalen(string login, int groepID);
    }
}
