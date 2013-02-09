using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public partial class Lid: BasisEntiteit
    {
        public Lid()
        {
            this.Functie = new HashSet<Functie>();
        }
    
        public Nullable<bool> LidgeldBetaald { get; set; }
        public bool NonActief { get; set; }
        public bool Verwijderd { get; set; }
        public short VolgendWerkjaarInt { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        public Nullable<System.DateTime> EindeInstapPeriode { get; set; }
        public Nullable<System.DateTime> UitschrijfDatum { get; set; }
    
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual ICollection<Functie> Functie { get; set; }

        /// <summary>
        /// Geeft een LidType weer, wat Kind of Leiding kan zijn.
        /// </summary>
        public LidType Type
        {
            get { return (this is Kind ? LidType.Kind : LidType.Leiding); }
        }

        /// <summary>
        /// Geeft een lijst terug van alle afdelingen waaraan het lid gegeven gekoppeld is.
        /// </summary>
        /// <returns>
        /// Lijst met afdelingIDs
        /// </returns>
        /// <remarks>
        /// Een kind is hoogstens aan 1 afdeling gekoppeld
        /// </remarks>
        public IList<int> AfdelingIds
        {
            get
            {
                if (this is Kind)
                {
                    var k = this as Kind;
                    Debug.Assert(k != null);
                    return new List<int>{k.AfdelingsJaar.Afdeling.ID};
                }
                if (this is Leiding)
                {
                    var l = this as Leiding;
                    Debug.Assert(l != null);

                    return (from aj in l.AfdelingsJaar select aj.Afdeling.ID).ToList();
                }
                throw new NotSupportedException("Lid moet kind of leiding zijn.");
            }
        }
    }
    
}
