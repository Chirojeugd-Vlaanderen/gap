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
	Handleiding: Persoonlijkegegevensfiche
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Persoonlijkegegevensfiche</h2>
	<!-- EditGegevens -->
	<h3>
		Hoe kom je hier?</h3>
	<p>
		Je komt hier als je op de tab 'Iedereen' klikt op de link 'Nieuwe persoon',
		of als je een naam aanklikt en dan op de persoonsfiche klikt op de link 'persoonlijke
		gegevens aanpassen'.</p>
		
<img src="<%=ResolveUrl("~/Content/Screenshots/Persoonlijkegegevensfiche.png") %>" alt="De persoonlijkegegevensfiche" />
	<h3>
		Wat zie je hier?</h3>
	<p>
		Hier vind je alle 'identificerende' gegevens van een persoon: eventueel een
		AD-nummer
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "AD-nummer", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een AD-nummer?" })%>,
		voornaam en naam, geboortedatum en Chiroleeftijd
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?"})%>.</p>
	<h3>
		Wat kun je hier doen?</h3>
	<p>
		Het AD-nummer kun je niet veranderen, dat wordt toegekend door het nationaal
		secretariaat. Alle andere gegevens kun je wél aanpassen.</p>
</asp:Content>
