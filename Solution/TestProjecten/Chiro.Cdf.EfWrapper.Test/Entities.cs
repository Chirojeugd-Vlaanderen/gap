using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data.Entity;
using System.Diagnostics;

namespace Chiro.Cdf.Data.Test
{
    public partial class PersoonsAdres : IEfBasisEntiteit
    {
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }

    [AssociationEndBehavior("PersoonsAdres", Owned=true)]
    public partial class GelieerdePersoon : IEfBasisEntiteit
    {
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }

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

            Debug.WriteLine(string.Format("Gelieerde Persoon: ID: {0}  Hashcode: {1}  Base Hashcode: {2}", ID, hashcode, base.GetHashCode()));

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

    public partial class Adres : IEfBasisEntiteit
    {
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }

    public partial class Categorie : IEfBasisEntiteit
    {
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }

        #region Identity en equality

        public override int GetHashCode()
        {
            int result;

            if (ID != 0)
            {
                // de ID bepaalt op unieke manier de identiteit
                result = ID.GetHashCode();
            }
            else
            {
                // Als er geen ID is, dan doen we een fallback naar de
                // GetHashCode van de parent, wat eigenlijk niet helemaal
                // correct is.
                result = base.GetHashCode();
            }

            Debug.WriteLine(string.Format("Categorie: ID: {0}  Hashcode: {1}  Base Hashcode: {2}", ID, result, base.GetHashCode()));

            return result;
        }

        public override bool Equals(object obj)
        {
            var andere = obj as Categorie;
            // Als obj geen Groep is, wordt andere null.

            if (andere == null)
            {
                return false;
            }
            else if (ID == 0 || andere.ID == 0)
            {
                return base.Equals(andere);
            }
            else
            {
                return ID.Equals(andere.ID);
            }
        }

        #endregion
    }

}
