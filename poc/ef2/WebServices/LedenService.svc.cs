using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.ServiceContracts;
using Cg2.Workers;
using Cg2.Orm;

namespace Cg2.Services
{
    // NOTE: If you change the class name "LedenService" here, you must also update the reference to "LedenService" in Web.config.
    public class LedenService : ILedenService
    {
        public Lid LidMakenEnBewaren(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            LedenManager lm = new LedenManager();

            Lid l = lm.LidMaken(pm.Dao.Ophalen(gelieerdePersoonID));
            
            return lm.Dao.Bewaren(l);
        }

        public IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            LedenManager lm = new LedenManager();

            IList<Lid> result = lm.Dao.PaginaOphalen(groepsWerkJaarID, pagina, paginaGrootte, out aantalOpgehaald);

            return result;
        }
        
        public IList<Lid> LedenOphalenMetInfo(string name, IList<LidInfo> gevraagd) //andere searcharg
        {
            LedenManager lm = new LedenManager();
            return lm.LedenOphalenMetInfo(name, gevraagd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public IList<Lid> LidOphalenMetInfo(int lidID, string name, IList<LidInfo> gevraagd) //andere searcharg
        {
            LedenManager lm = new LedenManager();
            return lm.LidOphalenMetInfo(lidID, name, gevraagd);
        }

        /// <summary>
        /// ook om te maken en te deleten
        /// </summary>
        /// <param name="persoon"></param>
        public void LidBewaren(Lid lid)
        {
            LedenManager lm = new LedenManager();
            lm.LidBewaren(lid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        public void LidOpNonactiefZetten(Lid lid)
        {
            LedenManager lm = new LedenManager();
            lm.LidOpNonactiefZetten(lid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        public void LidActiveren(Lid lid)
        {
            LedenManager lm = new LedenManager();
            lm.LidActiveren(lid);
        }
    }
}
