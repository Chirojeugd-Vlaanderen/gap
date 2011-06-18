using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace Algemeen.Data.Entity
{

    public interface IDbContext : IDisposable, IObjectContextAdapter
    {
        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        IEnumerable<DbEntityValidationResult> GetValidationErrors();

        //void OnModelCreating(DbModelBuilder modelBuilder);
        int SaveChanges();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);
        //bool ShouldValidateEntity(DbEntityEntry entityEntry);


        DbChangeTracker ChangeTracker { get; }
        DbContextConfiguration Configuration { get; }
        Database Database { get; }

    }

}
