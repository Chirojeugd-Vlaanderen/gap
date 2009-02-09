using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    /// <summary>
    /// De PersonenDAO moet meer kunnen dan de standaard-CRUD-operaties:
    /// een persoon moet bijvoorbeeld ook samen met zijn contacten
    /// opgehaald kunnen worden.  Op die manier volstaat de generische
    /// IDao niet meer.
    /// </summary>
    public interface IPersonenDao: IDao<Persoon>
    {
        Persoon OphalenMetCommunicatie(int id);
    }
}
