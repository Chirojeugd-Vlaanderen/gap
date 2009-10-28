using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;

namespace Cg2.Validatie
{
    public class PersonenValidator: Validator<Persoon>
    {
        public override bool Valideer(Persoon p)
        {
            return (p.GeboorteDatum <= DateTime.Now) && (p.Naam.Length > 0) 
                && base.Valideer(p);
        }
    }
}
