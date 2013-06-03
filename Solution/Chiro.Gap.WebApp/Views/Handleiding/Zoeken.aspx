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
    Handleiding: Zoeken in tabellen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
   <h2>Zoeken in de tabellen</h2>
   <p>Wanneer je gegevens zoekt in de tabellen op de pagina's 'Ingeschreven' en 'Iedereen', 
    zijn er een aantal mogelijkheden om de gegevens te filteren of te doorzoeken
   </p>
   <h3>De Zoekbalk</h3>
   <p>In de zoekbalk bovenaan de pagina kan je personen zoeken op hun naam, voornaam of geboortedatum.
   De gegevens in de tabel die voldoen aan je zoekopdracht blijven staan, de rest verdwijnt.</p>
   <img src="<%=ResolveUrl("~/Content/Screenshots/Zoeken_Zoekbalk.png") %>" alt="Zoekbalk" />
   <h3>Sorteren</h3>
   <p>De gegevens in de tabellen kunnen gesorteerd worden door op de hoofding van een bepaalde kolom te klikken.
   Eén keer klikken sorteert de tabel van laag naar hoog (voor datums) of van A naar Z, klik je een tweede keer dan wordt de sortering omgedraaid.
   De tabel kan gesorteerd worden op elke kolom waarboven de twee pijltjes te zien zijn.</p>
   <img src="<%=ResolveUrl("~/Content/Screenshots/Zoeken_Sortering.png") %>" alt="Sorteren" />
   <h3>Paginering</h3>
   <p>In de linkerbovenhoek van de tabel kan je de paginering aanpassen. Hier kan je selecteren hoeveel personen
   er per pagina weergegeven worden.</p>
   <img src="<%=ResolveUrl("~/Content/Screenshots/Zoeken_Paginering.png") %>" alt="Paginering" />                               
   <img src="<%=ResolveUrl("~/Content/Screenshots/Zoeken_Paginering2.png") %>" alt="Paginering" />   
   <p>Onderaan de tabel kan je dan tussen die verschillende pagina's navigeren.</p>
   <img src="<%=ResolveUrl("~/Content/Screenshots/Zoeken_Paginering3.png") %>" alt="Paginering" />   
</asp:Content>
