// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Class die een persoon voorstelt en daar de basiseigenschappen van bevat
    /// </summary>
    [DataContract]
    public class Persoon
    {
        /// <summary>
        /// Een uniek identificatienummer
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Het administratief identificatienummer dat aan die persoon toegekend werd
        /// door het administratieprogramma van Chirojeugd Vlaanderen.
        /// </summary>
        /// <remarks>
        /// Wordt toegekend op het moment dat er een reden is om de persoon te registreren in de
        /// nationale Chiroadministratie: bij aansluiting, bij inschrijving voor een evenement of 
        /// bij een gift waar de persoon een fiscaal attest voor wil/moet krijgen.
        /// </remarks>
        [DataMember]
        public int? AdNummer { get; set; }

        /// <summary>
        /// De voornaam van de persoon
        /// </summary>
        [DataMember]
        public string VoorNaam { get; set; }

        /// <summary>
        /// De familienaam van de persoon
        /// </summary>
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// De geboortedatum van de persoon
        /// </summary>
        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        /// <summary>
        /// De datum waarop de persoon overleden is
        /// </summary>
        [DataMember]
        public DateTime? SterfDatum { get; set; }

        /// <summary>
        /// Een aanduiding voor de sekse van de persoon
        /// </summary>
        [DataMember]
        public GeslachtsEnum Geslacht { get; set; }
    }
}