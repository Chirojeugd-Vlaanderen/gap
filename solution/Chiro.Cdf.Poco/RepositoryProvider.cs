namespace Chiro.Cdf.Poco
{
    /// <summary>
    /// Een repository provider levert eender welke repository af, en de
    /// (gedeelde) context van al die repository's. 
    /// </summary>
    /// <remarks>
    /// Een repository wordt typisch
    /// door de IOC-container gemaakt op het niveau van de service, en liefst
    /// nergens anders. Op die manier vermijden we dat er verschillende contexten
    /// in elkaars weg gaan lopen.
    /// </remarks>
    public class RepositoryProvider : IRepositoryProvider
    {
        private readonly IContext _context;

        public RepositoryProvider(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Vraagt de (gedeelde) context van alle repository's op
        /// </summary>
        /// <returns>de (gedeelde) context van alle repository's</returns>
        public IContext ContextGet()
        {
            return _context;
        }

        /// <summary>
        /// Creeert een repository voor entiteiten van type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">type van entiteit waarvoor repository gevraagd</typeparam>
        /// <returns>Een repository voor entiteiten van type <typeparamref name="TEntity"/></returns>
        public IRepository<TEntity> RepositoryGet<TEntity>() where TEntity : BasisEntiteit
        {
            return new Repository<TEntity>(_context);
        }
    }
}
