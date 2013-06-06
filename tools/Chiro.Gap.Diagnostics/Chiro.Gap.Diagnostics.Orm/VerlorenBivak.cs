using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Diagnostics.Orm
{
    public partial class VerlorenBivak : IEfBasisEntiteit
    {
        /// <summary>
        /// Als ID gebruiken we het UitstapID.  Ik vermoed dat dat
        /// uniek zal zijn in de view.
        /// </summary>
        public int ID
        {
            get { return UitstapID; }
            set { UitstapID = value; }
        }

        public byte[] Versie
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string VersieString
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool TeVerwijderen
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

    }
}
