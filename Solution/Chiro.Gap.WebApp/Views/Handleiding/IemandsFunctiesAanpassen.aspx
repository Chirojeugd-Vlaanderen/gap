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
	Handleiding: Iemands functies aanpassen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iemands functies aanpassen</h2>
	<p>
		Je kunt alleen functies toekennen aan ingeschreven leden en leiding. Op het
		tabblad 'Ingeschreven' zie je in de kolom 'Func.' de afkortingen van de functies
		die iemand heeft.</p>
	<p>
		Je kunt iemand een functie toekennen of ze weer afnemen. Als je iemand
		<%=Html.ActionLink("uitschrijft", "ViewTonen", new { controller = "Handleiding", helpBestand = "Uitschrijven" })%>,
		worden de toegekende functies automatisch afgenomen. Bij de jaarovergang worden
		<em>alle</em> functies afgenomen.</p>
	<p>
		Er zijn twee functies die elke groep <em>moet</em> invullen:</p>
	<ul>
		<li>Contactpersoon: dat is degene die de post zal ontvangen van het nationaal secretariaat.</li>
		<li>Financieel verantwoordelijke: dat is degene naar wie de facturen opgestuurd worden</li>
	</ul>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
		<li>Klik daar op de naam van degene voor wie je de functies wilt aanpassen. Je komt
			dan op de persoonsfiche.</li>
		<li>Klik links onderaan, bij de lidgegevens op de link 'functies aanpassen'.</li>
		<li>Je komt nu op het formulier met lidgegevens van de persoon in kwestie. Je vindt
			er onder andere een lijstje met alle mogelijke functies: zowel de officiële
			als degene die jouw groep zelf toevoegde. Vink aan welke functies de persoon
			moet krijgen, en vink wanneer nodig de andere af.</li>
		<li>Klik om af te ronden op de knop 'Gegevens wijzigen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Inschrijvingsgegevens.png") %>"
		alt="Inschrijvingsgegevens aanpassen" />
</asp:Content>
