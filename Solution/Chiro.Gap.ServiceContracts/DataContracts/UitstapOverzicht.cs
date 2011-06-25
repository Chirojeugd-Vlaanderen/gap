// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
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
    /// <remarks>Startdatum en einddatum zijn <c>DateTime?</c>, opdat we dit
    /// datacontract ook als model zouden kunnen gebruiken in de webappl.  Als
    /// Startdatum en einddatum nullable zijn, dan zijn ze bij het aanmaken
    /// van een nieuwe uitstap gewoon leeg, ipv een nietszeggende datum in het
    /// jaar 1 als ze niet nullable zijn.</remarks>
    [DataContract]
    public class UitstapOverzicht : UitstapInfo
    {
        [DataMember]
        [DisplayName(@"Naam van de bivakplaats")]
        public string PlaatsNaam { get; set; }

        // Een datacontract moet normaalgezien 'plat' zijn.  Maar het lijkt me
        // zo raar om hier gewoon over te tikken wat er al in AdresInfo staat.
        [DataMember]
        public AdresInfo Adres { get; set; }

        [DataMember]
        public int GroepsWerkJaarID { get; set; }
    }
}
