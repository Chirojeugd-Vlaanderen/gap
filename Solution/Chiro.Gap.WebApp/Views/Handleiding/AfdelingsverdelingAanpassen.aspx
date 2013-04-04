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
	Handleiding: Afdelingsverdeling aanpassen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		De afdelingsverdeling aanpassen</h2>
	<p>
		De eerste - en normaal gezien enige - keer dat je iets moet aanpassen aan de
		afdelingsverdeling is bij de
		<%=Html.ActionLink("jaarovergang", "ViewTonen", new { controller = "Handleiding", helpBestand = "Jaarovergang" })%>:
		je moet elk jaar aanduiden welke afdelingen je in het nieuwe werkjaar hebt.
		Heb je daar een fout gemaakt of wil je in de loop van het jaar nog iets aanpassen?
		Volg dan de stappen hieronder.</p>
	<p>
		<a class="anchor" id="GesplitsteAfdelingen" />Speciaal geval: verschillende
		groepen hebben meerdere afdelingen voor dezelfde leeftijd. Een gemengde groep
		met niet-gemengde afdelingen heeft bijvoorbeeld voor elke leeftijd twee afdelingen:
		één voor de jongens en één voor de meisjes. <span class="Vroeger">In het Chirogroepprogramma
			moest je zowel de rakkers als de kwiks in dezelfde rakwi-afdeling stoppen. Of
			je moest zogezegd een jongens- en een meisjesgroep hebben, elk met eigen afdelingen.
		</span>
	</p>
	<p>
		Mogelijke procedures:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Een nieuwe afdeling aanmaken", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweAfdeling" })%></li>
		<li>
			<%=Html.ActionLink("Een bestaande afdeling activeren", "ViewTonen", new { controller = "Handleiding", helpBestand = "AfdelingActiveren" })%></li>
	</ul>
</asp:Content>