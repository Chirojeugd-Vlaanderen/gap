namespace Chiro.Gap.Poco.Model
{
    public partial class BelgischAdres : Adres
    {
    
        public virtual StraatNaam StraatNaam { get; set; }
        public virtual WoonPlaats WoonPlaats { get; set; }
    }
    
}
