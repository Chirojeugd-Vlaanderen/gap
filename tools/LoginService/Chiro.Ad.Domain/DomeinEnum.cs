// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Ad.Domain
{
    /// <summary>
    /// Enum voor de verschillende domeinen in Active Directory waar mensen een login
    /// kunnen krijgen.
    /// </summary>
    public enum DomeinEnum
    {
        /// <summary>
        /// Domein voor interne gebruikers, typisch beroepskrachten, die op het
        /// netwerk van de Chiro moeten kunnen.
        /// </summary>
        Lokaal,

        /// <summary>
        /// Domein voor externe gebruikers, typisch vrijwilligers, die alleen op het extranet
        /// moeten kunnen.
        /// </summary>
        Wereld
    }
}