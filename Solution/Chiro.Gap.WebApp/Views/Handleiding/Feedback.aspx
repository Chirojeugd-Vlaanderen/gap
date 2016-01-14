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
	Handleiding: Feedback over het programma
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Feedback geven</h2>
	<p>
		De bedoeling van deze website is dat het een handig instrument is voor lokale
		leiding. Heb je opmerkingen over hoe bepaalde zaken nu uitgewerkt zijn, of heb
		je suggesties voor verbeteringen of uitbreiding? <a href="http://www.chiro.be/eloket/feedback-gap"
			title="Stel vragen, of geef opmerkingen en suggesties door">Laat het dan zeker
			weten!</a></p>
	<p>
		Je kunt discussiëren over hoe handig het systeem wel of niet is, en je
		kunt elkaar helpen om het te leren gebruiken: op het <a href="http://forum.chiro.be/forum/144">
			Chiroforum</a>.</p>
	<p>
		En zit je heel erg in de knoop en heb je dringend hulp nodig? Dan kun je altijd
		nog bellen naar het nationaal secretariaat: 03-231&nbsp;07&nbsp;95.</p>
</asp:Content>
