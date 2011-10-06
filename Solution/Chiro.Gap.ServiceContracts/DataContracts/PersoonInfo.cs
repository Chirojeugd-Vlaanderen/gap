// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Properties;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Minimale persoonsinformatie
    /// </summary>
    [KnownType(typeof(PersoonDetail))]	// Anders pakt de inheritance niet
    [KnownType(typeof(PersoonOverzicht))]
    [DataContract]
    public class PersoonInfo
    {
        /// <summary>
        /// Het administratief nummer dat Chirojeugd Vlaanderen aan de persoon toekende
        /// </summary>
        [DisplayName(@"AD-nummer")]
        [DataMember]
        public int? AdNummer { get; set; }

        /// <summary>
        /// De ID van de gelieerde persoon
        /// </summary>
        [DataMember]
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Het aantal jaren dat de persoon in leeftijd afwijkt van zijn/haar Chirogeneratiegenoten
        /// </summary>
        [DataMember]
        [DisplayName(@"Chiroleeftijd")]
        [Verplicht]
        [Range(-8, 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
        [DisplayFormat(DataFormatString = "{0:#0;-#0;#0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int ChiroLeefTijd { get; set; }

        /// <summary>
        /// De voornaam van de persoon
        /// </summary>
        [Verplicht]
        [DisplayName(@"Voornaam")]
        [StringLengte(60), StringMinimumLengte(2)]
        [DataMember]
        public string VoorNaam { get; set; }

        /// <summary>
        /// De familienaam van de persoon
        /// </summary>
        [Verplicht, StringLengte(160), StringMinimumLengte(2)]
        [DisplayName(@"Familienaam")]
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// De geboortedatum van de persoon
        /// </summary>
        [Verplicht] // OPM: Altijd? Ik ben niet zeker. => Voor contactpersonen van de parochie of de jeugdraad is dat inderdaad niet relevant en dikwijls niet gekend
        [DataType(DataType.Date)]
        [DisplayName(@"Geboortedatum")]
        // [DisplayFormat(DataFormatString="{0:d}", ApplyFormatInEditMode=true, ConvertEmptyStringToNull=true)]
        [DatumInVerleden]
        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        /// <summary>
        /// De datum waarop de persoon overleden is
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayName(@"Sterfdatum")]
        [DatumInVerleden]
        [DataMember]
        public DateTime? SterfDatum { get; set; }

        /// <summary>
        /// Enumwaarde voor het geslacht van de persoon
        /// </summary>
        [Verplicht]
        [DataMember]
        public GeslachtsType Geslacht
        {
            get;
            set;
        }

        /// <summary>
        /// VersieString van de Persoon.
        /// (Die van de gelieerde persoon nemen we niet mee, aangezien bij het wijzigen van een
        /// gelieerde persoon de persoon nagenoeg altijd mee zal wijzigen.  Tenzij enkel de
        /// Chiroleeftijd wordt aangepast natuurlijk.  We zullen zien hoe erg dat is.)
        /// </summary>
        [DataMember]
        public string VersieString { get; set; }
    }
}
