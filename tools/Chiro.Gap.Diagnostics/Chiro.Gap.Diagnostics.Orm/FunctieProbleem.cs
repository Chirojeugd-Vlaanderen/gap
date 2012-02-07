using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Diagnostics.Orm
{
    /// <summary>
    /// Een functieprobleem wordt bepaald door de view diag.vFunctieProblemen.
    /// Bedoeling is dat deze klasse read-only wordt gebruikt.
    /// </summary>
    public partial class FunctieProbleem: IEfBasisEntiteit
    {
        public int ID
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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
