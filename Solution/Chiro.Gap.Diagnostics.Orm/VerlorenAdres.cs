using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Diagnostics.Orm
{
    /// <summary>
    /// Een verloren adres wordt bepaald door de view diag.vVerlorenAdressen.
    /// Bedoeling is dat deze klasse read-only wordt gebruikt.
    /// </summary>
    public partial class VerlorenAdres: IEfBasisEntiteit
    {
        /// <summary>
        /// Als ID gebruiken we het GelieerdePersoonID.  Ik vermoed dat dat
        /// uniek zal zijn in de view.
        /// </summary>
        public int ID
        {
            get { return GelieerdePersoonID; }
            set { GelieerdePersoonID = value; }
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
