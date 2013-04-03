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
    Handleiding: Trefwoorden
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Trefwoorden</h2>
    <dl class="Woordenlijst">
        <dt><strong><a class="anchor" id="Aansluiting">Aansluiting</a></strong>:</dt>
        <dd id="AANINFO">
            Je bent aangesloten bij Chirojeugd Vlaanderen wanneer een groep, gewest, verbond
            of nationale vrijwilligersploeg je inschrijft en je instapperiode <a href="#Instapperiode">
                [?]</a> is verstreken. Een inschrijving kan ongedaan gemaakt worden in de loop
            van het werkjaar, een aansluiting niet.<br />
            Je groep is aangesloten bij Chirojeugd Vlaanderen als je de jaarovergang uitgevoerd
            hebt en minstens één leider of leidster ingeschreven hebt.
        </dd>
        <dt ><strong><a class="anchor" id="AD-nummer">AD-nummer</a></strong>:</dt>
        <dd id="ADINFO">
            Iedereen die in de administratie van het nationaal Chirosecretariaat terechtkomt,
            krijgt een administratief identificatienummer. Je hebt dat nodig als je bepaalde
            zaken wilt op-of aanvragen, en Chirojeugd Vlaanderen gebruikt het om je makkelijk
            terug te vinden.</dd>
        <dt><strong><a class="anchor" id="Bivak">Bivak</a></strong>:</dt>
        <dd id="BIVINFO">
            Jeugudbewegingen gaan elk jaar in de zomer op kamp. In de Chiro noemen we dat ook
            wel eens bivak. Typisch voor de Chiro is dat we met heel de groep samen ergens naartoe
            trekken - al zijn er wel uitzonderingen. Sommige groepen gaan bijvoorbeeld apart
            op kamp met hun jongste afdeling. Ook dat geldt als een bivak, en dus moet je er
            een bivakaangifte <a href="#Bivakaangifte">[?]</a> voor doen. Alle info over verzekeringen,
            administratie en praktische regelingen vind je op <a href="http://www.chiro.be/bivak">
                www.chiro.be/bivak</a> en in <a href="http://www.chiro.be/info-voor-leiding/uitgaven/boeken/bivakboek">
                    het Bivakboek</a>.
        </dd>
        <dt><strong><a class="anchor" id="Bivakaangifte">Bivakaangifte</a></strong>:</dt>
        <dd id="BAINFO">
            Voor 1 juni moet je de gegevens van je bivakplaats doorgeven aan het nationaal secretariaat,
            en je moet aangeven wie de contactpersoon is. Zo kan het secretariaat met jullie
            contact opnemen wanneer iemand van de ouders je niet kan bereiken of wanneer er
            iets aan de hand is met je bivakplaats. Dat doorgeven gebeurt automatisch wanneer
            je je bivak registreert op deze website en daarbij aanvinkt dat het om het jaarlijks
            bivak gaat. Dat moet ook bij aparte kampen voor één of twee afdelingen. Alleen die
            gegevens worden doorgegeven, de deelnemerslijst is alleen voor jouw groep zichtbaar.</dd>
        <dt><strong><a class="anchor" id="Categorie">Categorie</a></strong>:</dt>
        <dd id="CATINFO">
            Een categorie is een label dat je zelf kunt kiezen. Geef ze een naam (bv. 'kookploeg')
            en een afkorting (bv. 'KP' of 'kok'). Mensen die je aan een categorie toevoegt,
            kun je makkelijk terugvinden door in het personenoverzicht (klik bovenaan rechts
            op 'Iedereen') te selecteren op categorie.</dd>
        <dt><strong><a class="anchor" id="Chiroleeftijd">Chiroleeftijd</a></strong>:</dt>
        <dd id="CLINFO">
            Bij de jaarovergang worden leden automatisch in de afdeling gestopt die overeenkomt
            met hun geboortejaar. Voor leden die een jaar zijn blijven zitten of om een andere
            reden niet bij degenen van hun leeftijd zitten, wil je dat niet. Pas voor hen de
            Chiroleeftijd aan: bv. -1 voor iemand die een jaar is blijven zitten, dan gaat hij
            of zij altijd mee met degenen die een jaar jonger zijn.
            <%=Html.ActionLink("Hoe doe je dat?", "ViewTonen", new { controller = "Handleiding", helpBestand = "ChiroleeftijdAanpassen" })%>
        </dd>
        <dt><strong><a class="anchor" id="Contactpersoon">Contactpersoon</a></strong>:</dt>
        <dd id="CPINFO">
            De contactpersoon is degene die de post van het nationaal secretariaat zal ontvangen.</dd>
        <dt><strong><a class="anchor" id="Factuur">Factuur</a></strong>:</dt>
        <dd id="FACINFO">
            Op geregelde tijdstippen verzamelt het nationaal secretariaat alle bewerkingen die
            groepen deden en waarvoor er betaald moet worden. De eerste keer gebeurt dat na
            15 oktober, de uiterste datum waarop je de jaarovergang moet uitvoeren. Daarna is
            dat om de zoveel weken - het kan dus wel een tijdje duren voor je financieel verantwoordelijke
            zo'n factuur effectief in de bus krijgt.<br />
            Je kunt facturen krijgen voor aansluitingen, Dubbelpuntabonnementen en bijkomende
            verzekeringen. De prijs van abonnementen en verzekeringen staat altijd ergens vermeld
            - zo kom je niet voor verrassingen te staan.</dd>
        <dt><strong><a class="anchor" id="FinancieelVerantwoordelijke">Financieel verantwoordelijke</a></strong>:</dt>
        <dd id="FVINFO">
            De financieel verantwoordelijke is degene naar wie de facturen opgestuurd worden.</dd>
        <dt><strong><a class="anchor" id="GAV">GAV</a></strong>:</dt>
        <dd id="GAVINFO">
            Groepsadministratieverantwoordelijke. Aangezien jij deze website gebruikt, ben je
            een GAV van jouw groep. Een groep kan zoveel GAV's hebben als je zelf wilt. Ze moeten
            wel allemaal een eigen login hebben voor deze website.</dd>
        <dt><strong><a class="anchor" id="GezinsgebondenCommunicatievorm">Gezinsgebonden communicatievormen</a></strong>:</dt>
        <dd id="GB-COMINFO">
            Een communicatievorm is bijvoorbeeld een telefoonnummer of een mailadres. Je kunt
            die allemaal met hetzelfde formulier toevoegen. Zo'n communicatievorm is gezinsgebonden
            als je er alle gezinsleden mee kunt bereiken - en dus ook de ouders van je leden,
            bijvoorbeeld. Een typisch voorbeeld is een telefoonnummer van een vaste lijn, maar
            je kunt ook het gsm-nummer van vader of moeder opgeven als gezinsgebonden voor je
            leden.</dd>
        <dt><strong><a class="anchor" id="Inschrijven">Inschrijven</a></strong>:</dt>
        <dd id="INSCHRINFO">
            Mensen die je inschrijft, maak je lid van je groep. Je vindt hen op het tabblad
            'Ingeschreven', en ze zijn verzekerd bij Chiroactiviteiten.<br />
            Iedereen krijgt bij de inschrijving een instapperiode <a href="#Instapperiode">[?]</a>.
            Pas nadat die verlopen is, word je effectief aangesloten bij Chirojeugd Vlaanderen,
            en kan het nationaal secretariaat daarvoor een factuur <a href="#Factuur">[?]</a>
            opmaken.</dd>
        <dt><strong><a class="anchor" id="Inschrijvingsvoorwaarden">Inschrijvingsvoorwaarden</a></strong>:</dt>
        <dd id="INSCHR-VWINFO">
            Je kunt enkel mensen inschrijven die aan alle voorwaarden voldoen, dus zie je in
            het personenoverzicht (tabblad 'Iedereen') alleen bij die mensen een link waarmee
            dat kan. Voorwaarden zijn: nog niet ingeschreven zijn in het lopende werkjaar (evident),
            de geboortedatum moet ingevuld zijn en de persoon moet de minimumleeftijd hebben
            om lid te mogen worden. De Chiro sluit namelijk geen kleuters aan - zie <a href="http://www.chiro.be/minimumleeftijd"
                title="Uitleg over de minimumleeftijd voor Chiroleden">de Chirosite</a>.</dd>
        <dt><strong><a class="anchor" id="Instapperiode">Instapperiode</a></strong>:</dt>
        <dd id="INSINFO">
            Iedereen die je inschrijft in je groep krijgt een instapperiode. De einddatum vind
            je op de persoonsfiche. Normaal duurt een instapperiode drie weken, maar als je
            de jaarovergang al in augustus of begin september uitvoert, is de einddatum 15 oktober:
            de deadline voor de jaarovergang. Je hebt dus zeker tijd genoeg om je ledenlijst
            nog na te kijken, ook als je inschrijvingen nog niet allemaal binnen zijn of als
            je bv. begin oktober een inschrijvingsdag houdt.<br />
            De GAV's krijgen een mailtje wanneer er zo'n einddatum nadert. Is de einddatum verstreken,
            dan geldt die persoon als definitief ingeschreven. Op dat moment wordt hij of zij
            aangesloten bij Chirojeugd Vlaanderen. Dat betekent dat je groep een factuur zal
            krijgen voor zijn of haar lidgeld.</dd>
        <dt><strong><a class="anchor" id="Jaarovergang">Jaarovergang</a></strong>:</dt>
        <dd id="JOINFO">
            Tijdens de jaarovergang geef je aan welke afdelingen je groep heeft. De leden van
            vorig jaar worden automatisch in de juiste afdeling gezet en ingeschreven. Voor
            die juiste afdeling kijkt het programma naar de leeftijd en de Chiroleeftijd
           &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?"})%>.
            Bij die inschrijving krijgen al die leden een instapperiode
           &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een instapperiode?"})%>.
            Als ze niet meer naar de Chiro willen komen, moet je ze uitschrijven voor het einde
            van die instapperiode, anders krijg je toch een factuur voor hun lidgeld. Werk je
            met een inschrijvingsdag die nogal laat valt? Voer de jaarovergang dan kort voor
            de deadline uit: inschrijven op 14 oktober betekent een instapperiode tot 4 november.
            Als dat nog te vroeg is, neem je onverantwoorde risico's en pas je het beste je
            werking aan.</dd>
        <dt><strong><a class="anchor" id="Ledenuitgave">Ledenuitgave</a></strong>:</dt>
        <dd id="LUINFO">
            Chirojeugd Vlaanderen maakt verschillende publicaties voor leden. Voor de jongste
            afdelingen sturen we een boekje of een spel naar de afdelingsleiding, keti's en
            aspi's krijgen Kramp thuisbezorgd.<br />
            Wanneer je een afdeling
            <%=Html.ActionLink("activeert voor het huidige werkjaar", "ViewTonen", new { controller = "Handleiding", helpBestand = "AfdelingActiveren" })%>
            moet je aangeven met welke offici&euml;le afdeling ze overeenkomt. Zo weet het nationaal
            secretariaat welke uitgave we moeten opsturen, en zo krijgt je hele afdeling dezelfde
            publicatie.</dd>
        <dt><strong><a class="anchor" id="Lid">Lid</a></strong>:</dt>
        <dd id="LIDINFO">
            Een persoon die ingeschreven is in de groep en (na de instapperiode <a href="#Instapperiode">
                [?]</a>) dus aangesloten <a href="#Aansluiting">[?]</a> bij Chirojeugd Vlaanderen</dd>
        <dt><strong><a class="anchor" id="Lidgeld">Lidgeld</a></strong>:</dt>
        <dd id="LGINFO">
            Op de persoonsfiche van ingeschreven leden kun je aanduiden of zij hun lidgeld al
            betaald hebben. Dat is informatie voor je groep, niet voor het nationaal secretariaat.
            Lidgeld is wat je leden aan je groep betalen om aan de activiteiten te mogen deelnemen.
            Je groep betaalt aan Chirojeugd Vlaanderen voor de aansluiting en dus de verzekering.
            Iedereen die ingeschreven is, is verzekerd. Iedereen die ingeschreven is én voor
            wie de instapperiode verstreken is, is aangesloten.</dd>
        <dt><strong><a class="anchor" id="LogistiekMedewerkers">Logistiek medewerk(st)ers</a></strong>:</dt>
        <dd id="LMINFO">
            Je kunt op uitstappen <a href="#Uitstap">[?]</a> en bivakken <a href="#Bivak">[?]</a>
            ook mensen meenemen die geen lid zijn van je groep en die geen afdeling begeleiden.
            De kookploeg, bijvoorbeeld. Als je hen inschrijft, komen ze ook op de deelnemerslijst.
            Let wel: iemand die niet aangesloten is, is niet automatisch verzekerd op zo'n uitstap!
            Je kunt wel een verzekering voor een beperkte periode afsluiten (zie <a href="http://www.chiro.be/verzekeringen">
                www.chiro.be/verzekeringen</a>).
        </dd>
        <dt><strong><a class="anchor" id="Persoon">Persoon</a></strong>:</dt>
        <dd id="PERINFO">
            In deze handleiding bedoelen we met een persoon om het even wie van wie je gegevens
            wilt bijhouden in je groepsadministratie. Dat zijn in de eerste plaats de mensen
            die ingeschreven moeten worden in je groep: je leden en je leiding. Maar je kunt
            ook andere (contact)gegevens bijhouden: van je kookploeg, oud-leiding, contactpersonen
            uit de jeugdraad of het parochieteam, noem maar op. Die informatie is dan ineens
            beschikbaar voor alle <a href="#GAV">GAV</a>'s van je groep. En wil je al die mensen
            makkelijk terugvinden tussen die honderden personen? Stop ze dan in duidelijke <a
                href="#Categorie">categorieën</a>.</dd>
        <dt><strong><a class="anchor" id="Snelleberichtenlijsten">Snelleberichtenlijsten</a></strong>:</dt>
        <dd id="SBLINFO">
            Chirojeugd Vlaanderen heeft een aantal mailinglijsten. We gebruiken die niet om
            je te overstelpen met mails, wel om je snel te kunnen bereiken als we dringend nieuws
            hebben. Zo zijn er lijsten voor leiding, kaderleiding, groepsleiding, VB's en jeugdraadvertegenwoordigers.
            Meer info vind je <a href="http://www.chiro.be/Snelleberichtenlijst">op de Chirosite</a>.</dd>
        <dt><a class="anchor" id="Uitschrijven"></a><strong>Uitschrijven</strong>:</dt>
        <dd id="UITSCHRINFO">
            Iemand uitschrijven is een drastische maatregel. In principe kan het ook niet -
            toch niet voor het nationaal secretariaat. Wanneer je iemand aansluit, is dat voor
            een heel werkjaar. Je kunt dus ook geen lidgeld terugvorderen, want daar zijn al
            een hoop vaste kosten mee betaald, zoals de verzekering.<br />
            In je groep kun je wel de beslissing nemen om iemand uit te schrijven. In het programma
            heet dat: 'op non-actief zetten'. De persoon blijft wel in je gegevensbestand zitten,
            maar hij of zij verschijnt niet meer bij de ingeschreven leden.
            <%=Html.ActionLink("Hoe doe je dat?", "ViewTonen", new { controller = "Handleiding", helpBestand = "Uitschrijven" })%></dd>
        <dt><strong><a class="anchor" id="Uitstap">Uitstap</a></strong>:</dt>
        <dd id="UITINFO">
            Ga je op weekend met je afdeling, naar het zwembad met heel de groep, of op bivak?
            Dat kun je op deze website registreren, zodat je een deelnemerslijst kunt opmaken.
            Een bivak <em>moet</em> je registreren (zie Bivakaangifte <a href="#Bivakaangifte">[?]</a>),
            voor andere uitstappen mag je zelf kiezen of je ze registreert.
            <%=Html.ActionLink("Hoe doe je dat?", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" })%>
        </dd>
        <dt><strong><a class="anchor" id="Voorkeursadres">Voorkeursadres</a></strong>:</dt>
        <dd id="VK-ADRINFO">
            Elke persoon kan verschillende adressen hebben, van verschillende types (thuis,
            kot, enz.). Het voorkeursadres is waar de post moet aankomen. Dat is dus het adres
            dat in het Excel-bestand staat dat je kunt downloaden, en dat doorgestuurd wordt
            naar het nationaal secretariaat op het moment dat je aangesloten wordt.</dd>
    </dl>
</asp:Content>
