using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
    public interface IGroepenSync
    {
        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        void Bewaren(Groep g);
    }
}
