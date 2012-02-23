// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Details van een uitstap
    /// </summary>
    [DataContract]
    public class UitstapOverzicht : UitstapInfo
    {
        /// <summary>
        /// De naam van de bivakplaats
        /// </summary>
        [DataMember]
        [DisplayName(@"Naam van de bivakplaats")]
        public string PlaatsNaam { get; set; }

        // Een datacontract moet normaal gezien 'plat' zijn.  Maar het lijkt me
        // zo raar om hier gewoon over te tikken wat er al in AdresInfo staat.

        /// <summary>
        /// Het adres van de bivakplaats
        /// </summary>
        [DataMember]
        public AdresInfo Adres { get; set; }

        /// <summary>
        /// De ID van het groepswerkjaar waarin de uitstap georganiseerd werd
        /// </summary>
        [DataMember]
        public int GroepsWerkJaarID { get; set; }
    }
}
