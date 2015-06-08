<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GroepsInstellingenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
            GroepModule.InitVoorFuncties();
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
        <li><%=Html.ActionLink("Categorieën", "Categorieen")%></li>
        <li><%=Html.ActionLink("Functies", "Functies")%></li>
    </ul>
    
	<fieldset id="groep_functies">
		<legend>Eigen functies voor ingeschreven leden en leiding</legend>
		<table>
		    <thead>
		        <th>Naam</th>
                <th>Code</th>
                <th>Type</th>
                <th></th>
                <th></th>
		    </thead>
		    <tbody>
			    <% foreach (var fie in Model.Detail.Functies.Where(fie=>fie.WerkJaarTot == null || fie.WerkJaarTot < Model.HuidigWerkJaar).OrderBy(fie => fie.Type)) { %>
			    <tr>
			        <td hidden><input value="<%=fie.ID %>"/></td>
			        <td><%=fie.Naam %></td>
                    <td><%=fie.Code %></td>
                    <td><%=fie.Type == LidType.Kind ? "leden" : fie.Type == LidType.Leiding ? "leiding" : "leden en leiding"%></td>
                    <td><div class="functieBewerken ui-icon ui-icon-pencil" title="Bewerken" style="cursor:pointer"></div></td>
                    <td><div class="functieVerwijderen ui-icon ui-icon-circle-minus" title="Verwijderen" style="cursor: pointer"></div></td>
			    </tr>
			    <% } %>
            </tbody>
		</table>
        <button id="groep_functies_toev_verw">Functie toevoegen</button>
	</fieldset>
</asp:Content>
