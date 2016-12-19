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
	Handleiding: Nieuwe functie
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuwe functie toevoegen</h2>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Groep'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Functies_aanpassen.png") %>"
		alt="Een nieuwe functie toevoegen vanop de groepsfiche" />
	<ul>
		<li>Klik op de link 'functies toevoegen/verwijderen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Functie_toevoegen.png") %>" alt="Een nieuwe functie toevoegen" />
	<ul>
		<li>Vul de naam en een afkorting (code) in. Geef op hoeveel mensen in je groep die
			functie mogen hebben (maximum), hoeveel er ze moeten hebben (minimum). Het maximumaantal
			mag normaal gezien leeg blijven. Vul niet 0 in, want dan mag niemand de functie
			hebben. Het jaartal mag normaal gezien ook leeg blijven. Vul het bijvoorbeeld in
			als je ledenlijsten van voorbije werkjaren wilt filteren op functie en je wilt
			de nieuwe functie daar niet bij zien staan voor ze ingevoerd werd. Ten slotte
			moet je nog aanduiden of alleen leiding de functie kan hebben, of alleen leden,
			of allebei.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je nog zaken kunt aanpassen. Het kan bv. zijn dat er al een functie bestaat
					met die naam of die afkorting.</li>
				<li class="goed">Als er geen problemen meer zijn, staat je nieuwe functie bovenaan
					in het lijstje en kun je ze voortaan gebruiken.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
