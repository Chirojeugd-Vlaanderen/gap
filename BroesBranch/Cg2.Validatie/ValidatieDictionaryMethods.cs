using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Fouten.FaultContracts;

namespace Cg2.Validatie
{
    /// <summary>
    /// Extension methods voor IValidatieDictionary
    /// </summary>
    public static class ValidatieDictionaryMethods
    {
        /// <summary>
        /// Voegt berichten uit een BusinessFault toe aan een
        /// 'IValidatieDictionary'
        /// </summary>
        /// <typeparam name="T">type foutcode voor BusinessFault</typeparam>
        /// <param name="dst">IValidatieDictionary waaraan de berichten
        /// moeten worden toegevoegd.</param>
        /// <param name="src">BusinessFault met toe te voegen berichten</param>
        /// <param name="keyPrefix">prefix toe te voegen aan de keys van src,
        /// alvorens ze toe te voegen aan dst</param>
        public static void BerichtenToevoegen<T>(this IValidatieDictionary dst, BusinessFault<T> src
            , string keyPrefix)
        {
            foreach (KeyValuePair<string, FoutBericht<T> > paar in src.Berichten)
            {
                dst.BerichtToevoegen(String.Format("{0}{1}", keyPrefix, paar.Key), paar.Value.Bericht);
            }
        }
    }
}
