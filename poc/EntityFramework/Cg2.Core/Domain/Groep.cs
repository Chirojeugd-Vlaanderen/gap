using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Algemene groepsklasse, parent voor zowel ChiroGroep als Satelliet.
    /// </summary>
    /// 
    [Serializable]
    [EdmEntityTypeAttribute
        (NamespaceName="Cg2.Core.Domain", Name="Groep")]
    // Als er ervende klasses zijn, moeten die hier als 'KnownType'
    // gemarkeerd worden? (als dat er niet staat, krijg je een exception
    // als een groep geserialiseerd moet worden.)
    [KnownType(typeof(ChiroGroep))]
    public class Groep: BasisEntiteit
    {
        #region Private members
        private string _code;
        private string _naam;
        private int? _oprichtingsJaar;
        private string _webSite;
        #endregion

        #region Properties

        [EdmScalarPropertyAttribute (EntityKeyProperty = true, IsNullable = false)]
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

        [EdmScalarPropertyAttribute()]
        public string Code
        {
            get { return _code; }
            set
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _code = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [EdmScalarPropertyAttribute (IsNullable=false)]
        public string Naam
        {
            get { return _naam; }
            set
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _naam = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [EdmScalarPropertyAttribute()]
        public int? OprichtingsJaar
        {
            get { return _oprichtingsJaar; }
            set
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _oprichtingsJaar = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [EdmScalarPropertyAttribute()]
        public string WebSite
        {
            get { return _webSite; }
            set
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _webSite = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }
        #endregion
    }
}
