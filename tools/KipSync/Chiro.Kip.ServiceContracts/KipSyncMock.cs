// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;

using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.ServiceContracts
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// De developers hebben typisch geen KipSync draaien.  Om te ontwikkelen en te testen
    /// vervangen we de echte KipSync door deze dummy, die gewoon niets doet.
    /// </summary>
    public class KipSyncMock : ISyncPersoonService
    {
        private string _ikke;

        /// <summary>
        /// Create instance
        /// </summary>
        public KipSyncMock()
        {
            this._ikke = this.ToString() + ' ';
        }

        /// <summary>
        /// Doet alsof het een persoon bijwerkt
        /// </summary>
        /// <param name="persoon">
        /// De persoon die 'bijgewerkt' moet worden
        /// </param>
        public void PersoonUpdaten(Persoon persoon)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "PersoonUpdaten {0}", persoon));
        }

        /// <summary>
        /// Doet alsof het een communicatievorm toevoegt aan een persoon
        /// </summary>
        /// <param name="persoon">
        /// De persoon die 'bijgewerkt' moet worden
        /// </param>
        /// <param name="communicatieMiddel">
        /// Het communicatiemiddel dat toegevoegd moet worden
        /// </param>
        public void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "CommunicatieToevoegen {0} {1}", persoon, communicatieMiddel));
        }

        /// <summary>
        /// Doet alsof het een communicatievorm verwijdert van een persoon
        /// </summary>
        /// <param name="persoon">
        /// De persoon die 'bijgewerkt' moet worden
        /// </param>
        /// <param name="communicatieMiddel">
        /// Het communicatiemiddel dat 'verwijderd' moet worden
        /// </param>
        public void CommunicatieVerwijderen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "CommunicatieVerwijderen {0} {1}", persoon, communicatieMiddel));
        }

        /// <summary>
        /// Doet alsof het een bestaand lid opnieuw bewaart
        /// </summary>
        /// <param name="adNummer">
        /// Het AD-nummer van het lid
        /// </param>
        /// <param name="gedoe">
        /// De bewerkte gegevens die 'bewaard' moeten worden
        /// </param>
        public void LidBewaren(int adNummer, LidGedoe gedoe)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "LidBewaren {0} {1}", adNummer, gedoe));
        }

        /// <summary>
        /// Doet alsof het een nieuwe persoon toevoegt en die ineens als lid inschrijft
        /// </summary>
        /// <param name="details">
        /// De persoonsgegevens die 'opgeslagen' moeten worden
        /// </param>
        /// <param name="lidGedoe">
        /// De lidgegevens die 'opgeslagen' moeten worden
        /// </param>
        public void NieuwLidBewaren(PersoonDetails details, LidGedoe lidGedoe)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "NieuwLidBewaren {0} {1}", details, lidGedoe));
        }

        /// <summary>
        /// Doet alsof het een lid uitschrijft in een opgegeven groep en voor een opgegeven werkJaar
        /// </summary>
        /// <param name="adNummer">
        /// Het AD-nummer van de persoon die niet meer als lid ingeschreven moet zijn 
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep waar het over gaat
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarin het lid uitgeschreven moet worden
        /// </param>
        /// <param name="uitschrijfDatum"> </param>
        public void LidVerwijderen(int adNummer, string stamNummer, int werkJaar, DateTime uitschrijfDatum)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "LidVerwijderen AD{0} SN{1} WJ{2}", adNummer, stamNummer, werkJaar));
        }

        /// <summary>
        /// Doet alsof het een persoon uitschrijft voor wie KipSync nog niet gelopen heeft, en van wie
        /// we dus niet noodzakelijk een AD-nummer kennen
        /// </summary>
        /// <param name="details">
        /// Gegevens van de persoon die 'uitgeschreven' moet worden
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep die hem of haar uitschrijft
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarin die persoon niet ingeschreven moet zijn
        /// </param>
        /// <param name="uitschrijfDatum"> </param>
        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkJaar, DateTime uitschrijfDatum)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "NieuwLidVerwijderen {0} SN{1} WJ{2}", details, stamNummer, werkJaar));
        }

        /// <summary>
        /// Aanpassen of iemand als lid, als leiding of als kader ingeschreven moet zijn in het opgegeven werkJaar
        /// </summary>
        /// <param name="persoon">
        /// De persoon over wie het gaat
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep, het gewest of het verbond waar de persoon ingeschreven is
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waar het over gaat
        /// </param>
        /// <param name="lidType">
        /// Het juiste lidtype
        /// </param>
        public void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "LidTypeUpdaten {0} SN{1} WJ{2} LT{3}", persoon, stamNummer, werkJaar, lidType));
        }

        /// <summary>
        /// Doet alsof het een nieuw Dubbelpuntabonnement aanvraagt
        /// </summary>
        /// <param name="adNummer">
        /// Het AD-nummer van de persoon die een abonnement wil
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep die het abonnement aanvraagt
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarin de persoon zich wil abonneren
        /// </param>
        public void DubbelpuntBestellen(int adNummer, string stamNummer, int werkJaar)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "DubbelpuntBestellen AD{0} SN{1} WJ{2}", adNummer, stamNummer, werkJaar));
        }

        /// <summary>
        /// Doet alsof het een nieuw Dubbelpuntabonnement aanvraagt voor een nieuw
        /// toegevoegde persoon
        /// </summary>
        /// <param name="details">
        /// De persoonsgegevens die 'opgeslagen' moeten worden
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep die het abonnement aanvraagt
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarin de persoon zich wil abonneren
        /// </param>
        public void DubbelpuntBestellenNieuwePersoon(PersoonDetails details, string stamNummer, int werkJaar)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "DubbelpuntBestellenNieuwePersoon {0} SN{1} WJ{2}", details, stamNummer, werkJaar));
        }

        /// <summary>
        /// Doet alsof het een verzekering tegen loonverlies aanvraagt
        /// </summary>
        /// <param name="adNummer">
        /// Het AD-nummer van de persoon die verzekerd moet worden
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep die de verzekering aanvraagt
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarin de persoon verzekerd wil worden
        /// </param>
        public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "LoonVerliesVerzekeren AD{0} SN{1} WJ{2}", adNummer, stamNummer, werkJaar));
        }

        /// <summary>
        /// Doet alsof het een verzekering tegen loonverlies aanvraagt voor een persoon
        /// van wie het AD-nummer niet gekend is aan de GAP-kant
        /// </summary>
        /// <param name="details">
        /// De persoonsgegevens die 'opgeslagen' moeten worden 
        /// (of aan de hand waarvan we de persoon kunnen opzoeken aan de KipAdmin-kant
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep die de verzekering aanvraagt
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarin de persoon verzekerd wil worden
        /// </param>
        public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "LoonVerliesVerzekeren D{0} SN{1} WJ{2}", details, stamNummer, werkJaar));
        }

        /// <summary>
        /// Doet alsof het een bivak registreren
        /// </summary>
        /// <param name="bivak">
        /// De gegevens die opgeslagen moeten worden
        /// </param>
        public void BivakBewaren(Bivak bivak)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "BivakBewaren {0}", bivak));
        }

        /// <summary>
        /// Doet alsof het adresgegevens van een bivakplaats opslaat
        /// </summary>
        /// <param name="uitstapID">
        /// De ID van de uitstap waarvoor er een adres toegevoegd wordt
        /// </param>
        /// <param name="plaatsNaam">
        /// De naam van de gemeente
        /// </param>
        /// <param name="adres">
        /// De adresgegevens
        /// </param>
        public void BivakPlaatsBewaren(int uitstapID, string plaatsNaam, Adres adres)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "BivakPlaatsBewaren {0} {1} {2}", uitstapID, plaatsNaam, adres));
        }

        /// <summary>
        /// Doet alsof het registreert wie de contactpersoon van een bepaalde uitstap is
        /// </summary>
        /// <param name="uitstapID">
        /// De ID van de uitstap
        /// </param>
        /// <param name="adNummer">
        /// Het AD-nummer van de persoon
        /// </param>
        public void BivakContactBewaren(int uitstapID, int adNummer)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "BivakContactBewaren {0} {1}", uitstapID, adNummer));
        }

        /// <summary>
        /// Doet alsof het registreert wie de contactpersoon van een bepaalde uitstap is,
        /// voor een persoon van wie het AD-nummer niet gekend is aan de GAP-kant
        /// </summary>
        /// <param name="uitstapID">
        /// De ID van de uistap
        /// </param>
        /// <param name="details">
        /// De persoonsgegevens die 'opgeslagen' moeten worden 
        /// (of aan de hand waarvan we de persoon kunnen opzoeken aan de KipAdmin-kant
        /// </param>
        public void BivakContactBewarenAdOnbekend(int uitstapID, PersoonDetails details)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "BivakContactBewarenAdOnbekend {0}", uitstapID, details));
        }

        /// <summary>
        /// Doet alsof het een uitstap verwijdert
        /// </summary>
        /// <param name="uitstapID">
        /// De ID van de uitstap die verwijderd moet worden
        /// </param>
        public void BivakVerwijderen(int uitstapID)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "BivakVerwijderen {0}", uitstapID));
        }

        /// <summary>
        /// Registreren bij welke afdelingen de opgegeven persoon hoort in de opgegeven
        /// groep en voor het opgegeven werkJaar
        /// </summary>
        /// <param name="persoon">
        /// De persoon over wie het gaat
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep in kwestie
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarover het gaat
        /// </param>
        /// <param name="afdelingen">
        /// Het lijstje van afdelingen waar de persoon toe behoort
        /// </param>
        public void AfdelingenUpdaten(
                                      Persoon persoon,
                                      string stamNummer,
                                      int werkJaar,
                                      IEnumerable<AfdelingEnum> afdelingen)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "AfdelingenUpdaten P{0} SN{1} WJ{2} A{3}", persoon, stamNummer, werkJaar, afdelingen));
        }

        /// <summary>
        /// Doet alsof het registreert welke functies een persoon heeft: 
        /// andere functies worden 'verwijderd', nieuwe functies worden 'toegevoegd'
        /// </summary>
        /// <param name="persoon">
        /// De persoon voor wie we de functies willen updaten
        /// </param>
        /// <param name="stamNummer">
        /// Het stamnummer van de groep waar die persoon de opgegeven functies heeft
        /// </param>
        /// <param name="werkJaar">
        /// Het werkJaar waarin die persoon de opgegeven functies heeft
        /// </param>
        /// <param name="functies">
        /// De lijst van functies die de persoon heeft en die 'bewaard' moet worden
        /// </param>
        public void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<FunctieEnum> functies)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "FunctiesUpdaten P{0} SN{1} WJ{2} F{3}", persoon, stamNummer, werkJaar, functies));
        }

        /// <summary>
        /// Registreert in één keer alle telefoonnummers, mailadressen en andere communicatiemiddelen
        /// voor de opgegeven persoon
        /// </summary>
        /// <param name="persoon">
        /// De persoon over wie het gaat
        /// </param>
        /// <param name="communicatieMiddelen">
        /// De communicatiemiddelen die opgeslagen moeten worden
        /// </param>
        public void AlleCommunicatieBewaren(Persoon persoon, IEnumerable<CommunicatieMiddel> communicatieMiddelen)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "AlleCommunicatieBewaren P{0} CM{1}", persoon, communicatieMiddelen));
        }

        /// <summary>
        /// Aan te roepen als een voorkeursadres gewijzigd moet worden (alsof).
        /// </summary>
        /// <param name="adres">
        /// Het adres dat als standaardadres geregistreerd moet worden
        /// </param>
        /// <param name="bewoners">
        /// Een lijstje met mensen die op dat adres wonen en voor wie dat dus het standaardadres is
        /// </param>
        public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, _ikke + "StandaardAdresBewaren A{0} B{1}", adres, bewoners));
        }
    }
}
