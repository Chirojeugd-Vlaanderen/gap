// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
    public class UitstappenService : IUitstappenService
    {
        /// <summary>
        /// Bewaart een uitstap aan voor de groep met gegeven <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep horende bij de uitstap.
        ///  Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
        /// <param name="info">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
        ///  dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
        /// <returns>ID van de uitstap</returns>
        public int Bewaren(int groepID, UitstapInfo info)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt alle uitstappen van een gegeven groep op.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="inschrijvenMogelijk">Als deze <c>true</c> is, worden enkel de uitstappen opgehaald
        /// waarvoor je nog kunt inschrijven.  In praktijk zijn dit de uitstappen van het huidige werkJaar.
        /// </param>
        /// <returns>Details van uitstappen</returns>
        /// <remarks>We laten toe om inschrijvingen te doen voor uitstappen uit het verleden, om als dat
        /// nodig is achteraf fouten in de administratie recht te zetten.</remarks>
        public IEnumerable<UitstapInfo> OphalenVanGroep(int groepID, bool inschrijvenMogelijk)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt details over uitstap met gegeven <paramref name="uitstapID"/> op.
        /// </summary>
        /// <param name="uitstapID">ID van de uitstap</param>
        /// <returns>Details over de uitstap</returns>
        public UitstapOverzicht DetailsOphalen(int uitstapID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Bewaart de plaats voor een uitstap
        /// </summary>
        /// <param name="id">ID van de uitstap</param>
        /// <param name="plaatsNaam">Naam van de plaats</param>
        /// <param name="adres">Adres van de plaats</param>
        public void PlaatsBewaren(int id, string plaatsNaam, AdresInfo adres)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Schrijft de gelieerde personen met ID's <paramref name="gelieerdePersoonIDs"/> in voor de
        /// uitstap met ID <paramref name="geselecteerdeUitstapID" />.  Als
        /// <paramref name="logistiekDeelnemer" /> <c>true</c> is, wordt er ingeschreven als
        /// logistiek deelnemer.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van in te schrijven gelieerde personen</param>
        /// <param name="geselecteerdeUitstapID">ID van uitstap waarvoor in te schrijven</param>
        /// <param name="logistiekDeelnemer">Bepaalt of al dan niet ingeschreven wordt als 
        /// logistieker</param>
        /// <returns>De basisgegevens van de uitstap, zodat die in de feedback gebruikt kan worden</returns>
        public UitstapInfo Inschrijven(IList<int> gelieerdePersoonIDs, int geselecteerdeUitstapID, bool logistiekDeelnemer)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapID"/> op.
        /// </summary>
        /// <param name="uitstapID">ID van de relevante uitstap</param>
        /// <returns>Informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapID"/></returns>
        public IEnumerable<DeelnemerDetail> DeelnemersOphalen(int uitstapID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verwijderd een uitstap met als ID <paramref name="uitstapID"/>
        /// </summary>
        /// <param name="uitstapID">ID van de te verwijderen uitstap</param>
        /// <returns>Verwijderd de uitstap en toont daarna het overzicht scherm</returns>
        public void UitstapVerwijderen(int uitstapID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Stelt de deelnemer met gegeven <paramref name="deelnemerID" /> in als contactpersoon voor de uitstap
        /// waaraan hij deelneemt
        /// </summary>
        /// <param name="deelnemerID">ID van de als contact in te stellen deelnemer</param>
        /// <returns>De ID van de uitstap, ter controle, en misschien handig voor feedback</returns>
        public int ContactInstellen(int deelnemerID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Schrijft de deelnemer met gegeven <paramref name="deelnemerID"/> uit voor zijn uitstap.
        /// </summary>
        /// <param name="deelnemerID">ID uit te schrijven deelnemer</param>
        /// <returns>ID van de uitstap, ter controle, en h andig voor feedback</returns>
        public int Uitschrijven(int deelnemerID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt informatie over de deelnemer met ID <paramref name="deelnemerID"/> op.
        /// </summary>
        /// <param name="deelnemerID">ID van de relevante deelnemer</param>
        /// <returns>Informatie over de deelnemer met ID <paramref name="deelnemerID"/></returns>
        public DeelnemerDetail DeelnemerOphalen(int deelnemerID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Updatet een deelnemer op basis van de info in <paramref name="info"/>
        /// </summary>
        /// <param name="info">Info nodig voor de update</param>
        public void DeelnemerBewaren(DeelnemerInfo info)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt informatie over de bivakaangifte op van de groep <paramref name="groepID"/> voor diens recentste 
        /// werkJaar.
        /// </summary>
        /// <param name="groepID">
        /// De groep waarvan info wordt gevraagd
        /// </param>
        /// <returns>
        /// Een lijstje met de geregistreerde bivakken en feedback over wat er op dit moment moet gebeuren 
        /// voor de bivakaangifte
        /// </returns>
        public BivakAangifteLijstInfo BivakStatusOphalen(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }
    }
}
