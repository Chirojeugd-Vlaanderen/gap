// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Bevat de gegevens waaruit een adres bestaat
    /// </summary>
    [DataContract]
    public class Adres
    {
        /// <summary>
        /// De straatnaam
        /// </summary>
        [DataMember]
        public string Straat { get; set; }

        /// <summary>
        /// Het huisnummer
        /// </summary>
        /// <remarks>Moet numeriek zijn. Elke toevoeging (letters, slash-nogiets, enz.) moet bij Bus.</remarks>
        [DataMember]
        public int? HuisNr { get; set; }

        /// <summary>
        /// Toevoeging bij het huisnummer
        /// </summary>
        [DataMember]
        public string Bus { get; set; }

        /// <summary>
        /// Het postnummer van de (deel)gemeente
        /// </summary>
        [DataMember]
        public int PostNr { get; set; }

        /// <summary>
        /// Niet-numerieke toevoegen bij het postnummer
        /// </summary>
        /// <example>De twee letters die na het postnummer komen in een Nederlands adres</example>
        [DataMember]
        public string PostCode { get; set; }

        /// <summary>
        /// De (deel)gemeente
        /// </summary>
        [DataMember]
        public string WoonPlaats { get; set; }

        /// <summary>
        /// Het land
        /// </summary>
        [DataMember]
        public string Land { get; set; }

        /// <summary>
        /// Returns a System.String that represents the current Persoon
        /// </summary>
        /// <returns>System.String that represents the current Persoon</returns>
        public override string ToString()
        {
            return this.Straat + ' ' + this.HuisNr + ' ' + this.WoonPlaats;
        }
    }
}