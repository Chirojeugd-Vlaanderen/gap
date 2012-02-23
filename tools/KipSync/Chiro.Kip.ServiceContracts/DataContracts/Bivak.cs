// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor een bivakobject
    /// </summary>
    [DataContract]
    public class Bivak
    {
        /// <summary>
        /// ID van de uitstap in GAP.
        /// </summary>
        [DataMember]
        public int UitstapID { get; set; }

        /// <summary>
        /// Stamnummer voor groep die op bivak gaat
        /// </summary>
        [DataMember]
        public string StamNummer { get; set; }

        /// <summary>
        /// Werkjaar van het bivak.
        /// </summary>
        [DataMember]
        public int WerkJaar { get; set; }

        /// <summary>
        /// Begindatum van het bivak
        /// </summary>
        [DataMember]
        public DateTime DatumVan { get; set; }

        /// <summary>
        /// Einddatum van het bivak
        /// </summary>
        [DataMember]
        public DateTime DatumTot { get; set; }

        /// <summary>
        /// Naam van het bivak
        /// </summary>
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// Opmerkingen bij het bivak
        /// </summary>
        [DataMember]
        public string Opmerkingen { get; set; }
    }
}