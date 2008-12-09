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
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute
        (NamespaceName="Cg2.Core.Domain",Name="CommunicatieVorm")]
    public class CommunicatieVorm: BasisEntiteit<int>
    {
        #region private members
        private CommunicatieType _type;
        private string _nummer;
        private bool _isGezinsGebonden;
        private bool _voorkeur;
        private string _nota;
        private int _persoonID;
        private Persoon _persoon;
        #endregion

        #region properties

        /// <summary>
        /// Voor het Entity Framework moeten we de ID-property expliciet
        /// overriden, anders werkt het niet met de attributen.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (EntityKeyProperty = true, IsNullable = false)]
        public override int ID
        {
            get { return base.ID; }
            set
            {
                this.PropertyChanging("ID");
                base.ID = value;
                this.PropertyChanged("ID");
            }
        }

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable = false)]
        public CommunicatieType Type
        {
            get { return _type; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _type = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
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

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable = false)]
        public int PersoonID
        {
            get { return _persoonID; }
            set
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _persoonID = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }


        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable = false)]
        public Persoon Persoon
        {
            get { return _persoon; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _persoon = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }
        #endregion

    }
}
