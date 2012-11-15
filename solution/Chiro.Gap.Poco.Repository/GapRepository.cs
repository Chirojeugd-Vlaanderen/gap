using Chiro.Cdf.Poco.Entity;
using Chiro.Gap.Poco.Context;

namespace Chiro.Gap.Poco.Repository
{
    /// <summary>
    /// Repository specifiek voor GAP, op basis van entity framework.
    /// </summary>
    /// <typeparam name="TEntity">Klasse; deze repository voert database-operaties uit voor entiteiten
    /// van dit type</typeparam>
    public class GapRepository<TEntity>: EfRepository<ChiroGroepEntities, TEntity> where TEntity : class
    {
        public GapRepository(ChiroGroepEntities context) : base(context)
        {
        }
    }
}
