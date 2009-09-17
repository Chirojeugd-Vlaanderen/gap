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
        public String LidMakenEnBewaren(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();
            LedenManager lm = Factory.Maak<LedenManager>();
            GelieerdePersoon gp = pm.DetailsOphalen(gelieerdePersoonID);

            Lid l = lm.LidMaken(gp);
            lm.LidBewaren(l);
            // TODO: feedback aanpassen (controleren of lid effectief is toegevoegd)
            return gp.Persoon.VolledigeNaam + " is toegevoegd als lid.";
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

        public Boolean Verwijderen(int id)
        {
            LedenManager lm = Factory.Maak<LedenManager>();
            return lm.LidVerwijderen(id);
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


        /// <summary>
        /// Haalt een pagina op met info over alle leden in een
        /// gegeven groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
        /// <returns>Lijst met LidInfo</returns>
        public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID)
        {
            LedenManager lm = Factory.Maak<LedenManager>();

            IList<Lid> result = lm.PaginaOphalen(groepsWerkJaarID);
            return LidInfoMapper.mapLid(result);
        }

        public IList<LidInfo> PaginaOphalenVoorAfdeling(int groepsWerkJaarID, int afdelingsID)
        {
            LedenManager lm = Factory.Maak<LedenManager>();

            IList<Lid> result = lm.PaginaOphalen(groepsWerkJaarID, afdelingsID);
            return LidInfoMapper.mapLid(result);
        }
    }
}