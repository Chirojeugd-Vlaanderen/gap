namespace Chiro.Cdf.Poco
{
    public interface IRepositoryProvider
    {
        IContext ContextGet();
        IRepository<TEntity> RepositoryGet<TEntity>() where TEntity : class;
    }
}
