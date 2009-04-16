using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm
{
    /// <summary>
    /// stelt voor welke extra info er in met een GelieerdePersoon opgevraagd kan worden
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
        // We gaan de lijst met PersoonsInfo niet opnemen in de
        // klasse.  De programmeur moet te allen tijde maar weten
        // welke informatie hij wel/niet opgevraagd heeft.

        // private IList<PersoonsInfo> _meeGeleverd;

        public GelieerdePersoon(): base()
        {
            // _meeGeleverd = null;
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }
}
