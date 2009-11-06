using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
    // Als een link naar PersoonsAdres verwijderd wordt, dan
    // moet het persoonsadres zelf ook verwijderd worden.
    // Vandaar het attribuut AssociationEndBehavior
    [AssociationEndBehavior("PersoonsAdres", Owned = true)]
    public partial class Adres: IEfBasisEntiteit
    {
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public Adres(): base()
        {
            // Bus en PostCode zijn niet nullable.
            // Als er geen bus/postcode is, dan moet er
            // gewoon een lege string staan.

            Bus = "";
            PostCode = "";
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }

        public override int GetHashCode()
        {
            return 19;
        }
    }
}
