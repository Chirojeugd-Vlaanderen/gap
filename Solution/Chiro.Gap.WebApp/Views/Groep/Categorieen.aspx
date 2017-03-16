<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GroepsInstellingenModel>" %>

<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Cleanup en refactoring met module pattern: Copyright 2015 Sam Segers
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
    <script src="<%= ResolveUrl("~/Scripts/Modules/GroepModule.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            GroepModule.InitVoorCategorieen();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
    <div id="extraInfoDialog" hidden>
        <img src="<%= ResolveUrl("~/Content/images/loading.gif")%>" />
    </div>

    <ul id="groep_Menu">
        <li><%=Html.ActionLink("Algemene groepsinfo", "Index")%></li>
        <% 
		if ((Model.Detail.Niveau & Niveau.Groep) != 0)
		{ // Afdelingen enkel tonen voor Chirogroepen (niet voor kadergroepen of satellieten) 
        %>
        <li><%=Html.ActionLink("Afdelingen dit werkjaar", "Afdelingen")%></li>
        <%} %>
        <li><%=Html.ActionLink("Categorie�n", "Categorieen")%></li>
        <li><%=Html.ActionLink("Functies", "Functies")%></li>
    </ul>
    
	<fieldset id="groep_categorie�n">
		<legend>Categorie�n voor ingeschreven en niet-ingeschreven personen</legend>
		<table>
		    <thead>
		        <th>Categorie</th>
                <th>Code</th>
                <th></th>
		    </thead>
		    <tbody>
			<% foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code)) { %>
			<tr>
				<td><%=cat.Naam%><input value="<%=cat.ID %>" hidden/></td>
                <td><%=cat.Code %></td>
                <td><div class="categorieVerwijderen ui-icon ui-icon-circle-minus" title="Verwijderen" style="cursor: pointer"></div></td>
			</tr>
			<% } %>
            </tbody>
		</table>
        <button id="groep_categorie�n_Toevoegen">Categorie toevoegen</button>
	</fieldset>
</asp:Content>
