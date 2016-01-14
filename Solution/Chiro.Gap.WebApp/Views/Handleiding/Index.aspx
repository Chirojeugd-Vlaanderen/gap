<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master" Inherits="System.Web.Mvc.ViewPage" %>

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
    Handleiding GAP
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="helpContent" runat="server">
    <% 
        // Van mei tot en met augustus tonen we bovenaan welke stappen er nodig zijn voor de bivakaangifte
        if (DateTime.Now.Month > 4 && (DateTime.Now.Month <= 8 && DateTime.Now.Day < 15))
        { %>
    <h2>
        Tijd voor de bivakaangifte!</h2>
    <p>
        Breng voor 1 juni je bivakaangifte in orde. Zo kunnen we je bereiken als er
        iets aan de hand is met je bivakplaats of als iemand je daar nodig heeft. De
        volgende drie stappen zijn daarbij noodzakelijk.</p>
    <ul>
        <li><%=Html.ActionLink("Registreer je bivak", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" })%></li>
        <li><%=Html.ActionLink("Schrijf minstens de bivakverantwoordelijke in", "ViewTonen", new { controller = "Handleiding", helpBestand = "DeelnemersInschrijven" })%></li>
        <li><%= Html.ActionLink("Duid aan dat hij of zij de contactpersoon is", "ViewTonen", "Handleiding", null, null, "Contactpersoon", new { helpBestand = "DeelnemersInschrijven" }, null)%></li>
    </ul>
    <p>
        Wat kun je verder nog doen? Deze gegevens worden niet doorgestuurd naar het
        nationaal secretariaat, dit is alleen voor jullie groep zichtbaar.</p>
    <ul>
        <li><%=Html.ActionLink("Bijhouden wie ingeschreven is", "ViewTonen", new { controller = "Handleiding", helpBestand = "Deelnemersadministratie" })%>
        </li>
        <li><%=Html.ActionLink("Registreren wie al betaald heeft", "ViewTonen", new { controller = "Handleiding", helpBestand = "Deelnemersadministratie" })%>
        </li>
        <li><%=Html.ActionLink("Registreren van wie je de medische fiche al hebt", "ViewTonen", new { controller = "Handleiding", helpBestand = "Deelnemersadministratie" })%>
        </li>
    </ul>
    <%
        }%>
    <% 
        // Van augustus tot en met november tonen we bovenaan welke stappen er nodig zijn voor het nieuwe werkjaar
        if (DateTime.Now.Month > 7 && DateTime.Now.Month <= 11)
        { %>
    <h2>
        Tijd voor de jaarovergang!</h2>
    <p>
        De GAV's van jouw groep moeten de volgende zaken uitvoeren om je aansluiting
        in orde te brengen:</p>
    <ul>
        <li>Zorgen dat eventuele opvolgers een login krijgen: zie
            <%=Html.ActionLink("Logins beheren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "GavsBeheren" })%></li>
        <li>
            <%=Html.ActionLink("De jaarovergang uitvoeren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "JaarovergangUitvoeren" })%>:
            je afdelingen instellen, en leden en leiding inschrijven die vorig jaar al aangesloten
            was</li>
        <li>Controleren of je leden van vorig werkjaar in de juiste afdeling terechtgekomen
            zijn</li>
        <li>De leiding bij de juiste afdeling zetten: belangrijk voor afdelingsgebonden post!</li>
        <li>Nieuwe leden
            <%=Html.ActionLink("toevoegen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwePersoon" })%>
            en
            <%=Html.ActionLink("inschrijven", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwLid" })%></li>
    </ul>
    <%
        }%>
    <h2>
        De online handleiding gebruiken</h2>
    <p>
        Als er zaken zijn die je zo snel mogelijk in orde moet brengen, dan krijg je
        daar onderaan rechts op je scherm een kadertje met een waarschuwing voor.</p>
    <p>
        Als je in de handleiding een link ziet staan die uit één of meerdere woorden
        bestaat, dan verwijst die naar een ander onderdeel van de handleiding of naar
        een externe website. Als er <%=Html.InfoLink("Voorbeeld") %> staat, dan krijg je meer informatie 
        over dat trefwoord door op het icoontje te klikken.</p>
    <p>
        Vragen of feedback? <a href="http://www.chiro.be/eloket/feedback-gap">Laat maar
            komen!</a></p>
    <p>
        Je kunt discussiëren over hoe handig het systeem wel of niet is, en je kunt
        elkaar helpen om het te leren gebruiken: op het <a href="https://develop.chiro.be:3000/projects/gap/boards/1">
            GAP-forum</a>.</p>
    <p>
        Of zit je heel erg in de knoop en heb je dringend hulp nodig? Dan kun je altijd
        nog bellen naar het nationaal secretariaat: 03-231&nbsp;07&nbsp;95.</p>
</asp:Content>
