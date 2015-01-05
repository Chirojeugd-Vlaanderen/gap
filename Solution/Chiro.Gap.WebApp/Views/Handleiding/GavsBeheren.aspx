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
    Handleiding: GAV's beheren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>GAV's beheren</h2>
    <p>Rechts bovenaan, onder je loginnaam, zie je onder andere een link 'GAV's <%=Html.InfoLink("GAVbeheer") %> beheren'.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Links_rechts_bovenaan.png") %>"
        alt="De GAV-info rechts bovenaan" />
    <p>Daarmee ga je naar een overzichtje van wie er op dit moment allemaal toegang
        heeft tot je gegevens.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Overzicht_gebruikersrechten.png") %>"
        alt="Overzicht GAV's" />
    <p>Hier kun je toegangsrechten verlengen of afnemen.</p>
    <p>
        Om iemand toegang te geven, moet je naar zijn of haar persoonlijkegegevensfiche
        gaan: klik op de naam op het tabblad Iedereen of Ingeschreven. Op die fiche
        zie je onder de communicatievormen de 'GAV-info' van die persoon.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Gebruikersrecht_toekennen.png") %>"
        alt="De link waarmee je gebruikersrechten toekent" />
    <p>
        Klik je op die link, dan krijgt de persoon in kwestie een mailtje met zijn of
        haar login en wachtwoord. Het mailadres van die persoon moet dus ingevuld zijn,
        anders lukt dat niet.</p>
    <p>
        Bij de mensen met een login zie je op de persoonlijkegegevensfiche dezelfde
        info als in het overzichtje: de gebruikersnaam en de vervaldatum. Je kunt hier
        ook gebruikersrechten afnemen of de toegangsperiode verlengen.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Toegekend_gebruikersrecht.png") %>"
        alt="Info over toegekende gebruikersrechten" />
</asp:Content>
