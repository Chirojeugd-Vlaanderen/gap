using System;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using GebruikersRecht = Chiro.Gap.ServiceContracts.DataContracts.GebruikersRecht;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Interface voor de service voor gebruikersrechtenbeheer.
    /// </summary>
    public class GebruikersService : IGebruikersService
    {
        /// <summary>
        /// Als de persoon met gegeven <paramref name="gelieerdePersoonID"/> nog geen account heeft, wordt er een
        /// account voor gemaakt. Aan die account worden dan de meegegeven <paramref name="gebruikersRechten"/>
        /// gekoppeld.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen. Mag leeg zijn. Bestaande 
        /// gebruikersrechten worden zo mogelijk verlengd als ze in <paramref name="gebruikersRechten"/> 
        /// voorkomen, eventuele bestaande rechten niet in <paramref name="gebruikersRechten"/> blijven
        /// onaangeroerd.
        /// </param>
        public void RechtenToekennen(int gelieerdePersoonID, GebruikersRecht[] gebruikersRechten)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Geeft de account met gegeven <paramref name="gebruikersNaam"/> de gegeven
        /// <paramref name="gebruikersRechten"/>.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// De gegeven accout moet bestaan; we moeten vermijden dat eender welke user zomaar accounts
        /// kan maken voor chiro.wereld.
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van de account die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen.
        /// Bestaande gebruikersrechten worden zo mogelijk verlengd als ze in 
        /// <paramref name="gebruikersRechten"/> voorkomen, eventuele bestaande rechten niet in 
        /// <paramref name="gebruikersRechten"/> blijven onaangeroerd.
        /// </param>
        public void RechtenToekennenGebruiker(string gebruikersNaam, GebruikersRecht[] gebruikersRechten)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gelieerdePersoonID"/> af voor de groepen met gegeven <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIDs">ID's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemen(int gelieerdePersoonID, int[] groepIDs)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gebruikersNaam"/> af voor de groepen met gegeven <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIDs">ID's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemenGebruiker(string gebruikersNaam, int[] groepIDs)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }
    }
}