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
	Handleiding: Nieuwe categorie
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuwe categorie toevoegen</h2>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Groep'</li>
		<li>Klik op de link 'categorieën toevoegen/verwijderen'</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Groepscategorieen.png") %>"
		alt="De categorieënlijst aanpssen op de groepsfiche" />
	<ul>
		<li>Je komt nu op het formulier met de categorieën van je groep. Bovenaan zie je
			degene die je al toevoegde, onderaan kun je er nog nieuwe bijmaken.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Groepsinstellingen_categorieen.png") %>"
		alt="Formulier voor aanpassingen aan categorieën van de groep" />
	<ul>
		<li>Vul de naam en een afkorting (code) in, en klik op de knop 'Bewaren'
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je nog zaken kunt aanpassen. Het kan bv. zijn dat er al een categorie bestaat
					met die naam of die afkorting.</li>
				<li class="goed">Als er geen problemen meer zijn, staat je nieuwe categorie bovenaan
					in het lijstje en kun je ze voortaan gebruiken.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
