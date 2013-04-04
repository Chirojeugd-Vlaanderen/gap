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
    Handleiding: Gegevens filteren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Gegevens in lijsten filteren</h2>
    <p>
        Er zijn twee plaatsen waar je lijsten te zien krijgt: op het tabblad 'Ingeschreven'
        en op het tabblad 'Iedereen'. Je kunt die gegevens filteren met het selectielijstje
        rechts ervan.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Filteren_op_categorie.png") %>" alt="Filteren op categorie" />
    <p>
        Op die pagina's staat ook een link waarmee je de gegevens kunt downloaden als Excel-bestand.
        Als je een filter gebruikte, staan alleen de mensen uit je selectie in dat bestand.
        Zo kun je dus een lijstje van een afdeling
        <%=Html.ActionLink("downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" })%>,
        of van de kookploeg, enz. Je ziet aan de titel boven de tabel welke filter er toegepast
        is.</p>
    <p>
        Afdelingen en functies kun je alleen toekennen aan ingeschreven leden en leiding,
        en alleen voor het huidige werkjaar. Categorieën
       &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Categorie", new { helpBestand = "Trefwoorden" }, null)%>
        kun je voor iedereen gebruiken, dus zowel voor leden en leiding als voor 'buitenstaanders'.
        Categorieën zijn niet werkjaargebonden, dus eens iemand erin zit, blijft dat zo
        tot iemand dat ongedaan maakt.</p>
    <a class="anchor" id="Afdeling" />
    <h3>
        Ingeschreven: filteren op afdeling</h3>
    <p>
        Klik op het tabblad 'Ingeschreven'. Rechts van de tabel zie je onder het titeltje
        'Filteren' een selectielijstje met de tekst 'Op afdeling'. Wanneer je een keuze
        maakt (als er een knop staat: en op de knop klikt), zie je de leden en de leiding
        in die afdeling, voor het gekozen werkjaar.
    </p>
    <a class="anchor" id="Functie" />
    <h3>
        Ingeschreven: filteren op functie</h3>
    <p>
        Klik op het tabblad 'Ingeschreven'. Rechts van de tabel zie je onder het titeltje
        'Filteren' een selectielijstje met de tekst 'Op functie'. Wanneer je een keuze maakt
        (als er een knop staat: en op de knop klikt), zie je de leden en de leiding met
        die functie, voor het gekozen werkjaar.
    </p>
    <a class="anchor" id="SpecialeLijst" />
    <h3>
        Ingeschreven: filteren op 'speciale lijst'</h3>
    <p>
        Klik op het tabblad 'Ingeschreven'. Rechts van de tabel zie je onder het titeltje
        'Filteren' een selectielijstje met de tekst 'speciale lijst'. Daarmee kun je de
        mensen in volgorde van verjaardag zetten, of een selectie maken die je administratie
        makkelijker maakt. Het is voor mensen in die lijsten dat je als GAV een mailtje kunt krijgen, of dat er een melding rechtsonder op je scherm staat omdat er iets nog niet in orde is.</p>
    <ul>
        <li>Leden en leiding in instapperiode</li>
        <li>Ingeschreven zonder adres</li>
        <li>Ingeschreven zonder telefoonnummer</li>
        <li>Leiding zonder e-mailadres</li>
    </ul>
    <a class="anchor" id="Categorie" />
    <h3>
        Iedereen: filteren op categorie</h3>
    <p>
        Klik op het tabblad 'Iedereen'. Rechts van de tabel zie je onder het titeltje 'Filteren'
        een selectielijstje met de tekst 'Op categorie'. Wanneer je een keuze maakt (als
        er een knop staat: en op de knop klikt), zie je alle personen in die categorie.
    </p>
</asp:Content>
