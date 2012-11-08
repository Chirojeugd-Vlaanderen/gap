using System;

namespace Chiro.Cdf.Poco
{
    public abstract class BasisEntiteit
    {
        /// <summary>
        /// De bedoeling is dat Versie een timestamp (row version) is, voor concurrency control
        /// </summary>
        public abstract byte[] Versie { get; set; }

        /// <summary>
        /// ID is de primary key
        /// </summary>
        public abstract int ID { get; set; }

        /// <summary>
        /// Versie als string, om gemakkelijk te kunnen gebruiken
        /// met MVC model binding in forms
        /// </summary>
        public string VersieString
        {
            get { return Versie == null ? String.Empty : Convert.ToBase64String(Versie); }
            set { Versie = Convert.FromBase64String(value ?? String.Empty); }
        }

        /// <summary>
        /// Experimentele equals die objecten als gelijk beschouwt als hun ID's niet nul en gelijk zijn.
        /// </summary>
        /// <param name="obj">Te vergelijken entiteit</param>
        /// <returns><c>True</c> als this en <paramref name="obj"/> hetzelfde niet-nulle ID 
        /// hebben</returns>
        /// <remarks>Als zowel this als <paramref name="obj"/> ID 0 hebben, wordt
        /// Object.Equas aangeroepen</remarks>
        public override bool Equals(object obj)
        {
            if (obj is BasisEntiteit)
            {
                var f = obj as BasisEntiteit;

                if (f.ID != 0 && f.ID == ID)
                {
                    return true;
                }
                if (f.ID == 0 && ID == 0)
                {
                    return ReferenceEquals(this, obj);
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Er moet gelden: a equals b => a.gethashcode == b.gethashcode
        /// </summary>
        /// <returns>De hashcode</returns>
        public override int GetHashCode()
        {
            return ID;
        }
    }
}
