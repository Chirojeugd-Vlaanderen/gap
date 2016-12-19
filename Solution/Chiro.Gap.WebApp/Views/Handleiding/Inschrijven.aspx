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
		Inschrijven</h2>
	<p>
		Iemand inschrijven kan op twee manieren:</p>
	<ul>
		<li>Automatisch bij de jaarovergang: iedereen die vorig werkjaar ingeschreven was,
			wordt automatisch opnieuw ingeschreven. Ze krijgen allemaal een instapperiode,
			zodat je nog tijd hebt om degenen uit te schrijven die niet meer komen. Kort
			voor het einde van die instapperiode krijgen de mensen met een login voor jouw
			groep een mailtje om daar even op te wijzen.</li>
		<li>In de loop van het jaar: je kunt het hele jaar door mensen toevoegen en inschrijven.</li>
	</ul>
	<p>
		Mensen die ingeschreven zijn en die niet meer in hun instapperiode zitten, zijn
		aangesloten bij Chirojeugd Vlaanderen. Daarvoor krijgt je financieel verantwoordelijke
		een factuur.</p>
		
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
	Handleiding: inschrijven
</asp:Content>
