using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Core.Domain
{
    [DataContract]
    public enum CommunicatieType
    {
        [EnumMember]
        Telefoon = 1, 
        [EnumMember]
        Fax = 2, 
        [EnumMember]
        EMail = 3, 
        [EnumMember]
        WebSite = 4, 
        [EnumMember]
        Msn = 5, 
        [EnumMember]
        Jabber = 6
    }

    /// <summary>
    /// Een communicatievorm is gekoppeld aan precies 1 persoon.  Als er dus
    /// meerdere personen bijv. hetzelfde telefoonnummer hebben, dan komt die
    /// communicatievorm met dat telefoonnummer verschillende keren voor.
    /// </summary>
    /// 
    [DataContract]
    public class CommunicatieVorm: BasisEntiteit
    {
        #region private members
        private int _type;
        private string _nummer;
        private bool _isGezinsGebonden;
        private bool _voorkeur;
        private string _nota;
        private int _persoonID;
        #endregion

        #region properties

        // Voor LTS is het nodig dat alle property's expliciet gedefinieerd
        // zijn.  Vandaar dat ik ze override, en gewoon de base class
        // opnieuw aanroep.

        public override int ID
        {
            get { return base.ID; }
            set { base.ID = value; }
        }

        public override byte[] Versie
        {
            get { return base.Versie; }
            set { base.Versie = value; }
        }

        // Enums kunnen niet rechtstreeks gemapt worden.  Daarom gebruik ik
        // de property 'TypeInt' voor het Entity framework, die met ints werkt,
        // en de property 'Type' voor de applicatie; die werkt met 
        // 'CommunicatieTypes'.

        public int TypeInt
        {
            get { return _type; }
            set { _type = value; }
        }

        [DataMember]
        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.TypeInt; }
            set { this.TypeInt = (int)value; }
        }

        [DataMember]
        public string Nummer
        {
            get { return _nummer; }
            set { _nummer = value; }
        }

        [DataMember]
        public bool IsGezinsGebonden
        {
            get { return _isGezinsGebonden; }
            set { _isGezinsGebonden = value; }
        }

        [DataMember]
        public bool Voorkeur
        {
            get { return _voorkeur; }
            set { _voorkeur = value; }
        }

        [DataMember]
        public string Nota
        {
            get { return _nota; }
            set { _nota = value; }
        }

        [DataMember]
        public int PersoonID
        {
            get { return _persoonID; }
            set { _persoonID = value; }
        }

        #endregion

    }
}
