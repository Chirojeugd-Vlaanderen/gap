<%@ Page Language="C#" Inherits="ViewPage<BevestigingsModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content runat="server" ID="Content" ContentPlaceHolderID="head">
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
</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
	<% using (Html.BeginForm())
	{%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bevestigen" /></li>
		<li>
			<%=Html.ActionLink("Annuleren", "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %></li>
	</ul>
	Je staat op het punt om
	<%=Html.ActionLink(Model.VolledigeNaam, "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %>
	te <a href="http://www.chiro.be/administratie/verzekeringen/extras-en-opties/loonverlies">verzekeren tegen loonverlies</a>.
    <%if (Model.GroepsNiveau.HasFlag(Niveau.KaderGroep))
      { %> (dit is gratis voor kaderleden)
    <% }else{ %>
    (Kostprijs: &euro;
    <%= Model.Prijs.ToString() %>) 
    <% } %>
    Klik op &lsquo;bevestigen&rsquo; om de verzekering af te sluiten.
	<%} %>
</asp:Content>
