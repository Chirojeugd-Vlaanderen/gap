namespace Chiro.Gap.Poco.Model
{
    public partial class CommunicatieVorm
    {
        public string Nota { get; set; }
        public string Nummer { get; set; }
        public int ID { get; set; }
        public bool IsGezinsgebonden { get; set; }
        public bool Voorkeur { get; set; }
        public byte[] Versie { get; set; }
        public bool IsVoorOptIn { get; set; }
    
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual CommunicatieType CommunicatieType { get; set; }
    }
    
}
