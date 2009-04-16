using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cg2.Orm;
using System.ServiceModel;
using Cg2.ServiceContracts;

namespace MvcWebApp2.ServiceCalls
{
    public static class Leden
    {
        private const string _endpointConfig = "LedenServiceEndPoint";

        /// <summary>
        /// Haalt persoonsinfo van leden van een bepaalde groep op
        /// </summary>
        /// <param name="aantal">bevat achteraf het aantal effectief opgehaalde leden</param>
        /// <param name="groepID">ID van groep waarvan leden opgevraagd moeten worden</param>
        /// <param name="pagina">te tonen pagina (>= 1)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <returns></returns>
        public static IList<Lid> PaginaOphalen(out int aantal, int groepID, int pagina, int paginaGrootte)
        {
            IList<Lid> res;
            using (ChannelFactory<ILedenService> cf = new ChannelFactory<ILedenService>(_endpointConfig))
            {
                ILedenService service = cf.CreateChannel();
                try
                {
                    res = service.PaginaOphalen(groepID, pagina, paginaGrootte, out aantal);
                }
                finally
                {
                    ((IClientChannel)service).Close();
                }
            }
            return res;
        }

        /// <summary>
        /// Maakt een gelieerde persoon lid (zonder meer; vrij lege functie)
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        public static void LidMakenEnBewaren(int gelieerdePersoonID)
        {
            using (ChannelFactory<ILedenService> cf = new ChannelFactory<ILedenService>(_endpointConfig))
            {
                ILedenService service = cf.CreateChannel();
                try
                {
                    service.LidMakenEnBewaren(gelieerdePersoonID);
                }
                finally
                {
                    ((IClientChannel)service).Close();
                }
            }
        }
    }
}
