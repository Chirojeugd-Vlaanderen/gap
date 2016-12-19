<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

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
    Handleiding: De jaarovergang uitvoeren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        De jaarovergang initiëren</h2>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Klik op het tabblad 'Jaarovergang' (dat is alleen zichtbaar tussen eind augustus
            en het moment dat een GAV van je groep het proces doorlopen heeft).</li>
        <li>Het eerste wat je moet doen, is aanvinken welke afdelingen je hebt in het nieuwe
            werkjaar. In het lijstje staan alle afdelingen die je het jaar ervoor had, dus
            meestal mag je ze gewoon allemaal aanvinken. Eventueel kun je afdelingen aanpassen
            of bijmaken. Als je klaar bent, klik je op de knop 'Volgende'.</li>
    </ul>
    <img src="<%= ResolveUrl("~/Content/Screenshots/Jaarovergang01_afdelingen_aanmaken.png") %>"
        alt="Afdelingen selecteren in jaarovergang" />
    <ul>
        <li>In het volgende scherm zie je de afdelingen die je gekozen hebt, met de officiële
            afdelingen die ermee overeenkomen, de geboortejaren en het 'geslacht'. Maak
            eventueel nog de nodige aanpassingen.</li>
    </ul>
    <img src="<%= ResolveUrl("~/Content/Screenshots/Jaarovergang02_afdelingen_verdelen.png") %>"
        alt="Afdelingen verdelen in jaarovergang" />
    <ul>
        <li>Nu heb je twee mogelijkheden. Ofwel schrijf je nog niemand in, en heb je dus
            alleen de jaarovergang uitgevoerd. Dat is in principe genoeg voor de deadline
            van 15 oktober. Schrijf dan wel zo snel mogelijk je leiding en leden in. Ofwel
            gebruik je de standaardoptie: dan krijg je ineens de kans om iedereen in te
            schrijven. Maak je keuze en klik op de knop 'Verdeling bewerken en verdergaan'.
            Aangezien iedereen sowieso een instapperiode&nbsp;<%=Html.InfoLink("INSTAP_JO") %>
            krijgt, waarin je hen nog kunt uitschrijven, is dat de makkeljkste optie.</li>
    </ul>
    <img src="<%= ResolveUrl("~/Content/Screenshots/Jaarovergang03_leden_inschrijven.png") %>"
        alt="Leden inschrijven in het nieuwe werkjaar" />
    <ul>
        <li>Al wie vorig jaar lid was, staat hier in de lijst. Twijfelgevallen en mensen
            van wie je nu al weet dat ze niet meer komen, kun je nu al afvinken. Klik op
            de knop 'Bewaren' om de procedure af te ronden.</li>
    </ul>
</asp:Content>
