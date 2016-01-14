<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
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
    <h2>
        Waarom moeten contactgegevens volledig ingevuld zijn?</h2>
    <p>
        De contactpersoon van je groep moeten we kennen om post te kunnen sturen, de
        financieel verantwoordelijke om facturen op te sturen. Uiteraard moeten hun
        contactgegevens daarom zo snel mogelijk en juist ingevuld zijn.</p>
    <p>
        Van de leiding hebben we het mailadres nodig voor de nieuwsbrief.
        Zo kunnen we jullie snel op de hoogte brengen van dringend nieuws. Gewest- en
        verbondsmedewerkers kunnen dat ook opvragen om makkelijker contact op te kunnen
        nemen. Postadressen hebben we bijvoorbeeld nodig om Dubbelpunt op te sturen
        naar de mensen met een abonnement, en om de afdelingsspelen bij de juiste mensen
        te kunnen bezorgen.</p>
    <p>
        Van keti's en aspi's hebben we het postadres nodig om Kramp op te sturen.</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Handleiding: Waarom contactgegevens vervolledigen?
</asp:Content>
