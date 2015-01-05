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
	Handleiding: Verzekering loonverlies
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	
    <h2>Verzekering loonverlies</h2>
    <span id="kort">
	<p>
		Aangesloten leden en leiding zijn verzekerd voor burgelijke aansprakelijkheid,
		dood en invaliditeit. Meer uitleg over de Chiroverzekering vind je op <a href="http://www.chiro.be/verzekeringen"
			title="Uitleg over de Chiroverzekering">www.chiro.be/verzekeringen</a>.</p>
	<p>
		Leiding die al gaat werken, kun je bijkomend verzekeren voor loonverlies. Die
		optie vult voor één jaar het bedrag aan dat de ziekteverzekering uitbetaalt
		als je door een ongeval niet meer kunt gaan werken.</p> </span>
	<p>
	   
		Stappen in het proces:
	</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' op de naam van de leider of leidster die
			je bijkomend wilt verzekeren.</li>
            <img src="<%=ResolveUrl("~/Content/Screenshots/Werkbalk_Ingeschreven.png") %>" alt="Werlkbalk Ingeschreven" />
		<li>Je komt nu op de persoonsfiche. Klik links onderaan, bij de lidgegevens, op
			de link 'Verzeker'.</li>
            <img src="<%=ResolveUrl("~/Content/Screenshots/Verzekeren_loonverlies.png") %>" alt="Iemand verzekeren voor loonverlies vanop de persoonsfiche" />
    </ul>
	
	<ul>
		<li>Dan krijg je eerst nog een vraag om bevestiging. Klik op de knop 'Bevestigen'
			om af te ronden.</li>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Verzekeren_loonverlies_bevestiging.png") %>" alt="Iemand verzekeren voor loonverlies vanop de persoonsfiche" />
    </ul>
</asp:Content>
