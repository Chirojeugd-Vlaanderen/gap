// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
    [DataContract]
    public class PlaatsInfo : AdresInfo
    {
        [DataMember]
        public string Naam { get; set; }
    }
}
