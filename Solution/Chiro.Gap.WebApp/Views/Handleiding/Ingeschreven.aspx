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
	Handleiding: Ingeschreven
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>Ingeschreven</h2>
	<h3>Wat zie je hier?</h3>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Werkbalk_Ingeschreven.png") %>"alt="Het tabblad 'Ingeschreven'" />
	<p>
		Op dit tabblad vind je een overzicht van al wie ingeschreven is, per werkjaar.
		Klik je op iemands naam, dan kun je zijn of haar persoonsgegevens aanpassen.
		Andere links hebben met lidgegevens te maken. Die kun je alleen aanpassen in
		het huidige werkjaar. Gegevens over het verleden staan vast, daar kun je niets
		meer aan veranderen.
    </p>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Tabblad_Ingeschreven.png") %>"
		alt="Het tabblad 'Ingeschreven'" />
    <p>Je ledenlijst wordt doorgegeven aan Chirojeugd Vlaanderen, voor de aansluiting&nbsp;<%= Html.InfoLink("ING_AANSLUITING") %>
    en de verzekering. Dat gebeurt per lid, na het einde van hun instapperiode&nbsp;<%= Html.InfoLink("ING_INSTAP")%>
    <h3>Wat kun je hier doen?</h3>
	<ul>
		<li><%= Html.ActionLink("Leiding verzekeren tegen loonverlies", "ViewTonen", new { controller = "Handleiding", helpBestand = "VerzekeringLoonverlies" }, new { title = "Werkende leiding extra verzekeren"})%></li>
		<li><%= Html.ActionLink("Iemand bij een (andere) afdeling zetten", "ViewTonen", new { controller = "Handleiding", helpBestand = "VeranderenVanAfdeling" }, new { title = "Leden in of leiding bij een andere afdeling zetten" })%></li>
		<li><%= Html.ActionLink("Filteren op afdeling of functie", "ViewTonen", new { controller = "Handleiding", helpBestand = "Filteren" }, new { title = "De ledenlijst filteren op afdeling of functie" })%></li>
		<li><%=Html.ActionLink("Een lijst downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" }, new { title = "Gegevens downloaden als Excel-bestand" })%>
			(bv. om
			<%=Html.ActionLink("etiketten te maken", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenMaken" }, new { title = "Etiketten maken met gegevens uit een Excel-bestand" })%>)</li>
	</ul>
</asp:Content>
