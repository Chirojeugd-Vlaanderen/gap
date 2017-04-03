<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <%
/*
* Copyright 2008-2017 the GAP developers. See the NOTICE file at the 
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
	Handleiding: Mailadrescontrole
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <div id="mailadrescontrole">
        <h2>Mailadrescontrole</h2>
        <p>In eerste instantie gaat GAP na of de ingevulde waarde eruitziet als een geldig mailadres:</p>
        <ul>
            <li>Enkel cijfers, letters en bepaalde leestekens (punt, koppelteken, underscore, @)</li>
            <li>Achter de @ iets dat eruitziet als een domeinnaam (provider.be)</li>
        </ul>
        <p>De markering dat het om een verdacht adres gaat, komt er na een tweede, meer uitgebreide controle. Daarvoor overlopen we een aantal criteria, waar we punten aan toekennen.</p>
        <p>Minpunten:</p>
        <ul>
            <li>De domeinnaam bevat veelvoorkomende typfouten (hotmail.ocm, gmail.be in plaats van gmail.com, enz.)</li>
            <li>Het adres bevat zaken als 'ikweethetniet', 'geenmailadres@chiro.be', enz.</li>
            <li>Het gaat om iemand van ketileeftijd maar het voorkeursadres is nog gezinsgebonden</li>
            <li>Het adres verwijst naar een functie binnen de groep (bv. groepsleiding@chirohuppeldepup.be)</li>
            <li>Het lijkt op een werkadres want er komt bvba of vzw in voor</li>
        </ul>
        <p>Pluspunten:</p>
        <ul>
            <li>Het adres bevat de voornaam van de persoon, of iets dat erop lijkt (bv. jerre in plaats van Jeroen)</li>
            <li>Het adres bevat het geboortejaar van de persoon</li>
            <li>Er komt 'chiro' of een andere Chiroterm in voor</li>
        </ul>
    </div>
</asp:Content>
