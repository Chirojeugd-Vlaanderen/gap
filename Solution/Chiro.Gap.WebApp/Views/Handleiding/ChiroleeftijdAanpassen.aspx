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
	Handleiding: Iemands Chiroleeftijd aanpassen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iemands Chiroleeftijd aanpassen</h2>
	<p>
		Je Chiroleeftijd speelt een rol zolang je lid bent. Hij bepaalt met welk geboortejaar
		je meegaat. Een Chiroleeftijd van 0 betekent dat je gewoon bij je leeftijdsgenoten
		zit. -1 laat je meegaan met de leden die een jaar jonger zijn, 1 met degenen
		die een jaar ouder zijn.</p>
	<p>
		Een typisch geval waarbij je de Chiroleeftijd aanpast, is bij iemand die in
		de lagere school 'is blijven zitten'. Zo iemand zit meestal bij zijn of haar
		klasgenoten in de afdeling, in plaats van bij zijn of haar leeftijdsgenoten.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen' of 'Ingeschreven' op de naam van die persoon.
			Je krijgt dan de persoonsfiche te zien, met links onderaan de Chirogegevens.</li>
        <li>Klik in de rij van de Chiroleeftijd op het potloodicoontje. Je krijgt dan een dropdown 
            waarin je de aanpassing kunt kiezen. +1 betekent dat de persoon bij de automatische 
            afdelingsverdeling meegaat met de 'generatie' van een jaar ouder. Iemand die bijvoorbeeld 
            op het eind van het jaar verjaart of die is blijven zitten, krijgt -1 om met de 'generatie' 
            van een jaar jonger mee op te schuiven.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Chiroleeftijd_aanpassen.png") %>"
		alt="Chiroleeftijd aanpassen" />
</asp:Content>
