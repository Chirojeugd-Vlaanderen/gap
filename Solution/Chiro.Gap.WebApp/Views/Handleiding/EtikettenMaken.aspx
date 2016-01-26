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
	Handleiding: Etiketten maken
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Etiketten maken</h2>
	<p>
		Je kunt adresetiketten afdrukken met de gegevens uit het GAP. Dat kan niet rechtstreeks
		vanuit het GAP: je moet eerst een
		<%=Html.ActionLink("lijst downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" })%>
		en dan een tekstverwerkingsprogramma als Word van Microsoft of Writer van LibreOffice
		gebruiken. Het gaat waarschijnlijk ook met andere programma's, maar de meeste
		mensen hebben Word of Writer op hun computer. Writer kun je overigens gratis
		<a href="http://www.libreoffice.org/download/" target="new">downloaden</a>.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Download de lijst die je nodig hebt: een ledenlijst, een lijst van personen
			in een bepaalde categorie, enz.</li>
		<li>Open je tekstverwerkingsprogramma en doorloop daar de procedure voor 'afdruk
			samenvoegen'.
			<ul>
				<li><%=Html.ActionLink("Etiketten maken in Microsoft Word 2003", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenInWord2003" })%></li>
				<li><%=Html.ActionLink("Etiketten maken in Microsoft Word 2007 of 2010", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenInWordMetRibbon" })%></li>
				<li><%=Html.ActionLink("Etiketten maken in LibreOffice Writer 3.2", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenInWriter" })%></li>
			</ul>
		</li>
	</ul>
</asp:Content>
