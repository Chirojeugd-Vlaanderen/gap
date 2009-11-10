using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using AutoMapper;

namespace Chiro.Gap.ServiceContracts.Mappers
{
    public class LidInfoMapper
    {
        public static LidInfo mapLid(Lid lid)
        {
            LidInfo l = new LidInfo()
            {
                PersoonInfo = Mapper.Map<GelieerdePersoon, PersoonInfo>(lid.GelieerdePersoon),
                LidgeldBetaald = lid.LidgeldBetaald,
                LidID = lid.ID
            };
            if (lid is Kind)
            {
                l.Type = LidType.Kind;
                Kind k = (Kind) lid;
                if (k.AfdelingsJaar == null)
                {
                    l.AfdelingsNamen = "null";
                }
                else
                {
                    l.AfdelingsNamen = k.AfdelingsJaar.Afdeling.Naam;
                }
            }
            else if (lid is Leiding)
            {
                l.Type = LidType.Leiding;
                l.AfdelingsNamen = "";
                Leiding leiding = (Leiding) lid;
                foreach (AfdelingsJaar aj in leiding.AfdelingsJaar)
                {
                    l.AfdelingsNamen = l.AfdelingsNamen + (l.AfdelingsNamen.Equals("") ? "" : ",") + " " + aj.Afdeling.Naam;
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
