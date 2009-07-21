using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.ServiceContracts;
using Cg2.Workers;
using Cg2.Orm;
using Cg2.Ioc;

namespace Cg2.Services
{
    // NOTE: If you change the class name "LedenService" here, you must also update the reference to "LedenService" in Web.config.
    public class LedenService : ILedenService
    {
        public void LidMakenEnBewaren(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();
            LedenManager lm = Factory.Maak<LedenManager>();

            Lid l = lm.LidMaken(pm.Ophalen(gelieerdePersoonID));

            lm.LidBewaren(l);

            //lm.LidMaken(gelieerdePersoonID);
        }

        public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID/*, int pagina, int paginaGrootte, out int aantalOpgehaald*/)
        {
            LedenManager lm = Factory.Maak<LedenManager>();

            IList<Lid> result = lm.PaginaOphalen(groepsWerkJaarID/*, pagina paginaGrootte, out aantalOpgehaald*/);
            return LidInfoMapper.mapLid(result);
        }

        /// <summary>
        /// ook om te maken en te deleten
        /// </summary>
        /// <param name="persoon"></param>
        public void Bewaren(Lid lid)
        {
            LedenManager lm = Factory.Maak<LedenManager>();
            lm.LidBewaren(lid);
        }

        public void BewarenMetAfdelingen(Lid lid)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void BewarenMetFuncties(Lid lid)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void BewarenMetVrijeVelden(Lid lid)
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        public void LidOpNonactiefZetten(Lid lid)
        {
            LedenManager lm = Factory.Maak<LedenManager>();
            lm.LidOpNonactiefZetten(lid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        public void LidActiveren(Lid lid)
        {
            LedenManager lm = Factory.Maak<LedenManager>();
            lm.LidActiveren(lid);
        }

    }

}