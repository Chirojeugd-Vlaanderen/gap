using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Cg2.EfWrapper;
using Cg2.EfWrapper.Entity;

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

    // Als er een persoon met adressen over de service gestuurd wordt,
    // en een PersoonsAdres is verdwenen, dan is het de bedoeling dat
    // het persoonsobject mee verdwijnt uit de database.  Om daarvoor
    // te zorgen, is onderstaand attribuut AssociationEndBehavior
    // nodig.  (Als dat attribuut er niet zou staan, dan zou enkel
    // de koppeling tussen Persoon en Persoonsadres verdwijnen, en
    // dat heeft dan weer een key violation tot gevolg.)

    [AssociationEndBehavior("PersoonsAdres", Owned = true)]
    public partial class GelieerdePersoon : IBasisEntiteit 
    {
        // We gaan de lijst met PersoonsInfo niet opnemen in de
        // klasse.  De programmeur moet te allen tijde maar weten
        // welke informatie hij wel/niet opgevraagd heeft.

        // private IList<PersoonsInfo> _meeGeleverd;

        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

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
