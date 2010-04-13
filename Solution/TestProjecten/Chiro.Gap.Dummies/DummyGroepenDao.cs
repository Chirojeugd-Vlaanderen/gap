using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;


namespace Chiro.Gap.Dummies
{
    public class DummyGroepenDao: DummyDao<Groep>, IGroepenDao
    {
        #region IGroepenDao Members

        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
        {
            throw new NotImplementedException();
        }

        public Groep OphalenMetGroepsWerkJaren(int groepID)
        {
            throw new NotImplementedException();
        }

        public Groep OphalenMetAfdelingen(int groepID)
        {
            throw new NotImplementedException();
        }

        public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
        {
            throw new NotImplementedException();
        }

        public GroepsWerkJaar GroepsWerkJaarOphalen(int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }

        #endregion

	#region IGroepenDao Members


	public OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID)
	{
		throw new NotImplementedException();
	}

	#endregion
    }
}
