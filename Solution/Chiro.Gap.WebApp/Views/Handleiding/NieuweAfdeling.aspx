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
	Handleiding: Nieuwe afdeling
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>Een nieuwe afdeling toevoegen</h2>
	<p>Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Groep'</li>
		<li>Klik op de link 'afdelingsverdeling aanpassen'</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_afdelingsverdeling_aanpassen.png") %>"
		alt="De link om de afdelingsverdeling aan te passen, vind je op het tabblad 'Groep'" />
	<ul>
		<li>Klik op de link 'Nieuwe afdeling'. Je komt dan op het formuliertje.</li></ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Nieuwe_afdeling.png") %>" alt="Een nieuwe afdeling toevoegen" />
	<ul>
		<li>Vul de naam en een afkorting in, en klik op de knop 'Bewaren'
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je nog zaken kunt aanpassen. Het kan bv. zijn dat er al een afdeling met die
					naam of die afkorting bestaat.</li>
				<li class="goed">Als er geen problemen meer zijn, kom je op het scherm met de afdelingsverdeling.
					Je nieuwe afdeling staat onderaan bij de niet-actieve.</li>
			</ul>
		</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Overzicht_afdelingen.png") %>"
		alt="De afdelingsverdeling" />
	<ul>
		<li>Klik naast je nieuwe afdeling op de link 'Activeren in huidig werkjaar'. Je
			komt dan weer op een formuliertje.</li></ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Afdeling_activeren.png") %>" alt="Een afdeling activeren" />
	<ul>
		<li>Onderaan vind je de 'offici&euml;le' afdelingen en de geboortejaren die daarmee
			overeenkomen. Je hoeft je daar niet aan te houden, ze staan er gewoon als hulp.
			Eén van de keuzes die je moet maken, is wel met welke offici&euml;le afdeling
			de jouwe overeenkomt. Zo weet het nationaal secretariaat welke ledenuitgave<%=Html.InfoLink("LedenUitgave") %>
			we moeten opsturen voor die leden. Er is ook een optie 'Speciaal'. Dat is niet
			voor bv. een speelclubafdeling met andere leeftijdsgrenzen dan de officiële!
			Met speciale afdelingen wordt geen rekening gehouden bij de automatische indeling
			als je nieuwe leden inschrijft. Meer uitleg over speciale afdelingen vind je
			op de pagina
			<%=Html.ActionLink("Speciale afdelingen", "ViewTonen", new { controller = "Handleiding", helpBestand = "SpecialeAfdelingen" })%></li>
		<li>Wanneer je de nodige gegevens ingevuld hebt, klik je op de knop 'Bewaren'.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je nog zaken kunt aanpassen. Het kan bijvoorbeeld zijn dat je geboortejaren
					invulde die niet overeenkomen met die van de corresponderende offici&euml;le
					afdeling. Je mag bijvoorbeeld wel drie jaar speelclub hebben, maar minstens
					één van die drie geboortejaren moet bij de offici&euml;le speelclub staan. Of
					misschien hield je geen rekening met <a href="http://www.chiro.be/minimumleeftijd"
						title="Uitleg over de minimumleeftijd voor Chiroleden">de minimumleeftijd</a>:
					de Chiro sluit geen kleuters aan.</li>
				<li class="goed">Als er geen problemen meer zijn, kom je op het scherm met de afdelingsverdeling.
					Je nieuwe afdeling staat bovenaan bij de actieve.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
