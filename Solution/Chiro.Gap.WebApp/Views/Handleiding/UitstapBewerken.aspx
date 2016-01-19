<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
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
	<h2>
		Uitstap/bivak bewerken</h2>
	<h3>
		Stappen in het proces:</h3>
	<ul>
		<li>Ga naar het tabblad 'Uitstappen/bivak'.</li>
		<li>Klik in het overzichtje op de naam die je aan die uitstap of het bivak gegeven
			hebt. Zo kom je op de detailfiche.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Fiche_bivakdetails_met_leden.png") %>"
		alt="Details van de uitstap" />
	<ul>
		<li>Je hebt een link om de data aan te passen en één om een bivakplaats toe te voegen
			of te bewerken (al naargelang).</li>
		<li>Om deelnemers uit te schrijven, klik je naast hun naam op de link 'uitschrijven'.</li>
	</ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
	Handleiding: Uitstap/bivak bewerken
</asp:Content>
