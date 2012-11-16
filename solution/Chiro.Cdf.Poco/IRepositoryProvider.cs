namespace Chiro.Cdf.Poco
{
    /// <summary>
    /// Een repository provider levert eender welke repository af, en de
    /// (gedeelde) context van al die repository's.
    /// </summary>
    public interface IRepositoryProvider
    {
        /// <summary>
        /// Vraagt de (gedeelde) context van alle repository's op
        /// </summary>
        /// <returns>de (gedeelde) context van alle repository's</returns>
        IContext ContextGet();
        /// <summary>
        /// Creeert een repository voor entiteiten van type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">type van entiteit waarvoor repository gevraagd</typeparam>
        /// <returns>Een repository voor entiteiten van type <typeparamref name="TEntity"/></returns>
        IRepository<TEntity> RepositoryGet<TEntity>() where TEntity : class;
    }
}
