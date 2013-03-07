using System;
using System.Data.Entity;

namespace Chiro.Cdf.Poco
{
    public interface IContext: IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}