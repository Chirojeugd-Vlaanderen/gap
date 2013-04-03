// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Zeer beperkt datacontract voor een plaats
    /// </summary>
    [DataContract]
    public class PlaatsInfo : AdresInfo
    {
        /// <summary>
        /// De naam van de stad of gemeente
        /// </summary>
        [DataMember]
        public string Naam { get; set; }
    }
}
