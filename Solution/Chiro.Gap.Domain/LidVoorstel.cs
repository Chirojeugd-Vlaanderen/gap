// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Domain
{
    /// <summary>
    /// Class voor een combinatie van gegevens die bepalen hoe iemand ingeschreven wordt
    /// </summary>
    public class LidVoorstel
    {
        /// <summary>
        /// In welk afdelingsjaren het lid moet worden ingeschreven.
        /// </summary>
        public int[] AfdelingsJaarIDs;

        /// <summary>
        /// True als er geen rekening moet worden gehouden met de inhoud van afdelingsjaarIDs
        /// </summary>
        public bool AfdelingsJarenIrrelevant;

        /// <summary>
        /// Of het lid moet worden ingeschreven als leiding
        /// </summary>
        public bool LeidingMaken;
    }
}