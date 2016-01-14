<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
    Handleiding: Uitstappen/bivak
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Uitstappen en bivakken</h2>
    <p>
        Je kunt de GAP-website gebruiken voor elke uitstap die je organiseert. Dat is voor
        je eigen gemak. Eén uitstap <em>moet</em> je in het systeem steken, en wel voor
        1 juni: het jaarlijkse bivak.</p>
    <p>
        We vragen je bivakgegevens in de eerste plaats om je bij noodgevallen te kunnen
        bereiken of efficiënt te kunnen ondersteunen - of het nu gaat om een ouder die dringend
        zijn of haar kind moet bereiken, een (natuur)ramp waarvan we je op de hoogte moeten
        brengen of een probleem waarvoor jij ons contacteert.
        <br />
        Daarnaast kunnen we met die gegevens het fenomeen bivak iets beter opvolgen en ondersteunen.</p>
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
    <h3>
        Wat kun je hier doen?</h3>
    <ul>
        <li>
            <%=Html.ActionLink("Een uitstap/bivak toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" })%></li>
        <li>
            <%=Html.ActionLink("Een uitstap/bivak bewerken", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapBewerken" })%></li>
        <li>
            <%=Html.ActionLink("Deelnemersadministratie", "ViewTonen", new { controller = "Handleiding", helpBestand = "Deelnemersadministratie" })%></li>
    </ul>
    <h3>Wat moet je op een andere pagina doen?</h3>
    <ul>
        <li>
            <%=Html.ActionLink("Deelnemers inschrijven", "ViewTonen", new { controller = "Handleiding", helpBestand = "DeelnemersInschrijven" })%></li>
            <li>
            <%=Html.ActionLink("Logistiek medewerkers (zoals koks) inschrijven", "ViewTonen", new { controller = "Handleiding", helpBestand = "MedewerkersInschrijven" })%></li>
    </ul>
</asp:Content>
