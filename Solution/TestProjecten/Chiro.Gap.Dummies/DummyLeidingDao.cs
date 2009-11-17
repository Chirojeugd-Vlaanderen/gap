using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy LeidingDao die niets implementeert
    /// </summary>
    public class DummyLeidingDao: DummyDao<Leiding>, ILeidingDao
    {
    }
}
