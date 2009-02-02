using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;

namespace Cg2.Core.DataInterfaces
{
    public interface IPersonenDao: IDao<Persoon>
    {
        Persoon OphalenMetCommunicatie(int id);
    }
}
