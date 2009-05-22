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
        public Lid LidMakenEnBewaren(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();
            LedenManager lm = Factory.Maak<LedenManager>();

            Lid l = lm.LidMaken(pm.Ophalen(gelieerdePersoonID));

            return lm.Dao.Bewaren(l);
        }

        public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            LedenManager lm = Factory.Maak<LedenManager>();

            IList<Lid> result = lm.Dao.PaginaOphalen(groepsWerkJaarID, pagina, paginaGrootte, out aantalOpgehaald);
            return mapLid(result);
        }

        /// <summary>
        /// ook om te maken en te deleten
        /// </summary>
        /// <param name="persoon"></param>
        public void LidBewaren(Lid lid)
        {
            LedenManager lm = Factory.Maak<LedenManager>();
            lm.LidBewaren(lid);
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

        #region mappers

        private LidInfo mapLid(Lid lid)
        {
            return new LidInfo() {
                AdNummer = lid.GelieerdePersoon.Persoon.AdNummer,
                GelieerdePersoonID = lid.GelieerdePersoon.ID,
                GeboorteDatum = lid.GelieerdePersoon.Persoon.GeboorteDatum,
                VolledigeNaam = lid.GelieerdePersoon.Persoon.VolledigeNaam,
                Geslacht = lid.GelieerdePersoon.Persoon.Geslacht,
                IsLid = (lid.GelieerdePersoon.Lid.Count > 0),
                LidgeldBetaald = lid.LidgeldBetaald
            };
        }

        private IList<LidInfo> mapLid(IList<Lid> leden)
        {
            IList<LidInfo> map = new List<LidInfo>();
            foreach (var lid in leden)
            {
                map.Add(mapLid(lid));
            }
            return map;
        }

        #endregion

    }

}