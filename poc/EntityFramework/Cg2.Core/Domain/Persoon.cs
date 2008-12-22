using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Cg2.Core.Domain
{
    public enum GeslachtsType {Man = 1, Vrouw = 2, Onbekend = 0};

    /// <summary>
    /// Klasse voor 'persoon'.  De bedoeling is dat elke individuele persoon
    /// slechts 1 keer bestaat.  Als een groep echter een nieuwe persoon
    /// maakt, en die persoon bestaat al, dan weet die groep dat soms nog
    /// niet.  De bedoeling is dat het systeem dat meldt aan het secretariaat,
    /// zodat ze daar de dubbele personen eventueel kunnen mergen.
    /// </summary>
    /// 
    [Serializable]
    [EdmEntityTypeAttribute
        (NamespaceName="Cg2.Core.Domain", Name="Persoon")]
    public class Persoon: BasisEntiteit
    {
        #region Private members
        private int? _adNummer;
        private string _naam;
        private string _voorNaam;
        private int _geslacht;
        private DateTime? _geboorteDatum;
        private DateTime? _sterfDatum;
        #endregion

        #region Properties

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        public int? AdNummer
        {
            get { return _adNummer; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _adNummer = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable=false)]
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

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        public string VoorNaam
        {
            get { return _voorNaam; }
            set 
            {
                this.PropertyChanging("VoorNaam");
                _voorNaam = value;
                this.PropertyChanged("VoorNaam");
            }
        }

        // Enums kunnen niet rechtstreeks gemapt worden door het EF.
        // Vandaar een property 'GeslachtsInt' voor EF, en een property
        // 'Geslacht' voor de applicatie.

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute
            (IsNullable=false)]
        public int GeslachtsInt
        {
            get { return _geslacht; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _geslacht = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)this.GeslachtsInt; }
            set { this.GeslachtsInt = (int)value; }
        }

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        public DateTime? GeboorteDatum
        {
            get { return _geboorteDatum; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _geboorteDatum = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        public DateTime? SterfDatum
        {
            get { return _sterfDatum; }
            set 
            {
                this.PropertyChanging(System.Reflection.MethodInfo.GetCurrentMethod().Name);
                _sterfDatum = value;
                this.PropertyChanged(System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        [global::System.Data.Objects.DataClasses
            .EdmRelationshipNavigationPropertyAttribute("Cg2.Core.Domain"
            , "PersoonCommunicatieVorm", "CommunicatieVorm")]
        public global::System.Data.Objects.DataClasses
            .EntityCollection<CommunicatieVorm> Communicatie
        {
            get
            {
                return
                    ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)
                    this).RelationshipManager.GetRelatedCollection<CommunicatieVorm>
                    ("Cg2.Core.Domain.PersoonCommunicatieVorm"
                    , "CommunicatieVorm");
            }
        }
        #endregion


        /// <summary>
        /// Nieuwe communicatievorm toevoegen voor een persoon
        /// </summary>
        /// <param name="type">communicatietype</param>
        /// <param name="nr">nr/url/...</param>
        /// <param name="voorkeur">true indien dit het voorkeursnummer voor
        /// het gegeven type is.</param>
        public void CommunicatieToevoegen(CommunicatieType type, string nr
            , bool voorkeur)
        {
            CommunicatieVorm cv = new CommunicatieVorm();
            cv.Nummer = nr;
            cv.Type = type;
            cv.Persoon = this;

            // TODO: validatie, en checken op dubbels

            if (voorkeur)
            {
                CommunicatieVorm bestaandeCv = (from CommunicatieVorm v in Communicatie
                                                where v.Type == type && v.Voorkeur
                                                select v).SingleOrDefault<CommunicatieVorm>();
                if (bestaandeCv != null)
                {
                    bestaandeCv.Voorkeur = true;
                }
            }

            cv.Voorkeur = voorkeur;
        }

    }
}
