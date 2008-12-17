using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Core.Domain
{
    public enum CommunicatieType
    {
        Telefoon = 1, Fax = 2, EMail = 3, WebSite = 4, Msn = 5, Jabber = 6
    }

    /// <summary>
    /// Een communicatievorm is gekoppeld aan precies 1 persoon.  Als er dus
    /// meerdere personen bijv. hetzelfde telefoonnummer hebben, dan komt die
    /// communicatievorm met dat telefoonnummer verschillende keren voor.
    /// </summary>
    /// 
    [Serializable]
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute
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

        // Enums kunnen niet rechtstreeks gemapt worden.  Daarom gebruik ik
        // de property 'TypeInt' voor het Entity framework, die met ints werkt,
        // en de property 'Type' voor de applicatie; die werkt met 
        // 'CommunicatieTypes'.

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable = false)]
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

        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.TypeInt; }
            set { this.TypeInt = (int)value; }
        }

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable = false)]
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

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable = false)]
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

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable = false)]
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

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
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

        [global::System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute
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
