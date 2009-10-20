using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace Chiro.CG2.ServiceContracts.Mappers
{
    public class GroepInfoMapper
    {
        public static GroepInfo mapGroep(Groep g)
        {
            return new GroepInfo()
            {
                ID = g.ID,
                Groepsnaam = g.Naam,
                Plaats = (g is ChiroGroep ? (g as ChiroGroep).Plaats : Properties.Resources.NietVanToepassing),
                StamNummer = g.Code == null ? String.Empty : g.Code.ToUpper()
            };
        }

        public static IList<GroepInfo> mapGroepen(IEnumerable<Groep> groepen)
        {
            IList<GroepInfo> giList = new List<GroepInfo>();

            foreach (var gr in groepen)
            {
                giList.Add(mapGroep(gr));
            }
            return giList;
        }

    }
}
