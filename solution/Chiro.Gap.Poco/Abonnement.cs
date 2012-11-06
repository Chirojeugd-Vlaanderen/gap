namespace Chiro.Gap.Poco.Model
{
    public partial class Abonnement
    {
        public int ID { get; set; }
        public System.DateTime AanvraagDatum { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual Publicatie Publicatie { get; set; }
    }
    
}
