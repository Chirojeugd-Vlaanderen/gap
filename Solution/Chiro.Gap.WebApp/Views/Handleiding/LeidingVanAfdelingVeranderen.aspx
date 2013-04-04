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
    Handleiding: Leiding van afdeling veranderen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Leiding van afdeling veranderen</h2>
    <p>
        Het is belangrijk dat je bij de leiding de afdeling invult. Anders weet het
        nationaal secretariaat niet naar wie we de afdelingsspelen moeten opsturen:
        we willen die namelijk ineens bij de juiste afdeling laten terechtkomen.</p>
    <p>
        Heb je je leiding al ingeschreven maar ben je de afdelingen vergeten of heb
        je een foutje gemaakt? Ga naar het tabblad Ingeschreven. Vink daar alle leiding
        aan van dezelfde afdeling (voor wie dat nog niet ingevuld is). Kies onder 'Selectie'
        (boven of rechts van de tabel) voor 'Afdeling aanpassen'. Op een andere pagina
        kun je voor alle geselecteerde personen dan aanvinken bij welke afdeling ze
        staan.</p>
        <img src="<%=ResolveUrl("~/Content/Screenshots/Mensen_van_afdeling_veranderen.png.png") %>"
        alt="Mensen in een andere afdeling stoppen" />
    <p>
        Je wijzigingen worden normaal gezien binnen de 24 uur gesynchroniseerd met de
        gegevens van het nationaal secretariaat, daar moet je niets extra's voor doen.</p>
</asp:Content>
