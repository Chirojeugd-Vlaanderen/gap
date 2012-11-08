using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;

namespace Chiro.Gap.Sync
{
    public class GroepenSync: IGroepenSync
    {
        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        public void Bewaren(Groep g)
        {
            Kip.ServiceContracts.DataContracts.Groep syncGroep =
                Mapper.Map<Groep, Kip.ServiceContracts.DataContracts.Groep>(g);

            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.GroepUpdaten(syncGroep));
        }
    }
}
