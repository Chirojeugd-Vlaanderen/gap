using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Ad.ServiceContracts
{
    /// <summary>
    /// Dummy-implementatie van de AdService, voor gebruik bij testen.
    /// </summary>
    public class AdServiceMock: IAdService
    {
        public string GapLoginAanvragen(int adNr, string voornaam, string familienaam, string mailadres)
        {
            return String.Format("ONGELDIG-{0}", adNr);
        }
    }
}
