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
    Handleiding: Privacy
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Privacy</h2>
    <p>
        De website is beveiligd, dus alleen mensen met een login kunnen gegevens bekijken
        en bewerken. Je kunt ook alleen aan gegevens van een groep waar je groepsadministratieverantwoordelijke
        (GAV) voor bent &#8211; in de meeste gevallen is dat er maar één.</p>
    <p>
        Van mensen die je aansluit, worden de volgende persoonsgegevens doorgestuurd naar
        Chirojeugd Vlaanderen:</p>
    <ul>
        <li>Naam en voornaam</li>
        <li>Geboortedatum</li>
        <li>Geslacht</li>
        <li>Het voorkeursadres<%=Html.InfoLink("VoorkeursAdres") %></li>
        <li>Mailadressen en telefoonnummers</li>
    </ul>
    <p>
        Van leden houden we bij in welke afdeling ze zitten, van leiders en leidsters bij
        welke afdeling(en) ze staan en welke officiële functies ze hebben. Officiële functies
        zijn bijvoorbeeld &#8216;contactpersoon&#8217;, &#8216;groepsleiding&#8217; en &#8216;financieel
        verantwoordelijke&#8217;.</p>
    <p>
        Van de bivakaangifte worden alleen de volgende gegevens doorgestuurd naar Chirojeugd
        Vlaanderen:</p>
    <ul>
        <li>De naam en het adres van de bivakplaats</li>
        <li>De periode waarin jullie op bivak gaan</li>
        <li>Wie jullie contactpersoon is (= de bivakverantwoordelijke, niet noodzakelijk de
            groepsleiding)</li>
    </ul>
    <p>
        Wie je inschrijft voor het bivak is alleen zichtbaar voor de groep. Van gewone uitstappen
        (weekends, daguitstappen) wordt er niets doorgegeven.</p>
    <p>
        Alle andere gegevens die je invult, zijn van je groep en zijn alleen zichtbaar voor
        de GAV&#8217;s van je groep. Chirojeugd Vlaanderen gebruikt die op geen enkele manier,
        ze worden alleen centraal opgeslagen en geback-upt.</p>
    <p>
        Als iemand uit je groep aan het nationaal secretariaat laat weten dat hij of zij
        verhuisd is, passen wij dat aan in onze administratie. Die aanpassing wordt dan
        ook doorgestuurd naar de groepsadministratiewebsite, zodat jullie ook ineens het
        juiste adres hebben.</p>
    <p>
        Chirojeugd Vlaanderen gebruikt persoonlijke gegevens enkel voor haar interne werking
        (vnl. post opsturen), voor de verzekering en voor statistische verwerking. Adres-
        en andere gegevens worden nooit doorgegeven aan derden. Als bijvoorbeeld studenten
        een enquête willen afnemen, is het het nationaal secretariaat dat de adresetiketten
        afprint en de formulieren verstuurt. Als organisaties of bedrijven reclame willen
        maken bij Chiroleden kan dat alleen via de Chiropublicaties.</p>
    <p>
        Chirojeugd Vlaanderen gebruikt mailadressen in principe niet, tenzij in twee gevallen:</p>
    <ul>
        <li>Via de Snelleberichtenlijst versturen we dringende berichtjes. De VRT zoekt een
            groep die op kamp is in de provincie Limburg, er zijn nog maar een paar plaatsen
            op Aspitrant of het leger houdt een stockverkoop: dat soort info kan niet wachten
            tot de volgende Dubbelpunt. We versturen niet veel mails, en het zijn altijd korte
            berichtjes zonder bijlagen. Je kunt per adres aanvinken of het ingeschreven moet
            worden. Meer info: <a href="http://www.chiro.be/snelleberichtenlijst" title="De maillijsten van Chirojeugd Vlaanderen, voor korte en dringende berichten">
                www.chiro.be/snelleberichtenlijst</a>.</li>
        <li>Voor enquêtes gebruiken we willekeurige adressen uit ons bestand, op basis van de
            steekproefvereisten.</li>
    </ul>
</asp:Content>
