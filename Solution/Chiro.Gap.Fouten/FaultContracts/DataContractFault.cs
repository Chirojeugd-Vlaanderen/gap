using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.FaultContracts
{
    /// <summary>
    /// Als er een fout optreedt aan de businesskant, zal de service
    /// een BusinessFault terugsturen.
    /// </summary>
    /// <typeparam name="T">Type van de foutcode (meestal een enum)</typeparam>
    [DataContract]
    public class DataContractFault<T>
    {
        [DataMember]
        public Dictionary<string, FoutBericht<T>> Berichten { get; set; }

        public DataContractFault()
        {
            Berichten = new Dictionary<string, FoutBericht<T>>();
        }

        /// <summary>
        /// Voegt een foutbericht toe
        /// </summary>
        /// <param name="foutCode">foutcode</param>
        /// <param name="component">omschrijving van de component waar het fout 
        /// liep.</param>
        /// <param name="bericht">foutboodschap.  Mag ook leeg zijn.</param>
        public void BerichtToevoegen(T foutCode, string component, string bericht)
        {
            Berichten.Add(component, new FoutBericht<T> { FoutCode = foutCode, Bericht = bericht });
        }
    }
}
