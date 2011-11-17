using System.Collections.Generic;

using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.ServiceContracts
{
    /// <summary>
    /// De developers hebben typisch geen KipSync draaien.  Om te ontwikkelen en te testen
    /// vervangen we de echte KipSync door deze dummy, die gewoon niets doet.
    /// </summary>
    public class KipSyncMock : ISyncPersoonService {
        public void PersoonUpdaten(Persoon persoon)
        {
        }

        public void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
        }

        public void CommunicatieVerwijderen(Persoon pers, CommunicatieMiddel communicatie)
        {
        }

        public void LidBewaren(int adNummer, LidGedoe gedoe)
        {
        }

        public void NieuwLidBewaren(PersoonDetails details, LidGedoe lidGedoe)
        {
        }

        public void LidVerwijderen(int adNummer, string stamNummer, int werkjaar)
        {
        }

        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkjaar)
        {
        }

        public void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType)
        {
        }

        public void DubbelpuntBestellen(int adNummer, string stamNummer, int werkJaar)
        {
        }

        public void DubbelpuntBestellenNieuwePersoon(PersoonDetails details, string stamNummer, int werkJaar)
        {
        }

        public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar)
        {
        }

        public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar)
        {
        }

        public void BivakBewaren(Bivak bivak)
        {
        }

        public void BivakPlaatsBewaren(int uitstapID, string plaatsNaam, Adres adres)
        {
        }

        public void BivakContactBewaren(int uitstapID, int adNummer)
        {
        }

        public void BivakContactBewarenAdOnbekend(int uitstapID, PersoonDetails details)
        {
        }

        public void BivakVerwijderen(int uitstapID)
        {
        }

        public void AfdelingenUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen)
        {
        }

        public void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<FunctieEnum> functies)
        {
        }

        public void AlleCommunicatieBewaren(Persoon persoon, IEnumerable<CommunicatieMiddel> communicatieMiddelen)
        {
        }

        public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
        {
        }
    }

}
