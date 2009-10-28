using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Chiro.Gap.ServiceContracts.Mappers
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
