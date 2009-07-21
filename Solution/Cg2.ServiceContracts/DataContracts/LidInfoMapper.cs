using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    public class LidInfoMapper
    {
        public static LidInfo mapLid(Lid lid)
        {
            LidInfo l = new LidInfo()
            {
                PersoonInfo = PersoonInfoMapper.mapPersoon(lid.GelieerdePersoon),
                LidgeldBetaald = lid.LidgeldBetaald
            };
            if (lid is Kind)
            {
                l.Type = LidType.Kind;
                Kind k = (Kind) lid;
                if (k.AfdelingsJaar == null)
                {
                    l.AfdelingString = "null";
                }
                else
                {
                    l.AfdelingString = k.AfdelingsJaar.Afdeling.AfdelingsNaam;
                }
            }
            else if (lid is Leiding)
            {
                l.Type = LidType.Leiding;
                l.AfdelingString = "not implemented";
            }
            else
            {
                l.Type = LidType.Onbekend;
            }
            return l;
        }

        public static IList<LidInfo> mapLid(IList<Lid> leden)
        {
            IList<LidInfo> map = new List<LidInfo>();
            foreach (var lid in leden)
            {
                map.Add(mapLid(lid));
            }
            return map;
        }

    }
}
