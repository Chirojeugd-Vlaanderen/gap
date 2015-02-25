using System.Threading.Tasks;

namespace Chiro.CiviSync.Helpers
{
    public interface IGapUpdateHelper
    {
        /// <summary>
        /// Configureert GapUpdate.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="path"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void Configureren(string server, string path, string username, string password);

        /// <summary>
        /// Rapporteer het gegeven <paramref name="adNummer"/> als ongeldig bij GAP.
        /// </summary>
        /// <param name="adNummer">Als ongeldig te rapporteren AD-nummer</param>
        Task OngeldigAdNaarGap(int adNummer);
    }
}