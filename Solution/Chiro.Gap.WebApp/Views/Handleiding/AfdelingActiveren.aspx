<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="ViewPage<HandleidingModel>" %>

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
	Handleiding: Afdeling activeren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een afdeling activeren</h2>
	<p>
		Elke afdeling die je ooit gebruikt, blijft bestaan voor jouw groep. Je kunt
		ze dus terugvinden, en je kunt in voorbije werkjaren nagaan wie er allemaal
		lid van was. Zo kan het zijn dat je één jaar een gemengde rakwi-afdeling had,
		maar het jaar erop twee niet-gemengde afdelingen voor diezelfde leeftijd: rakkers
		en kwiks.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Groep'</li>
		<li>Klik op de link 'afdelingsverdeling aanpassen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_afdelingsverdeling_aanpassen.png") %>"
		alt="De link om de afdelingsverdeling aan te passen, vind je op het tabblad 'Groep'" />
	<ul>
		<li>Je krijgt nu een overzichtje van de afdelingen die je dit werkjaar hebt, en
			onderaan van de afdelingen die je ooit toevoegde maar die je nog niet activeerde
			dit werkjaar.</li></ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Overzicht_afdelingen.png") %>"
		alt="De afdelingsverdeling" />
	<ul>
		<li>Klik naast zo'n niet-actieve afdeling op de link 'Activeren in huidig werkjaar'.
			Je gaat dan naar een formuliertje waar je bijkomende gegevens moet invullen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Afdeling_activeren.png") %>" alt="Een afdeling activeren" />
	<ul>
		<li>Onderaan vind je de 'offici&euml;le' afdelingen en de geboortejaren die daarmee
			overeenkomen. Je hoeft je daar niet aan te houden, ze staan er gewoon als hulp.
			Eén van de keuzes die je moet maken, is wel met welke offici&euml;le afdeling
			de jouwe overeenkomt. Zo weet het nationaal secretariaat welke ledenuitgave
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Ledenuitgave", new { helpBestand = "Trefwoorden" }, new { title="Wat is een ledenuitgave?" }) %>
			we moeten opsturen voor die leden.
			<%=Html.ActionLink("Meer uitleg nodig over speciale afdelingen?", "ViewTonen", "Handleiding", new { title = "Hoe moet je speciale afdelingen koppelen aan officiële?" })%></li>
		<li>Wanneer je de nodige gegevens ingevuld hebt, klik je op de knop 'Bewaren'.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je de nodige aanpassingen nog kunt doen. Het kan bijvoorbeeld zijn dat je geboortejaren
					invulde die niet overeenkomen met die van de corresponderende offici&euml;le
					afdeling. Je mag bijvoorbeeld wel drie jaar speelclub hebben, maar minstens
					één van die drie geboortejaren moet bij de offici&euml;le speelclub staan. Of
					misschien hield je geen rekening met <a href="http://www.chiro.be/minimumleeftijd"
						title="Uitleg over de minimumleeftijd voor Chiroleden">de minimumleeftijd</a>:
					de Chiro sluit geen kleuters aan.</li>
				<li class="goed">Als er geen problemen meer zijn, keer je automatisch terug naar
					het overzicht van je afdelingen. De geactiveerde afdeling staat nu ook bovenaan.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
