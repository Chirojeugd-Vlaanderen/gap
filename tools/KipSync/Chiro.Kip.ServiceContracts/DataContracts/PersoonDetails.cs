// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract dat typisch gebruikt zal worden om een onbekende persoon naar kipadmin te sturen.
    /// Het bevat een hoop informatie over de persoon, in de hoop dat we op die manier kunnen uitvissen
    /// over wie in Kipadmn het gaat.
    /// </summary>
    [DataContract]
    public class PersoonDetails
    {
        /// <summary>
        /// Een persoonobject dat basiseigenschappen als naam en geboortedatum bevat
        /// </summary>
        [DataMember]
        public Persoon Persoon { get; set; }

        /// <summary>
        /// Een adresobject dat gegevens bevat over een woon- of werkplaats
        /// </summary>
        [DataMember]
        public Adres Adres { get; set; }

        /// <summary>
        /// Een aanduiding over welk soort adres het gaat
        /// </summary>
        [DataMember]
        public AdresTypeEnum AdresType { get; set; }

        /// <summary>
        /// Een opsomming van telefoonnummers, mailadressen, enz.
        /// </summary>
        [DataMember]
        public IEnumerable<CommunicatieMiddel> Communicatie { get; set; }
    }
}