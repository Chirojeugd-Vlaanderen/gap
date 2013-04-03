using System;
using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public partial class Functie: BasisEntiteit
    {
        public Functie()
        {
            this.Lid = new HashSet<Lid>();
        }
    
        public string Naam { get; set; }
        public string Code { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        public Nullable<int> MaxAantal { get; set; }
        public int MinAantal { get; set; }
        public Nullable<int> WerkJaarVan { get; set; }
        public Nullable<int> WerkJaarTot { get; set; }
        public bool IsNationaal { get; set; }
        public int LidTypeInt { internal get; set; }
        public int NiveauInt { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<Lid> Lid { get; set; }

        /// <summary>
        /// Controleert of de functie de nationale functie de nationale functie <paramref name="natFun"/> is.
        /// </summary>
        /// <param name="natFun">Een nationale functie</param>
        /// <returns><c>True</c> als de functie de nationale functie is, anders <c>false</c></returns>
        public bool Is(NationaleFunctie natFun)
        {
            return ID == ((int) natFun);
        }

        /// <summary>
        /// Koppeling tussen enum Niveau en databaseveld NiveauInt
        /// </summary>
        public Niveau Niveau
        {
            get { return (Niveau)NiveauInt; }
            set { NiveauInt = (int)value; }
        }

        /// <summary>
        /// Koppeling tussen enum LidType en databaseveld LidTypeInt
        /// </summary>
        public LidType Type
        {
            get { return (LidType)LidTypeInt; }
            set { LidTypeInt = (int)value; }
        }
    }
    
}
