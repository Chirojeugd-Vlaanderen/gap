using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chiro.Kip.Services.DataContracts;

namespace Chiro.Kip.Services
{
    public class SyncPersoonService : ISyncPersoonService
    {
        
        public void PersoonUpdated(Persoon persoon)
        {
            Debug.WriteLine(string.Format("You entered: {0}", persoon.Id));
            Console.WriteLine(string.Format("You entered: {0} {1} {2}", persoon.Id, persoon.Voornaam, persoon.Naam));
        }

        public void AdresUpdated(Persoon persoon, IEnumerable<Adres> adreses)
        {
            Debug.WriteLine(string.Format("You entered: {0}", persoon.Id));
            Console.WriteLine(string.Format("You entered: {0}", persoon.Id));
        }

        public void CommunicatieUpdated(Persoon persoon, IEnumerable<Communicatiemiddel> communicatiemiddelen)
        {
            Debug.WriteLine(string.Format("You entered: {0}", persoon.Id));
            Console.WriteLine(string.Format("You entered: {0}", persoon.Id));
        }
    }
}
