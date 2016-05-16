<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
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
<asp:Content ID="Content2" contentplaceholderid="HelpContent" runat="server">
    <h2>Medewerk(st)ers inschrijven voor je uitstap/bivak</h2>
    <p>
        Eerst moet je zorgen dat je uitstap
       &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Uitstap", new { helpBestand = "Trefwoorden" }, new { title = "Wat wordt hier beschouwd als een uitstap?" } ) %>
        of bivak
       &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Bivak", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een bivak?" } ) %>
        geregistreerd is. Lees eventueel eerst
        <%=Html.ActionLink("hoe je dat doet", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" })%>.
    </p>
    <p>
        Medewerkers zijn mensen die niet als lid of als leiding meegaan. Zij nemen dus niet
        deel aan het groepsleven, maar voeren andere taken uit die noodzakelijk zijn voor
        het verloop van je kamp. De kookploeg is daar het duidelijkste voorbeeld van.
    </p>
    <p>
        Vroeger moest je logistiek medewerkers dikwijls nog apart verzekeren, voor een beperkte
        periode. Dat is niet meer nodig. Ook al zijn ze niet aangesloten bij je groep, die
        mensen vallen automatisch mee onder de Chiroverzekering. Meer info daarover vind
        je <a href="https://chiro.be/administratie/verzekeringen/extra-verzekering/beperkte-periode">
            op de Chirosite</a>.
    </p>
    <p>
        Stappen in het proces:
    </p>
    <ul>
        <li>Ga naar het tabblad 'Iedereen'.</li>
        <li>Vink aan wie je wilt inschrijven, en kies in het selectielijstje onder 'Acties'
            voor 'Inschrijven voor uitstap/bivak'.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Deelnemers_inschrijven.png") %>" alt="Deelnemers selecteren en inschrijven" />
    <ul>
        <li>Je krijgt nu een nieuwe pagina te zien. Daarop staan al de mensen die je aanvinkte,
            en een selectielijstje met je uitstappen en bivakken.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Inschrijving_uitstap_bevestigen.png") %>"
        alt="De inschrijving bevestigen" />
    <ul>
        <li>Kies de juiste uitstap of het juiste kamp, vink aan dat het om logistiek medewerk(st)ers
            gaat en klik op Bewaren. Je keert dan terug naar het tabblad 'Iedereen', waar je
            de melding ziet dat die mensen ingeschreven zijn.</li>
        <li>Ga naar het tabblad 'Uitstappen/bivak' en klik op de juiste uitstap om de deelnemerslijst
            te bekijken.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Fiche_bivakdetails_met_leden.png") %>"
        alt="Detailsfiche van je uitstap, met deelnemerslijst" />
</asp:Content>
