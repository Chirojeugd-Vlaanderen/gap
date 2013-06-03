<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
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
	Handleiding: Jaarovergang
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>Jaarovergang</h2>
	<h3>Wat zie je hier?</h3>
	<p>
		Het tabblad voor de jaarovergang wordt zichtbaar in de periode dat het nationaal
		secretariaat aan het nieuwe werkjaar begint, dus rond 1 september. Het blijft
		zichtbaar tot een GAV <%=Html.InfoLink("GAV") %>
		je groep het proces doorlopen heeft. Dat moet <b>voor 15 oktober</b> gebeuren.
		Neem daar wel je tijd voor! Er moet op dat moment veel gebeuren, en het is het
		beste dat je het allemaal ineens doet - anders kun je een factuur<%=Html.InfoLink("FACTUUR") %>
		krijgen voor de aansluiting&nbsp;<%=Html.InfoLink("AANSLUITING") %> van mensen die je eigenlijk niet wilt inschrijven.
    </p>
	<h3>Wat kun je hier doen?</h3>
	<ul>
		<li><%=Html.ActionLink("De jaarovergang uitvoeren", "ViewTonen", new { controller = "Handleiding", helpBestand = "JaarovergangUitvoeren" })%></li>
	</ul>
</asp:Content>
