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
	Handleiding: Nieuw lid
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>Een nieuw lid of een nieuwe leid(st)er toevoegen</h2>
	<p>Voeg wanneer nodig eerst nog een nieuwe persoon toe</p>
	<p>Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen'.</li>
		<li>Klik op de naam van degene die je wilt inschrijven om de persoonsfiche te bekijken.</li>
		</ul>
		<img src="<%=ResolveUrl("~/Content/Screenshots/NieuwLid_PersoonsFiche.png") %>" alt="Iemand inschrijven van op de persoonsfiche" />
		<ul>
		<li>Klik onderaan op de link waarmee je de persoon kunt inschrijven.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je de nodige aanpassingen nog kunt doen. Het kan bijvoorbeeld zijn dat de geboortedatum
					niet ingevuld is.</li>
				<li class="goed">Als er geen problemen meer zijn, krijg je een melding dat de persoon
					ingeschreven is. Vanaf nu vind je hem of haar op het tabblad 'Ingeschreven'.</li>
			</ul>
		</li>
	</ul>
	<p>
		Er is nog een <strong>kortere manier</strong>:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen'.</li>
	</ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Werkbalk_Iedereen.png") %>" alt="Werkbalk'Iedereen'" />
	
	<ul>
		<li>Klik achter de naam van degene die je wilt inschrijven op de link 'inschrijven
			als lid' of 'inschrijven als leiding'
            <img src="<%=ResolveUrl("~/Content/Screenshots/NieuwLid_Iedereen.png") %>"
		alt="Iemand inschrijven op het tabblad 'Iedereen'" />
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je de nodige aanpassingen nog kunt doen. Normaal gezien moet je daarvoor op
					de persoonsfiche zijn. Klik daarvoor op de naam van die persoon.</li>
				<li class="goed">Als er geen problemen meer zijn, krijg je een melding dat de persoon
					ingeschreven is. Je ziet die status in de kolom 'Ingeschr.', de inschrijvingslink
					is verdwenen en vanaf nu vind je de persoon ook op het tabblad 'Ingeschreven'.</li>
			</ul>
		</li>
	</ul>
	<p>
		<a class="anchor" id="MeerdereMensen" />Je kunt ook <strong>meerdere mensen tegelijk</strong>
		inschrijven:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen'</li>
        <img src="<%=ResolveUrl("~/Content/Screenshots/Werkbalk_Iedereen.png") %>" alt="Werkbalk'Iedereen'" />
		</ul>
		<img src="<%=ResolveUrl("~/Content/Screenshots/NieuwLid_MeerdereLedenInschrijven.png") %>" alt="Meerdere mensen ineens inschrijven" />
		<ul>
		<li>Vink de personen aan die je wilt inschrijven en naast de tabel de
			actie 'inschrijven als lid'.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor. Heb
					je meerdere personen proberen in te schrijven, dan krijg je alleen een foutmelding
					voor de personen waar dat niet lukte. Bij hen moet je de persoonsfiche eerst
					bekijken en de nodige gegevens invullen (of aanpassen).</li>
				<li class="goed">Bij de personen waar er geen problemen waren, zie je nu in de tabel
					dat ze lid of leiding zijn, naargelang de keuze die je maakte.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
