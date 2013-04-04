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
    Handleiding: Uitstappen/bivak
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Deelnemersadministratie voor uitstappen/bivakken</h2>
    <p>
        Van de mensen die meegaan, kun je bijhouden of ze al betaald hebben en of hun medische
        fiche al binnen is. Je kunt ook een opmerking toevoegen. Die informatie wordt niet
        doorgegeven aan Chirojeugd Vlaanderen, ze is alleen voor jouw groep zichtbaar.</p>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Ga naar het tabblad 'Uitstappen/bivak' en klik op de juiste uitstap om de deelnemerslijst
            te bekijken.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Fiche_bivakdetails_met_leden.png") %>"
        alt="Detailsfiche van je uitstap, met deelnemerslijst" />
    <ul>
        <li>Klik in de deelnemerslijst op een naam om naar de deelnemersfiche te gaan. Daar
            kun je aanpassen of het inschrijvingsgeld betaald is en of de medische fiche binnen
            is.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Deelnemersinfo_bewerken.png") %>"
        alt="Deelnemersinfo bewerken" />
</asp:Content>
