using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CgDal
{
    public sealed class DBFactory
    {
        static ChiroGroepEntities instance = null;
        static readonly object padlock = new object();

        DBFactory()
        {
        }

        public static ChiroGroepEntities Databaseinstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ChiroGroepEntities();
                    }
                    return instance;
                }
            }
        }
    }

}
