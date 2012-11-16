namespace Chiro.Cdf.Poco
{
    public class RepositoryProvider : IRepositoryProvider
    {
        private readonly IContext _context;

        public RepositoryProvider(IContext context)
        {
            _context = context;
        }

        public IContext ContextGet()
        {
            return _context;
        }

        public IRepository<TEntity> RepositoryGet<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_context);
        }
    }
}
