using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Context;

namespace Chiro.Gap.Poco.Repository
{
    public class GapRepositoryProvider: IRepositoryProvider
    {
        private readonly ChiroGroepEntities _context;

        public GapRepositoryProvider()
        {
            _context = new ChiroGroepEntities();
        }

        public IContext ContextGet()
        {
            return _context;
        }

        public IRepository<TEntity> RepositoryGet<TEntity>() where TEntity : class
        {
            return new GapRepository<TEntity>(_context);
        }
    }
}
