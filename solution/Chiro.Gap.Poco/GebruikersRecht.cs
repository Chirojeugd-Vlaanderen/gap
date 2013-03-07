using System;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public partial class GebruikersRecht: BasisEntiteit
    {
        public override int ID { get; set; }
        public Nullable<System.DateTime> VervalDatum { get; set; }
        public override byte[] Versie { get; set; }
    
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

        /// <summary>
        /// Rol van de gebruiker. Op dit moment ondersteunen we enkel GAV (zie #844)
        /// </summary>
        public Rol Rol
        {
            get { return VervalDatum == null || VervalDatum < DateTime.Now ? Rol.Geen : Rol.Gav; }
        }
    }
    
}
