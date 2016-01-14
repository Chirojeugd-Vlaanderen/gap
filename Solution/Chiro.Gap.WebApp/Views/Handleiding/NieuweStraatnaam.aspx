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
	Handleiding: Nieuwe straatnaam
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuwe straatnaam toevoegen</h2>
	<p>
		In onze databank zit een lijst met de officiële straatnamen van België. Bij
		elk nieuw adres dat toegevoegd wordt, controleert het programma of de straatnaam
		bestaat en of ze voorkomt in de gemeente die je invulde. Zo willen we een hoop
		fouten en misverstanden vermijden.</p>
	<p>
		Zit je nu met een nieuwe straat, die nog niet in onze lijst voorkomt, dan moet
		je vragen aan het nationaal secretariaat dat ze ze toevoegen. Daar wordt dan
		eerst nagekeken of het toch niet om een bestaande straat gaat, en of de straat
		echt bestaat. Pas dan voegen we de nieuwe straatnaam toe. Het kan dus wel een
		tijdje duren voor je ze effectief kunt gebruiken.</p>
	<p>
		Welke gegevens moet je zeker doorgeven?</p>
	<ul>
		<li>De <strong>volledige</strong> straatnaam, volledig <strong>juist gespeld</strong></li>
		<li>Het postnummer en de naam van de gemeente</li>
	</ul>
	<p>
		Stuur die gegevens door via het <a href="http://www.chiro.be/eloket/gap-straatnaam">
			e-loket</a>.</p>
	<p>
		Het secretariaat brengt je op de hoogte zodra ze je verzoek behandeld hebben.</p>
</asp:Content>
