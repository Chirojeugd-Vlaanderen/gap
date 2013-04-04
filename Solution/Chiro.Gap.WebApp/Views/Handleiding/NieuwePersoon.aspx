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
	Handleiding: Nieuwe persoon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuwe persoon toevoegen</h2>
	<p>
		Zit er al een broer of een zus van die persoon in je gegevens, dan kun je jezelf
		werk besparen: dankzij de procedure
		<%=Html.ActionLink("Zus/broer maken", "ViewTonen", new { controller = "Handleiding", helpBestand = "ZusBroer" })%>
		kun je de adres- en andere gezinsgebonden info kopiëren. Dan moet je alleen
		nog de voornaam, de geboortedatum en het geslacht invullen. Gaat het echt over
		iemand die nieuw is? Volg dan de stappen hieronder.
	</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Persoon toevoegen'.</li>
		<li>Je komt nu op een formuliertje om een nieuwe persoon toe te voegen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Nieuwe_persoon.png") %>" alt="Formuliertje voor een nieuwe persoon" />
	<ul>
		<li>Vul de gevraagde gegevens in en klik op de knop 'Bewaren'
			<ul>
				<li>Als er iets foutliep, krijg je daar een foutmelding voor zodat je nog zaken
					kunt aanpassen. Het kan bv. zijn dat je iemand dubbel probeert toe te voegen
					of dat je te weinig invulde.</li>
				<li>Als er geen problemen meer zijn, kom je op een ander scherm, waar je bijkomende
					informatie kunt toevoegen.</li>
			</ul>
		</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_nieuwe_persoon.png") %>"
		alt="Bijkomende gegevens voor een nieuwe persoon" />
	<ul>
		<li>Bijkomende gegevens die je kunt toevoegen:
			<ul>
				<li>Adres(sen)</li>
				<li>Communicatievormen: telefoonnummer(s), mailadres(sen), enz.</li>
				<li>Categorieën</li>
			</ul>
		</li>
	</ul>
	<p>
		Voeg zeker nog een adres toe als die persoon lid moet worden van je groep. Vanaf dan kun je hem of haar inschrijven.</p>
</asp:Content>
