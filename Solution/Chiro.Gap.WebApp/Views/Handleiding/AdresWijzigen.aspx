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
	Handleiding: Adres wijzigen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <div id="verhuizen">
	<h2>Een adres wijzigen</h2>
	<p>Zit je met een adreswijziging, dan kun je in principe het oude adres verwijderen
		en een nieuw toevoegen. Het programma kijkt na of er nog andere mensen op dat
		adres wonen, en je krijgt de mogelijkheid om ook voor hen dat oude adres te
		verwijderen. Dat betekent dan wel dat je voor verschillende mensen een nieuw
		adres moet toevoegen.</p>
	<p>Eenvoudiger is natuurlijk om het bestaande adres aan te passen.</p>
		
	<p>Stappen in de procedure:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
		<li>Klik daar op de naam van iemand waarvan je het adres wil wijzigen. Je komt dan op de persoonsfiche.</li>
        <li>Klik daar op het <strong>'pen'-icoon</strong> achter het adres dat je wenst aan te passen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/AdresWijzigen_pen.png") %>" alt="De persoonlijkegegevensfiche" />
    <ul>
        <li>Er verschijnt een venster waarin de gegevens aangepast kunnen worden.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/AdresWijzigen_Venster.png") %>" alt="De persoonlijkegegevensfiche" />
    <ul>
        <li>Klik op 'Wijzig adres' om de nieuwe gegevens op te slaan.</li>
    </ul>
	<p>
		<strong>Opgelet:</strong> het programma controleert of je een geldige combinatie
		van straat en gemeente invult. Is dat niet het geval, dan heb je twee mogelijkheden.
		Ofwel zit de straat anders in onze databank, ofwel zit ze er niet in. We gebruiken
		alleen de officiële schrijfwijze. Vraag bij de inschrijvingen dus dat het adres
		correct ingevuld wordt. Het is bijvoorbeeld mogelijk dat de Hendrickxstraat
		die iedereen kent officieel Burgemeester Hendrickxstraat heet. Zit je met een
		straatnaam die je echt niet terugvindt, dan kun je
		<%= Html.ActionLink("vragen aan het nationaal secretariaat dat ze de straatnamenlijst aanvullen", 
			"ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweStraatnaam" })%>.</p>
            </div>
</asp:Content>
