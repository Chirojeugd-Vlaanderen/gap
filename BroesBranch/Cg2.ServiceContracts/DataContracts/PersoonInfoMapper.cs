using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    public class PersoonInfoMapper
    {
        public static PersoonInfo mapPersoon(GelieerdePersoon gp)
        {
            PersoonInfo p = new PersoonInfo()
            {
                AdNummer = gp.Persoon.AdNummer,
                GelieerdePersoonID = gp.ID,
                GeboorteDatum = gp.Persoon.GeboorteDatum,
                VolledigeNaam = gp.Persoon.VolledigeNaam,
                Geslacht = gp.Persoon.Geslacht,
                IsLid = gp.Lid.Count > 0
            };
            return p;
        }

        public static IList<PersoonInfo> mapPersoon(IList<GelieerdePersoon> gps)
        {
            IList<PersoonInfo> map = new List<PersoonInfo>();
            foreach (var gp in gps)
            {
                map.Add(mapPersoon(gp));
            }
            return map;
        }

    }
}
