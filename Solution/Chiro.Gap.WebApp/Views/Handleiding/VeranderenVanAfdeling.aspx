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
	Handleiding: Veranderen van afdeling
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Veranderen van afdeling</h2>
	<p>
		Leiding kan bij meer dan één afdeling staan, leden kunnen er maar in één zitten.</p>
	<p>
		Stappen in de procedure:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven'.</li>
		<li>Klik daar op de naam van degene die je in een andere afdeling wilt stoppen.
			Je krijgt dan de persoonsfiche te zien, met links onderaan de lidgegevens. Klik
			daar bij Afdelingen op de link 'aanpassen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Afdelingen_aanpassen.png") %>" 
		alt="Iemands afdeling aanpassen vanop de persoonsfiche" />
	<ul>
		<li>Nu krijg je een lijstje te zien met alle actieve afdelingen in het huidige werkjaar.
			Vink aan in welke afdeling de persoon zit, vink de andere zo nodig af, en klik op 'Bewaren'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Iemand_bij_afdeling_zetten.png") %>" alt="Iemands afdelingen aanpassen" />
	<p>
		<strong>Opgelet:</strong> leiding kun je bij eender welke afdeling zetten, maar
		leden zijn aan hun leeftijd gebonden. Gaat het over een lid dat niet mee moet
		gaan met zijn of haar leeftijdsgenoten, pas dan de Chiroleeftijd
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?" } ) %>
		aan.
	</p>
</asp:Content>
