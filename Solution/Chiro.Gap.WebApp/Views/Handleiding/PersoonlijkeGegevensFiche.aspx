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
	Handleiding: Gegevens aanpassen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>Persoonlijke gegevens</h2>
    <h3>Hoe kom je hier?</h3>
	<p>Je komt hier terecht wanneer je in het personenoverzicht (tabblad 'Iedereen')
		of in het ledenoverzicht (tabblad 'Ingeschreven') op een naam klikt.</p>
    <h3>Wat zie je hier?</h3>
	<p>Op deze pagina krijg je een overzicht van alle gegevens van een persoon. Zowel zijn of haar persoonlijke- en contactgegevens als gegevens
    specifiek over Chiro.</p>
    <h3>Wat kan je hier doen?</h3>
	<p>
	    Alle persoonlijke- en Chirogegevens van een persoon kunnen via zijn of haar persoonsfiche aangepast worden. 
        Klik op het <strong>'pen'-icoon</strong> achter het gegeven in de tabel dat je wil aanpassen. Je krijgt nu ofwel een tekstveld om het geven in te vullen, ofwel een
        pop-up waarin de juiste gegevens ingegeven kunnen worden. Gegevens waarachter geen 'pen'-icoon staat (bv. AD-nummer) kunnen niet aangepast worden.
    </p>
    <p>Je kan van hieruit ook iemand leiding maken, een functie toekennen of zijn of haar Chiroleeftijd aanpassen en,
     als dat nog niet gebeurd is, de persoon inschrijven.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/GegevensAanpassen_Persoonsfiche.png") %>" alt="De persoonlijkegegevensfiche" />

	
</asp:Content>
