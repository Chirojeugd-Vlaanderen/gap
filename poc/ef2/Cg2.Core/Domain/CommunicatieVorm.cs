using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
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
    [EdmEntityTypeAttribute
        (NamespaceName="Cg2.Core.Domain",Name="CommunicatieVorm")]
    public class CommunicatieVorm: BasisEntiteit
    {
        #region private members
        private int _type;
        private string _nummer;
        private bool _isGezinsGebonden;
        private bool _voorkeur;
        private string _nota;
        #endregion

        #region properties

        [DataMember]
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        public override int ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = value;
            }
        }

        // Enums kunnen niet rechtstreeks gemapt worden.  Daarom gebruik ik
        // de property 'TypeInt' voor het Entity framework, die met ints werkt,
        // en de property 'Type' voor de applicatie; die werkt met 
        // 'CommunicatieTypes'.

        [EdmScalarPropertyAttribute (IsNullable = false)]
        public int TypeInt
        {
            get { return _type; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _type = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [DataMember]
        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.TypeInt; }
            set { this.TypeInt = (int)value; }
        }

        [DataMember]
        [EdmScalarPropertyAttribute (IsNullable = false)]
        public string Nummer
        {
            get { return _nummer; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _nummer = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [DataMember]
        [EdmScalarPropertyAttribute (IsNullable = false)]
        public bool IsGezinsGebonden
        {
            get { return _isGezinsGebonden; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _isGezinsGebonden = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [DataMember]
        [EdmScalarPropertyAttribute (IsNullable = false)]
        public bool Voorkeur
        {
            get { return _voorkeur; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _voorkeur = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [DataMember]
        [EdmScalarPropertyAttribute()]
        public string Nota
        {
            get { return _nota; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _nota = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [EdmRelationshipNavigationPropertyAttribute
            ("Cg2.Core.Domain", "PersoonCommunicatieVorm", "Persoon")]
        public Persoon Persoon
        {
            get 
            { 
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this))
                    .RelationshipManager.GetRelatedReference<Persoon>
                    ("Cg2.Core.Domain.PersoonCommunicatieVorm", "Persoon").Value; 
            }
            set
            {
                // Hier blijkbaar geen 'PropertyChanging' en 'PropertyChanged'

                ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this))
                    .RelationshipManager.GetRelatedReference<Persoon>
                    ("Cg2.Core.Domain.PersoonCommunicatieVorm", "Persoon").Value = value;
            }
        }
        #endregion

    }
}
