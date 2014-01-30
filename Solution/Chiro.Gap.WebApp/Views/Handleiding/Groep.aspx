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
	Handleiding: Groep
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>Groep</h2>
    <h3>Wat zie je hier?</h3>
    <p>
        Op dit tabblad zie je alle groepsgebonden instellingen, te beginnen met de 'algemene groepsinfo': de naam en het stamnummer van je groep. De groepsnaam kun je hier aanpassen door op het potloodicoontje te klikken.
    </p>
    <p>Via het menu aan de linkerkant kun je de afdelingen bekijken die je voor dit werkjaar geactiveerd hebt, de categorieën die je aangemaakt hebt om personen en leden in te stoppen, en de functies voor leden en leiding.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Groepsinstellingen_overzicht.png") %>"
        alt="Wat is er te zien op het tabblad Groep?" />
    <h3>Wat kun je hier doen?</h3>
    <ul>
        <li>
            <%=Html.ActionLink("De afdelingsverdeling aanpassen", "ViewTonen", new { controller = "Handleiding", helpBestand = "AfdelingsverdelingAanpassen" })%></li>
        <li>
            <%=Html.ActionLink("Een nieuwe categorie toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweCategorie" })%></li>
        <li>
            <%=Html.ActionLink("Een nieuwe functie toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweFunctie" })%></li>
    </ul>
</asp:Content>
