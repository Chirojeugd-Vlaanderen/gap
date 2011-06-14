// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Interface voor een data access object voor adressen
    /// </summary>
    public interface IAdressenDao : IDao<Adres>
    {
        /// <summary>
        /// Haalt adres op, op basis van de adresgegevens
        /// </summary>
        /// <param name="adresInfo">adresgegevens</param>
        /// <param name="metBewoners">Indien <c>true</c>, worden ook de PersoonsAdressen
        /// opgehaald.  (ALLE persoonsadressen gekoppeld aan het adres; niet
        /// zomaar over de lijn sturen dus)</param>
        /// <returns>Gevraagd adresobject</returns>
        Adres Ophalen(AdresInfo adresInfo, bool metBewoners);

        /// <summary>
        /// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners uit de groepen met ID
        /// in <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="adresID">ID van het op te halen adres</param>
        /// <param name="groepIDs">ID's van de groepen waaruit bewoners moeten worden gehaald</param>
        /// <param name="metAlleGelieerdePersonen">Haalt alle gelieerde personen op, gekoppeld aan de personen (ook al zijn
        /// ze gelieerd aan een andere groep dan de jouwe.</param>
        /// <remarks>ALLE ANDERE ADRESSEN VAN DE GEKOPPELDE BEWONERS WORDEN OOK MEE OPGEHAALD</remarks>
        /// <returns>Het gevraagde adres met de relevante bewoners.</returns>
        Adres BewonersOphalen(int adresID, IEnumerable<int> groepIDs, bool metAlleGelieerdePersonen);
    }
}
