using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data.Entity;
using System.Diagnostics;
using Chiro.Cdf.Data;
namespace Chiro.Gap.Orm
{
    public partial class Categorie: IEfBasisEntiteit
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
            return 15;
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
