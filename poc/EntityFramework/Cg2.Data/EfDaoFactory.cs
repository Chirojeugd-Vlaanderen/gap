using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;

namespace Cg2.Data.Ef
{
    /// <summary>
    /// Toegang tot de DAO-classes.  Dit bestand bevat de concrete
    /// impelementatie van de interafces in Cg2.Core.DataInterfaces.
    /// 
    /// Aangepast uit het boek 'Pro LINQ Object Relational Mapping
    /// with C# 2008'
    /// </summary>
    public class EfDaoFactory: IDaoFactory
    {
        #region IDaoFactory Members

        public IGroepenDao GroepenDaoGet()
        {
            throw new NotImplementedException();
        }

        public IChiroGroepenDao ChiroGroepenDaoGet()
        {
            throw new NotImplementedException();
        }

        public IPersonenDao PersonenDaoGet()
        {
            throw new NotImplementedException();
        }

        public ICommunicatieVormenDao CommunicatieVormenDaoGet()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
