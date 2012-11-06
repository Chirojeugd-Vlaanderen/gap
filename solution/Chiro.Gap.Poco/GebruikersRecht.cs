using System;

namespace Chiro.Gap.Poco.Model
{
    public partial class GebruikersRecht
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> VervalDatum { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual Gav Gav { get; set; }
        public virtual Groep Groep { get; set; }
    }
    
}
