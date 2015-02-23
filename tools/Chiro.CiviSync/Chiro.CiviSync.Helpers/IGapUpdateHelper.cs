using System.Threading.Tasks;

namespace Chiro.CiviSync.Helpers
{
    public interface IGapUpdateHelper
    {
        /// <summary>
        /// Configureert GapUpdate.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void Configureren(string url, string username, string password);

        /// <summary>
        /// Rapporteer het gegeven <paramref name="adNummer"/> als ongeldig bij GAP.
        /// </summary>
        /// <param name="adNummer">Als ongeldig te rapporteren AD-nummer</param>
        Task OngeldigAdNaarGap(int adNummer);
    }
}