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
	Handleiding: Colofon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Colofon</h2>
	<p>
		Heel wat mensen hebben ertoe bijgedragen dat deze website tot stand kwam.</p>
	<ul>
		<li>De <strong>werkgroep GAP</strong>: Johan Vervloet (hoofdprogrammeur en projectleider),
			Bart Boone, Peter Bertels, Broes Decat, Tim Mallezie, Koen Meersman, Tom Haepers,
            Mathias Keustermans, Sven Maes, Mattias Deparcq, Steven Lemmens</li>
		<li>De <strong>testers</strong>: Merijn Gouweloose, Maarten Perpet, Roel Vercammen, Ben Bridts en heel wat
			anderen</li>
		<li>En verder: alle mensen die ooit feedback gaven over het vroegere Chirogroepprogramma,
			de medewerk(st)ers van het nationaal secretariaat</li>
	</ul>
	<p>
		De bedoeling van deze website is dat het een handig instrument is voor lokale
		leiding. Heb je opmerkingen over hoe bepaalde zaken nu uitgewerkt zijn, of heb
		je suggesties voor verbeteringen of uitbreiding? <a href="http://www.chiro.be/eloket/feedback-gap"
			title="Stel vragen, of geef opmerkingen en suggesties door">Laat het dan zeker
			weten!</a></p>
</asp:Content>
