﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
    /// <summary>
    /// Faultcontract voor als er iets mis is met een object.  Dit contract bevat verschillende foutberichten, die
    /// betrekking kunnen hebben op property's van het object.
    /// </summary>
    [DataContract]
    public class OngeldigObjectFault : GapFault
    {
        // TODO (#1041): Dit wordt blijkbaar enkel gebruikt voor adressen.  Is heel die constructie dan wel
        // nodig? Misschien is een AdresFault wel even goed.

        /// <summary>
        /// Dictionary die voor elke fout een dictionary-entry (component, foutbericht) bevat.
        /// </summary>
        [DataMember]
        public Dictionary<string, FoutBericht> Berichten { get; set; }
    }
}