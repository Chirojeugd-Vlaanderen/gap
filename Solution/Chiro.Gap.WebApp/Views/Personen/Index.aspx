<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersoonInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #713) %>
	
     <%//jQuery scripts (algemeen en Datatables) %>
    <script src="<%= ResolveUrl("~/Scripts/jquery.dataTables.min.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/datestringsort.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery-overzicht-iedereen.js")%>" type="text/javascript"></script>
    <%//CSS bestanden voor opmaak DataTables %>
    <link href="<%= ResolveUrl("~/Content/jquery.dataTables_themeroller.css")%>" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        $(document).ready(function () {
            $('#kiesCategorie').hide();
            $("#GekozenCategorieID").change(function () {
                $('#kiesCategorie').click();
            });

            $('#kiesActie').hide();
            $("#GekozenActie").change(function () {
                $('#kiesActie').click();
            });
        });
	</script>
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<p><em>Hou je muispijl boven een link in de tabel - zonder te klikken - voor iets meer uitleg over wat de link doet.</em></p>
	<%using (Html.BeginForm("List", "Personen"))
   { %>
   <% Html.RenderPartial("PersonenLijstControl", Model); %>
	<div id="acties">
		<h1>
			Acties</h1>
		<ul>
			<li>
				<strong><%= Html.ActionLink("Nieuwe persoon", "Nieuw", new{ title="Voeg een nieuwe persoon toe in je gegevensbestand"}) %></strong></li>
			<li>
				<%= Html.ActionLink("Lijst downloaden", "Download", new { id = Model.GekozenCategorieID }, new { title = "Download de geselecteerde gegevens in een Excel-bestand"}) %></li>
		</ul>
        <h1>Selectie</h1>

    	<select id="GekozenActie" name="GekozenActie">
		    <option value="0">kies een actie</option>
		    <option value="1">Inschrijven</option>
		    <option value="3">In dezelfde categorie stoppen</option>
            <option value="4">Inschrijven voor bivak/uitstap</option>
	    </select>
	    <input id="kiesActie" type="submit" value="Uitvoeren" />

		
		<h1>
			Uitleg</h1>
		<ul>
			<li>
				<%=Html.ActionLink("Wat betekent 'zus/broer maken'?", "ViewTonen", new { Controller = "Handleiding", helpBestand = "ZusBroer" }) %>
			</li>
			<li>
				<%= Html.ActionLink("Wat betekent 'inschrijven'?", "ViewTonen", "Handleiding", null, null, "Inschrijven", new { helpBestand = "Trefwoorden" }, new { title = "Lees in de handleiding wat de gevolgen zijn wanneer je iemand inschrijft" })%></li>
		</ul>
        
	</div>

    <%} %>
</asp:Content>
