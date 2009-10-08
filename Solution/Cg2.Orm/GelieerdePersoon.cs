using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Cg2.EfWrapper;
using Cg2.EfWrapper.Entity;
using System.Diagnostics;

namespace Cg2.Orm
{
    // Als er een persoon met adressen over de service gestuurd wordt,
    // en een PersoonsAdres is uit de lijst met PersoonsAdressen 
    // verdwenen, dan is het de bedoeling dat
    // het PersoonsAdresobject mee verdwijnt uit de database.  Om daarvoor
    // te zorgen, is onderstaand attribuut AssociationEndBehavior
    // nodig.  (Als dat attribuut er niet zou staan, dan zou enkel
    // de koppeling tussen Persoon en Persoonsadres verdwijnen, en
    // dat heeft dan weer een key violation tot gevolg.)

    [AssociationEndBehavior("PersoonsAdres", Owned = true)]
    public partial class GelieerdePersoon : IBasisEntiteit 
    {
        // We gaan de lijst met PersoonsInfo niet opnemen in de
        // klasse.  De programmeur moet te allen tijde maar weten
        // welke informatie hij wel/niet opgevraagd heeft.

        // private IList<PersoonsInfo> _meeGeleverd;

        #region Standaarddingen IBasisEntiteit
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public GelieerdePersoon(): base()
        {
            // _meeGeleverd = null;
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
        #endregion


        #region Identity en equality

        // Op een collectie van GelieerdePersonen zou ik een
        // distinct willen kunnen uitvoeren.  Om dit correct te kunnen doen,
        // moeten Equals en GetHashCode aangepast worden.

        public override int GetHashCode()
        {
            int hashcode;

            if (ID != 0)
            {
                // de ID bepaalt op unieke manier de identiteit van de
                // GelieerdePersoon
                hashcode = ID.GetHashCode();
            }
            else
            {
                // Als er geen ID is, dan doen we een fallback naar de
                // GetHashCode van de parent, wat eigenlijk niet helemaal
                // correct is.
                hashcode = base.GetHashCode();
            }

            return hashcode;
        }

        public override bool Equals(object obj)
        {
            bool result;

            var andere = obj as GelieerdePersoon;

            if (andere == null)
            {
                result = false;
            }
            else if (ID == 0 || andere.ID == 0)
            {
                result = base.Equals(andere);
            }
            else
            {
                result = (ID == andere.ID);
            }

            return result;
        }

        #endregion
    }
}
