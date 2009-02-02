using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Reflection;

namespace Cg2.Data
{
    /// <summary>
    /// De 'session factory' genereert 'NHibernate sessions'.  Deze
    /// klasse instantieert een singleton.
    /// </summary>
    public static class SessionFactory
    {
        private static ISessionFactory _sessionFactory = null;

        public static ISessionFactory Factory
        {
            get 
            {
                if (_sessionFactory == null)
                {
                    Configuration cfg = new Configuration();
                    cfg.Configure();
                    cfg.AddAssembly(Assembly.GetCallingAssembly());

                    _sessionFactory = cfg.BuildSessionFactory();
                }
                return _sessionFactory; 
            }
        }
    }
}
