using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm
{
    /// <summary>
    /// stelt voor welke extra info er in een gelieerd persoon object opgeslagen is dat terug wordt gegeven.
    /// ADRESSEN: alle adressen van de persoon die zichtbaar zijn, zijn toegevoegd
    /// COMMUNICATIEVORMEN: alle communicatievormen
    /// LIDINFO: alle lidinfo van het huidige werkjaar en de huidige groep
    /// ...
    /// </summary>
    [DataContract]
    public enum PersoonsInfo {
        [EnumMember] Adressen,
        [EnumMember] CommunicatieVormen,
        [EnumMember] VrijeVelden,
        [EnumMember] LidInfo, //TODO maar lidmaken mag niet via persoon, moet via ledenservice
        [EnumMember] Categorieen,
        [EnumMember] CursusInfo,
        [EnumMember] VerzekeringsInfo
    }

    public partial class GelieerdePersoon : IBasisEntiteit 
    {
        private IList<PersoonsInfo> _meeGeleverd;

        public GelieerdePersoon(): base()
        {
            _meeGeleverd = null;
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }
}
