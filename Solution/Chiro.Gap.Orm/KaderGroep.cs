// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
    /// <summary>
    /// Klasse voor kadergroep, die het meeste gewoon erft van Groep.
    /// </summary>
    public partial class KaderGroep
    {
        /// <summary>
        /// Converteert de 'NiveauInt' uit de database naar een enum Niveau.
        /// </summary>
        public override Niveau Niveau
        {
            get { return (Niveau)NiveauInt; }
        }
    }
}
