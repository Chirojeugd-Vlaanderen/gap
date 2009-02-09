using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Iesi.Collections.Generic;

namespace Cg2.Core.Domain
{
    [DataContract]
    public enum GeslachtsType 
    {
        [EnumMember]
        Man = 1, 
        [EnumMember]
        Vrouw = 2, 
        [EnumMember]
        Onbekend = 0
    };


    /// <summary>
    /// Klasse voor 'persoon'.  De bedoeling is dat elke individuele persoon
    /// slechts 1 keer bestaat.  Als een groep echter een nieuwe persoon
    /// maakt, en die persoon bestaat al, dan weet die groep dat soms nog
    /// niet.  De bedoeling is dat het systeem dat meldt aan het secretariaat,
    /// zodat ze daar de dubbele personen eventueel kunnen mergen.
    /// </summary>
    /// 
    [DataContract]
    [KnownType(typeof(CommunicatieVorm))]
    [KnownType(typeof(CommunicatieType))]
    public class Persoon: BasisEntiteit
    {
        #region Private members
        private int? _adNummer;
        private string _naam;
        private string _voorNaam;
        private int _geslacht;
        private DateTime? _geboorteDatum;
        private DateTime? _sterfDatum;
        private IList<CommunicatieVorm> _communicatie = new List<CommunicatieVorm>();
        #endregion

        #region Properties

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

        [DataMember]
        public int? AdNummer
        {
            get { return _adNummer; }
            set { _adNummer = value; }
        }

        [DataMember]
        public string Naam
        {
            get { return _naam; }
            set { _naam = value; }
        }

        [DataMember]
        public string VoorNaam
        {
            get { return _voorNaam; }
            set { _voorNaam = value; }
        }

        // Enums kunnen niet rechtstreeks gemapt worden door het EF.
        // Vandaar een property 'GeslachtsInt' voor EF, en een property
        // 'Geslacht' voor de applicatie.

        public int GeslachtsInt
        {
            get { return _geslacht; }
            set { _geslacht = value; }
        }

        [DataMember]
        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)this.GeslachtsInt; }
            set { this.GeslachtsInt = (int)value; }
        }

        [DataMember]
        public DateTime? GeboorteDatum
        {
            get { return _geboorteDatum; }
            set { _geboorteDatum = value; }
        }

        [DataMember]
        public DateTime? SterfDatum
        {
            get { return _sterfDatum; }
            set { _sterfDatum = value; }
        }

        [DataMember]
        public IList<CommunicatieVorm> Communicatie
        {
            get 
            {
                if (_communicatie == null)
                {
                    _communicatie = new List<CommunicatieVorm>();
                }

                return _communicatie; 
            }
            set
            {
                _communicatie = value;
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
                // oude voorkeur disablen

                CommunicatieVorm bestaandeCv = (from CommunicatieVorm v in Communicatie
                                                where v.Type == type && v.Voorkeur
                                                select v).SingleOrDefault<CommunicatieVorm>();
                if (bestaandeCv != null)
                {
                    bestaandeCv.Voorkeur = false;
                }
            }

            cv.Voorkeur = voorkeur;

            _communicatie.Add(cv);
        }

    }
}
