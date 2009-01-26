using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloNhibernate
{
    public enum GeslachtsType 
    {
        Man = 1, 
        Vrouw = 2, 
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
    public class Persoon
    {
        #region Private members
        private int _id;
        private byte[] _versie;
        private int? _adNummer;
        private string _naam;
        private string _voorNaam;
        private int _geslacht;
        private DateTime? _geboorteDatum;
        private DateTime? _sterfDatum;
        #endregion

        #region Properties

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public byte[] Versie
        {
            get { return _versie; }
            set { _versie = value; }
        }

        public int? AdNummer
        {
            get { return _adNummer; }
            set { _adNummer = value; }
        }

        public string Naam
        {
            get { return _naam; }
            set { _naam = value; }
        }

        public string VoorNaam
        {
            get { return _voorNaam; }
            set { _voorNaam = value; }
        }

        public int GeslachtsInt
        {
            get { return _geslacht; }
            set { _geslacht = value; }
        }

        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)this.GeslachtsInt; }
            set { this.GeslachtsInt = (int)value; }
        }

        public DateTime? GeboorteDatum
        {
            get { return _geboorteDatum; }
            set { _geboorteDatum = value; }
        }

        public DateTime? SterfDatum
        {
            get { return _sterfDatum; }
            set { _sterfDatum = value; }
        }

        #endregion

        public string Hallo()
        {
            return string.Format("Hallo, {0} {1}!", VoorNaam, Naam);
        }
    }
}
