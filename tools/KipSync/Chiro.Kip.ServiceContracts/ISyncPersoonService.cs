/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.ServiceModel;

using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.ServiceContracts
{
    /// <summary>
    /// Servicecontract voor communicatie GAP-&gt;KIP
    /// <para>
    /// </para>
    /// BELANGRIJK: Oorspronkelijk werden voor de meeste methods geen personen over de lijn gestuurd, maar enkel
    /// AD-nummers.  Het idee daarachter was dat toch enkel gegevens van personen met AD-nummer naar kipadmin
    /// gesynct moeten worden.
    /// <para>
    /// </para>
    /// Maar met het AD-nummer alleen kom je er niet.  Het kan namelijk goed zijn dat een persoon gewijzigd wordt
    /// tussen het moment dat hij voor het eerst lid wordt, en het moment dat hij zijn AD-nummer krijgt.  Deze
    /// wijzigingen willen we niet verliezen.
    /// <para>
    /// </para>
    /// Het PersoonID van GAP meesturen helpt in de meeste gevallen.  Maar dat kan mis gaan op het moment dat een persoon
    /// uit kipadmin nog dubbel in GAP zit.  Vooraleer deze persoon zijn AD-nummer krijgt, weten we dat immers niet.
    /// <para>
    /// </para>
    /// Vandaar dat nu alle methods volledige persoonsobjecten gebruiken, zodat het opzoeken van een persoon zo optimaal
    /// mogelijk kan gebeuren.  Het persoonsobject een AD-nummer heeft, wordt er niet naar de rest gekeken.
    /// </summary>
    [ServiceContract]
    public interface ISyncPersoonService
    {
        #region persoonsgegevens

        /// <summary>
        /// Updatet een persoon in Kipadmin op basis van de gegevens in GAP.  Als er geen AD-nummer is, dan doen we
        /// een schamele poging om de persoon al te vinden.   Als ook dat niet lukt, maken we een nieuwe persoon aan.
        /// Bij ontbrekend AD-nummer, en wordt het achteraf ingevuld bij <paramref name="persoon"/>.  (Niet interessant
        /// voor service, maar wel voor andere methods die deze aanroepen).
        /// </summary>
        /// <param name="persoon">
        /// Informatie over een geupdatete persoon in GAP
        /// </param>
        /// <remarks>
        /// Als AD-nummer ontbreekt, wordt er sowieso een nieuwe persoon gemaakt.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void PersoonUpdaten(Persoon persoon);

        /// <summary>
        /// Aan te roepen als een voorkeursadres gewijzigd moet worden.
        /// </summary>
        /// <param name="adres">
        /// Nieuw voorkeursadres
        /// </param>
        /// <param name="bewoners">
        /// AD-nummers en adrestypes voor personen de dat adres moeten krijgen
        /// </param>
        [OperationContract(IsOneWay = true)]
        void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners);

        /// <summary>
        /// Voegt 1 communicatiemiddel toe aan de communicatiemiddelen van een persoon
        /// </summary>
        /// <param name="persoon">
        /// Persoon die het nieuwe communicatiemiddel krijgt
        /// </param>
        /// <param name="communicatieMiddel">
        /// Het nieuwe communicatiemiddel
        /// </param>
        [OperationContract(IsOneWay = true)]
        void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel);

        /// <summary>
        /// Gaat op zoek naar de gegeven <paramref name="persoon"/>, en zoekt daarvan de communicatie van
        /// type <c>communicatieMiddel.Type</c> en nummer <paramref name="nummerBijTeWerken"/>. Dat
        /// gevonden communicatiemiddel wordt vervangen door <paramref name="communicatieMiddel"/>.
        /// </summary>
        /// <param name="persoon">persoon met te vervangen communicatiemiddel</param>
        /// <param name="nummerBijTeWerken">huidig nummer van te vervangen communicatiemiddel</param>
        /// <param name="communicatieMiddel">nieuwe info voor te vervangen communicatiemiddel</param>
        [OperationContract(IsOneWay = true)]
        void CommunicatieBijwerken(Persoon persoon, string nummerBijTeWerken, CommunicatieMiddel communicatieMiddel);

        /// <summary>
        /// Verwijdert alle bestaande contactinfo, en vervangt door de contactinfo meegegeven in 
        /// <paramref name="communicatieMiddelen"/>.
        /// </summary>
        /// <param name="persoon">
        /// Persoon voor wie we contactinfo updaten
        /// </param>
        /// <param name="communicatieMiddelen">
        /// Te updaten contactinfo
        /// </param>
        [OperationContract(IsOneWay = true)]
        void AlleCommunicatieBewaren(Persoon persoon, IEnumerable<CommunicatieMiddel> communicatieMiddelen);

        /// <summary>
        /// Verwijdert een communicatiemiddel uit Kipadmin.
        /// </summary>
        /// <param name="persoon">
        /// Persoonsgegevens van de persoon waarvan het communicatiemiddel moet verdwijnen.
        /// </param>
        /// <param name="communicatieMiddel">
        /// Gegevens over het te verwijderen communicatiemiddel
        /// </param>
        [OperationContract(IsOneWay = true)]
        void CommunicatieVerwijderen(Persoon persoon, CommunicatieMiddel communicatieMiddel);

        #endregion

        #region lidgegevens

        /// <summary>
        /// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer van het opgegeven lid
        /// </param>
        /// <param name="gedoe">
        /// De nodige info voor het lid.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void LidBewaren(int adNummer, LidGedoe gedoe);

        /// <summary>
        /// Maakt een persoon zonder ad-nummer lid.
        /// </summary>
        /// <param name="details">
        /// Details van de persoon die lid moet kunnen worden
        /// </param>
        /// <param name="lidGedoe">
        /// Nodige info om lid te kunnen maken
        /// </param>
        [OperationContract(IsOneWay = true)]
        void NieuwLidBewaren(PersoonDetails details, LidGedoe lidGedoe);

        /// <summary>
        /// Verwijdert een persoon met gekend AD-nummer als lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer te verwijderen lid
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer te verwijderen lid
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar te verwijderen lid
        /// </param>
        /// <param name="uitschrijfDatum"> uitschrijfdatum zoals geregistreerd in GAP</param>
        /// <remarks>
        /// Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void LidVerwijderen(int adNummer, string stamNummer, int werkJaar, DateTime uitschrijfDatum);

        /// <summary>
        /// Verwijdert een lid als het ad-nummer om een of andere reden niet bekend is.
        /// </summary>
        /// <param name="details">
        /// Gegevens die hopelijk toelaten het lid te identificeren
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van het lid
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar van het lid
        /// </param>
        /// <param name="uitschrijfDatum">uitschrijfdatum zoals geregistreerd in GAP</param>
        /// <remarks>
        /// Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkJaar, DateTime uitschrijfDatum);

        /// <summary>
        /// Updatet de functies van een lid.
        /// </summary>
        /// <param name="persoon">
        /// Persoon van wie de lidfuncties geüpdatet moeten worden
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van de groep waarin de persoon lid is
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar waarin de persoon lid is
        /// </param>
        /// <param name="functies">
        /// Toe te kennen functies.  Eventuele andere reeds toegekende functies worden verwijderd.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<FunctieEnum> functies);

        /// <summary>
        /// Stelt het lidtype van het lid in, bepaald door <paramref name="persoon"/>, <paramref name="stamNummer"/>
        /// en <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="persoon">
        /// Persoon van wie het lidtype aangepast moet worden
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van groep waarin de persoon lid is
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar waarvoor het lidtype moet aangepast worden
        /// </param>
        /// <param name="lidType">
        /// Nieuw lidtype
        /// </param>
        [OperationContract(IsOneWay = true)]
        void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType);

        /// <summary>
        /// Updatet de afdelingen van een lid.
        /// </summary>
        /// <param name="persoon">
        /// Persoon waarvan de afdelingen geupdatet moeten worden
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van de groep waarin de persoon lid is
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar waarin de persoon lid is
        /// </param>
        /// <param name="afdelingen">
        /// Toe te kennen afdelingen.  Eventuele andere reeds toegekende functies worden verwijderd.
        /// </param>
        /// <remarks>
        /// Er is in Kipadmin maar plaats voor 2 afdelingen/lid
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void AfdelingenUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen);

        #endregion

        #region verzekeringen

        /// <summary>
        /// Verzekert de gegeven persoon in het gegeven groepswerkjaar tegen loonverlies, gegeven dat de persoon
        /// een AD-nummer heeft
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer van te verzekeren persoon
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van betalende groep
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar voor de verzekering
        /// </param>
        [OperationContract(IsOneWay = true)]
        void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar);

        /// <summary>
        /// Verzekert een persoon waarvan we het AD-nummer nog niet kennen tegen loonverlies
        /// </summary>
        /// <param name="details">
        /// Details van de te verzekeren persoon
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van betalende groep
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar voor de verzekering
        /// </param>
        [OperationContract(IsOneWay = true)]
        void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar);

        #endregion

        #region bivak

        /// <summary>
        /// Bewaart een bivak, zonder contactpersoon of adres
        /// </summary>
        /// <param name="bivak">
        /// Gegevens voor de bivakaangifte
        /// </param>
        [OperationContract(IsOneWay = true)]
        void BivakBewaren(Bivak bivak);

        /// <summary>
        /// Bewaart <paramref name="plaatsNaam"/> en <paramref name="adres"/> voor een bivak
        /// in Kipadmin.
        /// </summary>
        /// <param name="uitstapID">
        /// ID van de uitstap in GAP
        /// </param>
        /// <param name="plaatsNaam">
        /// Naam van de bivakplaats
        /// </param>
        /// <param name="adres">
        /// Adres van de bivakplaats
        /// </param>
        [OperationContract(IsOneWay = true)]
        void BivakPlaatsBewaren(int uitstapID, string plaatsNaam, Adres adres);

        /// <summary>
        /// Stelt de persoon met gegeven <paramref name="adNummer"/> in als contactpersoon voor
        /// het bivak met gegeven <paramref name="uitstapID"/>
        /// </summary>
        /// <param name="uitstapID">
        /// UitstapID (GAP) voor het bivak
        /// </param>
        /// <param name="adNummer">
        /// AD-nummer contactpersoon bivak
        /// </param>
        [OperationContract(IsOneWay = true)]
        void BivakContactBewaren(int uitstapID, int adNummer);

        /// <summary>
        /// Stelt de persoon met gegeven <paramref name="details"/> in als contactpersoon voor
        /// het bivak met gegeven <paramref name="uitstapID"/>
        /// </summary>
        /// <param name="uitstapID">
        /// UitstapID (GAP) voor het bivak
        /// </param>
        /// <param name="details">
        /// Gegevens van de persoon
        /// </param>
        /// <remarks>
        /// Deze method mag enkel gebruikt worden als het ad-nummer van de
        /// persoon onbestaand of onbekend is.
        /// </remarks>
        [OperationContract(IsOneWay = true)]
        void BivakContactBewarenAdOnbekend(int uitstapID, PersoonDetails details);

        /// <summary>
        /// Verwijdert een bivak uit kipadmin.
        /// </summary>
        /// <param name="uitstapID">
        /// UitstapID (GAP) van het te verwijderen bivak
        /// </param>
        [OperationContract(IsOneWay = true)]
        void BivakVerwijderen(int uitstapID);

        #endregion

        #region groep

        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        [OperationContract(IsOneWay = true)]
        void GroepUpdaten(Groep g);

        #endregion

    }
}
