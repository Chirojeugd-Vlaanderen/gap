<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
<%
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
%>
    Handleiding: Veelgestelde vragen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Veelgestelde vragen</h2>
    <ul>
        <li><a href="#NieuweGav">Mag ik mijn login doorgeven aan mijn opvolger?</a></li>
        <li><a href="#ExtraLogins">Kan ik extra logins aanvragen voor andere leiding?</a></li>
        <li><a href="#AansluitingInOrde">Hoe weet ik of mijn aansluiting in orde is?</a></li>
        <li><a href="#JaarovergangFout">De jaarovergang lukt niet.</a></li>
        <li><a href="#BuitenlandsePostcode">Bij buitenlandse adressen moet je zowel een
            postnummer als een postcode invullen. Wat is het verschil?</a></li>
        <li><a href="#WieVerzekerd">Hoe kan ik zien wie er allemaal verzekerd is?</a></li>
        <li><a href="#WieAangesloten">Hoe kan ik zien wie er allemaal aangesloten is?</a></li>
        <li><a href="#Uitstel">Kunnen we uitstel krijgen voor de jaarovergang/inschrijving/aansluiting?</a></li>
        <li><a href="#LateInschrijving">Onze inschrijvingsdag is kort voor/na de deadline
            van 15 oktober. Wat nu?</a></li>
        <li><a href="#Bestandsprobleem">Ik kan wel lijsten downloaden, maar ik krijg ze
            niet open.</a></li>
        <li><a href="#AfdelingLeiding">Ik heb leiding ingeschreven maar vergat de afdeling te selecteren</a></li>
        <li><a href="#WaaromBivakaangifte">Waarom moeten we een bivakaangifte doen?</a></li>
        <li><a href="#BivakaangifteInOrde">Hoe weet ik of mijn bivakaangifte in orde is?</a></li>
        <li><a href="#Bivakverzekering">Hoe weet ik of iedereen die meegaat op bivak verzekerd
            is?</a></li>
        <li><a href="#Bivakdeelnemers">Is het erg als de deelnemerslijst van ons bivak nog
            niet volledig is?</a></li>
    </ul>
    <h3>
        <a class="anchor" id="NieuweGav">Mag ik mijn login doorgeven aan mijn opvolger?</a></h3>
    <p>
        Het kan natuurlijk, maar het is niet de bedoeling. Een login is persoonlijk
        - hij is dan ook samengesteld uit delen van je naam. Maak voor je opvolger(s)
        een nieuwe login aan. Zie
        <%=Html.ActionLink("Gebruikersrechten regelen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "GAVsBeheren" })%>
        voor de handleiding.</p>
    <h3>
        <a class="anchor" id="ExtraLogins">Kan ik extra logins aanvragen voor mede-GAV's?</a></h3>
    <p>
        Meer zelfs: je kunt ze <%=Html.ActionLink("zelf aanmaken", "ViewTonen", new { Controller = "Handleiding", helpBestand = "GAVsBeheren" })%>.</p>
    <h3>
        <a class="anchor" id="AansluitingInOrde">Hoe weet ik of mijn aansluiting in orde
            is?</a></h3>
    <p>
        Je moet daarvoor niet bellen of mailen naar het nationaal secretariaat: als
        je nog maar pas de jaarovergang uitvoerde, wéten wij dat zelfs niet. Je kunt het wel zelf controleren:
        bovenaan zie je het juiste (nieuwe) werkjaar, en op het tabblad 'Ingeschreven'
        zie je al je leden.</p>
    <h3>
        <a class="anchor" id="JaarovergangFout">De jaarovergang lukt niet.</a></h3>
    <p>
        In de meeste gevallen heeft dat te maken met de leeftijdsgrenzen van je afdelingen.
        Je mag geen afdeling hebben voor leden die jonger zijn dan 6 jaar (zie <a href="http://www.chiro.be/minimumleeftijd"
            title="Uitleg over de minimumleeftijd voor Chiroleden">de Chirosite</a>).
        Als het daar niet aan ligt, heb je waarschijnlijk leden die te jong zijn. Schrijf
        hen uit en probeer de jaarovergang opnieuw uit te voeren. Ofwel heb je geen
        afdeling voor hun geboortejaar. Kijk de leeftijdsgrenzen na en probeer opnieuw.
        Lukt het nog niet? Neem dan contact op met <a href="http://www.chiro.be/eloket/feedback-gap">
            de helpdesk</a>.</p>
    <h3>
        <a class="anchor" id="BuitenlandsePostcode">Bij buitenlandse adressen moet je zowel
            een postnummer als een postcode invullen. Wat is het verschil?</a></h3>
    <p>
        Het postnummer is hetzelfde als in Belgische adressen: dat bestaat alleen uit
        cijfers. In Nederland zetten ze daar nog twee letters achter (bv. 1216 RA Hilversum).
        Die letters zijn de postcode.</p>
    <h3>
        <a class="anchor" id="WieVerzekerd">Hoe kan ik zien wie er allemaal verzekerd is?</a></h3>
    <p>
        Iedereen die op het tabblad 'Ingeschreven' staat, is verzekerd. Ook al is hun
        instapperiode nog niet voorbij en zijn de gegevens dus nog niet doorgestuurd
        naar het nationaal secretariaat.</p>
    <h3>
        <a class="anchor" id="WieAangesloten">Hoe kan ik zien wie er allemaal aangesloten
            is?</a></h3>
    <p>
        Iedereen die op het tabblad 'Ingeschreven' staat en niet meer in hun instapperiode
        zit, is aangesloten. De aansluiting geldt wel voor een heel werkjaar. Mensen
        die je uitschrijft, staan niet meer op het tabblad 'Ingeschreven', maar ze blijven
        wel aangesloten. Als je hen achteraf dus opnieuw inschrijft, krijg je geen nieuwe
        factuur.</p>
   <h3>
        <a class="anchor" id="Uitstel">Kunnen we uitstel krijgen voor de jaarovergang/inschrijving/aansluiting?</a></h3>
    <p>
        Op 15 oktober moet je de jaarovergang uitgevoerd hebben. Niemand krijgt uitstel.
        Die datum is al jaren een deadline: voor de verzekeringen, én voor de werking
        van het nationaal secretariaat. Dat zijn zaken waar we geen risico mee willen
        nemen.</p>
    <p>
        Dat betekent niet dat je ledenlijst al volledig in orde moet zijn. Twijfelgevallen
        schrijf je uit (bij voorkeur nog voor de jaarovergang), nieuwe leden kun je
        nog het hele jaar toevoegen.</p>
    <h3>
        <a class="anchor" id="LateInschrijving">Onze inschrijvingsdag is kort voor/na de
            deadline van 15 oktober. Wat nu?</a></h3>
    <p>
        Op 15 oktober moet de jaarovergang uitgevoerd zijn. Dat betekent niet dat je
        leden allemaal aangesloten worden op die dag. Doe je de jaarovergang op 14 oktober,
        dan krijgen je automatisch ingeschreven leden een instapperiode
       &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een instapperiode?" } ) %>
        tot 4 november. Je hebt dus tot 4 november de tijd om diegenen uit te schrijven
        die niet meer komen. Nieuwe leden kun je het hele jaar door toevoegen en inschrijven.</p>
    <h3>
        <a class="anchor" id="Bestandsprobleem">Ik kan wel lijsten downloaden, maar ik krijg
            ze niet open.</a></h3>
    <p>
        De lijsten die je kunt downloaden, zijn opgemaakt in het formaat van Office
        2010. Als je Office 2003 hebt en je krijgt het bestand niet open, dan moet je
        eerst een <a href="http://www.microsoft.com/downloads/details.aspx?familyid=941b3470-3ae9-4aee-8f43-c6bb74cd1466&displaylang=nl"
            title="Hulpprogramma om Office 2010-bestanden te kunnen openen in Office 2003">
            extra programma</a> installeren. Zorg ook dat je <a href="http://update.microsoft.com">
                de recentste updates van Office</a> geïnstalleerd hebt.</p>
    <p>
        Heb je geen programma waarmee je het bestand kunt openen? Calc van <a href="http://www.libreoffice.org/download">
            LibreOffice</a> is gratis.</p>
    <p>
        Microsoft Office voor Mac kan blijkbaar problemen hebben met de geboortedatums,
        daar moet je dus mee opletten.</p>
    <h3><a class="anchor" id="AfdelingLeiding">Ik heb leiding ingeschreven maar vergat de afdeling te selecteren</a></h3>
    <p>Leiding ingeschreven zonder de afdeling te kiezen? Dat kan vervelend zijn. Omdat de leiding 
        van de jongste afdelingen in het voorjaar het spel voor hun leden moet krijgen, omdat we aspileiding info willen geven over Aspitrant, 
        en omdat dat anders allemaal bij de contactpersoon van je groep aankomt en hij of zij voor de verdere verdeling moet zorgen. Geen nood: 
        dat is makkelijk aan te passen! Ga naar het tabblad Ingeschreven en klik daar op de naam van de leiding. Op de persoonlijke fiche klik 
        je bij Chirogegevens op het potloodicoontje naast Afdelingen. In het pop-up-venstertje vink je de juiste afdeling(en).</p>
    <h3>
        <a class="anchor" id="WaaromBivakaangifte">Waarom moeten we een bivakaangifte doen?</a></h3>
    <p>
        We vragen je bivakgegevens in de eerste plaats om je bij noodgevallen te kunnen
        bereiken of efficiënt te kunnen ondersteunen, of het nu gaat om een ouder die
        dringend zijn of haar kind moet bereiken, een (natuur)ramp waarvan we je op
        de hoogte moeten brengen of een probleem waarvoor jij ons contacteert.
        <br />
        Daarnaast kunnen we met die gegevens het fenomeen bivak iets beter opvolgen
        en ondersteunen.
    </p>
    <h3>
        <a class="anchor" id="BivakaangifteInOrde">Hoe weet ik of mijn bivakaangifte in
            orde is?</a></h3>
    <p>
        Je kunt dat zelf controleren. Zie je op het tabblad 'Uitstappen/bivak' in het
        lijstje je kamp staan? Staat er een vinkje in de kolom 'bivak'? En als je naar
        de deelnemerslijst kijkt, vind je daar dan een contactpersoon op? (Die staat
        in het vet, en er staat geen link bij 'contact maken'.) In orde!</p>
    <h3>
        <a class="anchor" id="Bivakverzekering">Hoe weet ik of iedereen die meegaat op bivak
            verzekerd is?</a></h3>
    <p>
        Ingeschreven leden, leiders en leidsters zijn sowieso verzekerd. Bij hen staat
        er op de deelnemerslijst 'lid' of 'leiding'. Staat er '???', dan moet je hen
        nog inschrijven in je groep. Logistiek medewerk(st)ers zijn verzekerd als ze
        aangesloten zijn, anders heb je een verzekering voor een beperkte periode nodig
        (zie <a href="http://www.chiro.be/verzekeringen">www.chiro.be/verzekeringen</a>).
        Of dat in orde is, kan niet getoond worden op de GAP-website, daar moet je dus
        zelf op letten.
    </p>
    <h3>
        <a class="anchor" id="Bivakdeelnemers">Is het erg als de deelnemerslijst van ons
            bivak nog niet volledig is?</a></h3>
    <p>
        Nee hoor. Van de bivakaangifte worden alleen de volgende gegevens doorgestuurd
        naar Chirojeugd Vlaanderen:</p>
    <ul>
        <li>De naam en het adres van de bivakplaats</li>
        <li>De periode waarin jullie op bivak gaan</li>
        <li>Wie jullie contactpersoon is (= de bivakverantwoordelijke, niet noodzakelijk
            de groepsleiding)</li>
    </ul>
    <p>
        Wie je inschrijft voor het bivak is alleen zichtbaar voor de groep. Van gewone
        uitstappen (weekends, daguitstappen) wordt er niets doorgegeven.</p>
</asp:Content>
