using System.Diagnostics;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Sync
{
    public static class MappingExtensionMethods
    {
        public static string LandGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return null;
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).Land.Naam;
        }

        public static string LandCodeGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return "BE";
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).Land.IsoCode;
        }

        public static string PostCodeGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return null;
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).PostCode;
        }
    }
}