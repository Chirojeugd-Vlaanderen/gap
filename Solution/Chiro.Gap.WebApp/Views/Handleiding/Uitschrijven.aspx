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
	Handleiding: Uitschrijven
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iemand uitschrijven</h2>
	<p>
		Iemand uitschrijven is een drastische maatregel. In principe kan het ook niet
		- toch niet voor het nationaal secretariaat. Wanneer je iemand aansluit, is
		dat voor een heel werkjaar.</p>
	<p>
		In je groep kun je wel de beslissing nemen om iemand uit te schrijven. De persoon
		blijft wel in je gegevensbestand zitten, maar hij of zij verschijnt niet meer
		bij de ingeschreven leden. Van mensen die automatisch ingeschreven werden bij
		de jaarovergang en die geen lid meer zijn, kun je zo vermijden dat je toch nog
		voor hun aansluiting betaalt.
	</p>
	<p>
		<em>Terzijde: het administratieprogramma waarschuwt je - o.a. via mail - wanneer
			de probeerperiode van je leden verloopt, zodat je nog degenen kunt uitschrijven
			die geen lid willen worden of van wie je het nog niet zeker bent. Als je na
			de probeerperiode ontdekt dat je toch leden foutief aangesloten hebt, dan kunnen
			we dat nog rechtzetten, maar dat kost wel wat werk. Je krijgt dan wel hun inschrijvingsgeld
			terug, maar min 15 euro administratiekosten.</em>
	</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven'. Vink aan wie je wilt uitschrijven, en kies
			in het selectievakje onder 'Selectie' voor 'Uitschrijven.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Uitschrijven.png") %>" alt="Iemand uitschrijven" />
	<ul>
		<li>Je krijgt nog een melding of de leden in kwestie uitgeschreven zijn.</li>
	</ul>
	<p>
		Foutje gemaakt? De verkeerde persoon uitgeschreven? Geen probleem: zoek hem
		of haar op het tabblad 'Iedereen' en klik weer op 'Inschrijven'. Je zult niet
		opnieuw een factuur krijgen voor de aansluiting.</p>
</asp:Content>
