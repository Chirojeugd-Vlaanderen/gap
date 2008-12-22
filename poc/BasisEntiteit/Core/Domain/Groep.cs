using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Domain
{
    /// <summary>
    /// There are no comments for ChiroGroepModel.Groep in the schema.
    /// </summary>
    /// <KeyProperties>
    /// GroepID
    /// </KeyProperties>
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName = "Core.Domain", Name = "Groep")]
    [global::System.Runtime.Serialization.DataContractAttribute(IsReference = true)]
    [global::System.Serializable()]
    public partial class Groep : BasisEntiteit
    {
        /// <summary>
        /// There are no comments for Property GroepID in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
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

        /// <summary>
        /// Create a new Groep object.
        /// </summary>
        /// <param name="naam">Initial value of Naam.</param>
        /// <param name="groepID">Initial value of GroepID.</param>
        /// <param name="versie">Initial value of Versie.</param>
        public static Groep CreateGroep(string naam, int groepID, byte[] versie)
        {
            Groep groep = new Groep();
            groep.Naam = naam;
            groep.ID = groepID;
            groep.Versie = versie;
            return groep;
        }
        /// <summary>
        /// There are no comments for Property Naam in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable = false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Naam
        {
            get
            {
                return this._Naam;
            }
            set
            {
                this.OnNaamChanging(value);
                this.PropertyChanging("Naam");
                this._Naam = value;
                this.PropertyChanged("Naam");
                this.OnNaamChanged();
            }
        }
        private string _Naam;
        partial void OnNaamChanging(string value);
        partial void OnNaamChanged();
        /// <summary>
        /// There are no comments for Property Code in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                this.OnCodeChanging(value);
                this.PropertyChanging("Code");
                this._Code = value;
                this.PropertyChanged("Code");
                this.OnCodeChanged();
            }
        }
        private string _Code;
        partial void OnCodeChanging(string value);
        partial void OnCodeChanged();
        /// <summary>
        /// There are no comments for Property OprichtingsJaar in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public global::System.Nullable<int> OprichtingsJaar
        {
            get
            {
                return this._OprichtingsJaar;
            }
            set
            {
                this.OnOprichtingsJaarChanging(value);
                this.PropertyChanging("OprichtingsJaar");
                this._OprichtingsJaar = value;
                this.PropertyChanged("OprichtingsJaar");
                this.OnOprichtingsJaarChanged();
            }
        }
        private global::System.Nullable<int> _OprichtingsJaar;
        partial void OnOprichtingsJaarChanging(global::System.Nullable<int> value);
        partial void OnOprichtingsJaarChanged();
        /// <summary>
        /// There are no comments for Property WebSite in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string WebSite
        {
            get
            {
                return this._WebSite;
            }
            set
            {
                this.OnWebSiteChanging(value);
                this.PropertyChanging("WebSite");
                this._WebSite = value;
                this.PropertyChanged("WebSite");
                this.OnWebSiteChanged();
            }
        }
        private string _WebSite;
        partial void OnWebSiteChanging(string value);
        partial void OnWebSiteChanged();
        /// <summary>
        /// There are no comments for Property Logo in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Logo
        {
            get
            {
                return this._Logo;
            }
            set
            {
                this.OnLogoChanging(value);
                this.PropertyChanging("Logo");
                this._Logo = value;
                this.PropertyChanged("Logo");
                this.OnLogoChanged();
            }
        }
        private byte[] _Logo;
        partial void OnLogoChanging(byte[] value);
        partial void OnLogoChanged();

        /// <summary>
        /// There are no comments for Property Versie in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable = false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Versie
        {
            get
            {
                return _Versie;
            }
            set
            {
                this.OnVersieChanging(value);
                this.PropertyChanging("Versie");
                this._Versie = value;
                this.PropertyChanged("Versie");
                this.OnVersieChanged();
            }
        }
        private byte[] _Versie;
        partial void OnVersieChanging(byte[] value);
        partial void OnVersieChanged();
    }
}
