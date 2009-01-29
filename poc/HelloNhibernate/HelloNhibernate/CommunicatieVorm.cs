using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HelloNhibernate
{
    public enum CommunicatieType
    {
        Telefoon = 1, 
        Fax = 2, 
        EMail = 3, 
        WebSite = 4, 
        Msn = 5, 
        Jabber = 6
    }

    /// <summary>
    /// Een communicatievorm is gekoppeld aan precies 1 persoon.  Als er dus
    /// meerdere personen bijv. hetzelfde telefoonnummer hebben, dan komt die
    /// communicatievorm met dat telefoonnummer verschillende keren voor.
    /// </summary>
    /// 
    public class CommunicatieVorm
    {
        #region private members
        private int _id;
        private byte[] _versie;
        private int _type;
        private string _nummer;
        private bool _isGezinsGebonden;
        private bool _voorkeur;
        private string _nota;
        private Persoon _persoon;
        #endregion

        #region properties

        // Voor LTS is het nodig dat alle property's expliciet gedefinieerd
        // zijn.  Vandaar dat ik ze override, en gewoon de base class
        // opnieuw aanroep.

        public int ID
        {
            get { return _id; }
        }

        public byte[] Versie
        {
            get { return _versie; }
            set { _versie = value; }
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

        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.TypeInt; }
            set { this.TypeInt = (int)value; }
        }

        public string Nummer
        {
            get { return _nummer; }
            set { _nummer = value; }
        }

        public bool IsGezinsGebonden
        {
            get { return _isGezinsGebonden; }
            set { _isGezinsGebonden = value; }
        }

        public bool Voorkeur
        {
            get { return _voorkeur; }
            set { _voorkeur = value; }
        }

        public string Nota
        {
            get { return _nota; }
            set { _nota = value; }
        }

        public Persoon Persoon
        {
            get { return _persoon; }
            set { _persoon = value; }
        }

        #endregion

    }
}
