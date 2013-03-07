using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Repositories
{

    /// <summary>
    /// Specifieke repository voor gemeenschappelijke data access voor Leden en hun aanhangsels
    /// </summary>
    public class LedenRepository : Repository<Lid>, ILedenRepository
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LedenRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LedenRepository(IContext context)
            : base(context)
        {
        }
    }
}
