// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Gegevens die je nodig hebt om een lid te maken, met uitzondering van de persoonsgegevens.
    /// </summary>
    [DataContract]
    public class LidGedoe
    {
        /// <summary>
        /// Stamnummer van de groep waarbij aan te sluiten
        /// </summary>
        [DataMember]
        public string StamNummer { get; set; }

        /// <summary>
        /// Werkjaar waarin de persoon lid moet worden
        /// </summary>
        [DataMember]
        public int WerkJaar { get; set; }

        /// <summary>
        /// Type van het lid (kind, leiding)
        /// </summary>
        [DataMember]
        public LidTypeEnum LidType { get; set; }

        /// <summary>
        /// Nationale functies die het lid moet krijgen
        /// </summary>
        [DataMember]
        public IEnumerable<FunctieEnum> NationaleFuncties { get; set; }

        /// <summary>
        /// Officiele afdelingen van het lid
        /// </summary>
        [DataMember]
        public IEnumerable<AfdelingEnum> OfficieleAfdelingen { get; set; }

        /// <summary>
        /// Einde van de instapperiode
        /// </summary>
        [DataMember]
        public DateTime? EindeInstapPeriode { get; set; }

        /// <summary>
        /// Returns a System.String that represents the current LidGedoe
        /// </summary>
        /// <returns>System.String that represents the current LidGedoe</returns>
        public override string ToString()
        {
            return this.LidType.ToString() + this.WerkJaar + this.StamNummer;
        }
    }
}