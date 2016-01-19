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
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Deelnemers inschrijven voor je uitstap/bivak</h2>
    <p>
        Eerst moet je zorgen dat je uitstap<%=Html.InfoLink("DI_Uitstap") %>
       of bivak<%=Html.InfoLink("DI_Bivak") %> geregistreerd is. Lees eventueel eerst
        <%=Html.ActionLink("hoe je dat doet", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" })%>.</p>
    <p>
        Deelnemers zijn je leden en je leiding: mensen die ingeschreven zijn in je groep.
        Je kunt ook logistiek medewerkers<%=Html.InfoLink("DI_LogisiekMedewerkers1") %>
        inschrijven, maar dat doe je best apart (zie
        <%=Html.ActionLink("Medewerkers inschrijven", "ViewTonen", new { controller = "Handleiding", helpBestand = "MedewerkersInschrijven" })%>).</p>
    <p>
        Van een uitstap wordt er niets doorgegeven aan Chirojeugd Vlaanderen. Van een bivak
        wordt er doorgegeven wanneer jullie weg zijn, naar welk adres, en wie van jullie
        groep de contactpersoon (= kampverantwoordelijke) is. De deelnemerslijst is alleen
        voor je groep zichtbaar.</p>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Ga naar het tabblad 'Ingeschreven'. (Op het tabblad 'Iedereen' gaat dat ook, maar
            op 'Ingeschreven' ben je zeker dat de verzekering al in orde is.)</li>
        <li>Vink aan wie je wilt inschrijven, en kies in het selectielijstje onder 'Acties'
            voor 'Inschrijven voor uitstap/bivak'. Doe dat apart voor bijvoorbeeld leiding die
            met een andere afdeling meegaat om te koken op weekend: zij zijn logistiek medewerk(st)ers<%=Html.InfoLink("DI_LogisiekMedewerkers2") %>,
            en voor hen moet je een extra vinkje zetten (zie
            <%=Html.ActionLink("Medewerkers inschrijven", "ViewTonen", new { controller = "Handleiding", helpBestand = "MedewerkersInschrijven" })%>).</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Deelnemers_inschrijven.png") %>" alt="Deelnemers selecteren en inschrijven" />
    <ul>
        <li>Je krijgt nu een nieuwe pagina te zien. Daarop staan al de mensen die je aanvinkte,
            en een selectielijstje met je uitstappen en bivakken.</li></ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Inschrijving_uitstap_bevestigen.png") %>"
        alt="De inschrijving bevestigen" />
    <ul>
        <li>Kies de juiste uitstap of het juiste kamp en klik op Bewaren. Je keert dan terug
            naar het tabblad 'Ingeschreven', waar je de melding ziet dat die mensen ingeschreven
            zijn.</li>
        <li>Ga naar het tabblad 'Uitstappen/bivak' en klik op de juiste uitstap om de deelnemerslijst
            te bekijken.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Fiche_bivakdetails_met_leden.png") %>"
        alt="Detailsfiche van je uitstap, met deelnemerslijst" />
    <ul>
        <a class="anchor" id="Contactpersoon" />
        <li>Zorg dat je een contactpersoon hebt als het om een bivak gaat. Dat mag ook een logistiek
            medewerk(st)er zijn. Klik bij die persoon op de link 'contact maken'. De link verdwijnt
            en de naam staat voortaan in het vet. Foutje gemaakt? Stel gewoon de juiste persoon
            in.</li>
        <li>Klik in de deelnemerslijst op een naam om naar de deelnemersfiche te gaan. Daar
            kun je aanpassen of het inschrijvingsgeld betaald is en of de medische fiche binnen
            is.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Deelnemersinfo_bewerken.png") %>"
        alt="Deelnemersinfo bewerken" />
</asp:Content>
