<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LedenLinksModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * s
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<p class="validation-summary-errors">
		Er zijn nog leden of leid(st)ers die in dit werkjaar de functie hebben die je wilt verwijderen.
		Als je de functie verwijdert, dan verliezen deze personen de functie voor dit werkjaar.
		Ben je er zeker van dat je de functie wilt verwijderen?
	</p>
	<% using (Html.BeginForm())
	{ %>
	<ul id="acties">
		<li>
			<input type="submit" value="Ja" /></li>
		<li>
			<%=Html.ActionLink("Nee", "Index")%></li>
	</ul>
	<h3>Personen met deze functie</h3>
	<%
		Html.RenderPartial("LedenLinksControl", Model); 
		// Ik denk dat die hidden hieronder niet nodig is,
		// dat kan even goed opnieuw uit de url gehaald worden.
	     %>
	<%=Html.HiddenFor(mdl => mdl.FunctieID) %>
	<%} %>
</asp:Content>
