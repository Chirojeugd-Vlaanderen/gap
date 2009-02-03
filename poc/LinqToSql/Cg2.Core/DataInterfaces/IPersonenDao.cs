using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using Cg2.Core.Domain;

namespace Cg2.Core.DataInterfaces
{
    interface IPersonenDao: IDao<Persoon>
    {
        Persoon OphalenMetCommunicatie(int id);
    }
}
