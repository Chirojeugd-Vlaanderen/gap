using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm
{
    /// <summary>
    /// Een straat heeft geen versie (timestamp) in de database.
    /// Dat lijkt me ook niet direct nodig voor een klasse die
    /// bijna nooit wijzigt.
    /// 
    /// Het feit dat er geen timestamp is, wil wel zeggen dat
    /// 'concurrencygewijze' de laatste altijd zal winnen.    
    /// </summary>
    public partial class Subgemeente: IBasisEntiteit
    {
        #region IBasisEntiteit Members

        public byte[] Versie
        {
            get
            {
                return null;
            }
            set
            {
                // Doe niets
            }
        }

        public string VersieString
        {
            get { return null; }
            set { /*Doe niets*/ }
        }

        #endregion
    }
}
