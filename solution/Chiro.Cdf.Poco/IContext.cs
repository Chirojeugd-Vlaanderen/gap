using System.Data.Entity;

namespace Chiro.Cdf.Poco
{
    public interface IContext
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}