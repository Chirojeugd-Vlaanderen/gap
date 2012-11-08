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

        /// <summary>
        /// Geeft weer of de vervaldatum verlengbaar is. Dit is eigenlijk business, dus dat
        /// staat hier verkeerd.
        /// </summary>
        /// <returns></returns>
        public bool IsVerlengbaar
        {
            get
            {
                return VervalDatum != null && ((DateTime) VervalDatum) < DateTime.Now.AddMonths(
                    Properties.Settings.Default.MaandenGebruikersRechtVerlengbaar);
            }
        }
    }
    
}
