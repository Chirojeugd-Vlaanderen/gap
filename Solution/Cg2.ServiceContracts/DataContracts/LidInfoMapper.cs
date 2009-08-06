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
                LidgeldBetaald = lid.LidgeldBetaald,
                LidID = lid.ID
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
                l.AfdelingString = "";
                Leiding leiding = (Leiding) lid;
                foreach (AfdelingsJaar aj in leiding.AfdelingsJaar)
                {
                    l.AfdelingString = l.AfdelingString + (l.AfdelingString.Equals("") ? "" : ",") + " " + aj.Afdeling.AfdelingsNaam;
                }
            }
            else
            {
                throw new InvalidProgramException("Een lid moet leiding of kind zijn.");
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
