<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GroepsInstellingenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
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
    <script src="<%= ResolveUrl("~/Scripts/jquery-groep.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#groep_Menu').menu();
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
    
    <% if ((Model.Detail.Niveau & Niveau.Groep) != 0) {
  			// Afdelingen enkel tonen voor Chirogroepen (niet voor kadergroepen of satellieten)  	
    %>
	    <fieldset id="groep_afdelingen">
		    <legend>Afdelingen</legend>
                <div class="Foutmelding" id="errors" hidden></div>
	            <h3>Afdelingen beschikbaar in het huidige werkjaar</h3>
                <%=Html.ValidationSummary()%>
	            <table>
		            <tr>
			            <th>Afdeling</th>
                        <th>Afkorting</th>
			            <th>Offici&euml;le afdeling</th>
                        <th>Geslacht</th>
			            <th>Van</th>
			            <th>Tot</th>
			            <th class="center">Actie</th>
		            </tr>
		            <% foreach (var ai in Model.Detail.Afdelingen.OrderByDescending(afd => afd.GeboorteJaarVan)) { %>
		            <tr>
			            <td><%=ai.AfdelingNaam %><input value="<%=ai.AfdelingsJaarID %>" readonly hidden/></td>
			            <td><%=ai.AfdelingAfkorting %></td>
			            <td><%=ai.OfficieleAfdelingNaam %></td>
                        <td>
                            <%=(ai.Geslacht == GeslachtsType.Gemengd) ? "Gemengd" : "" %>
                            <%=(ai.Geslacht == GeslachtsType.Man) ? "Jongens" : "" %>
                            <%=(ai.Geslacht == GeslachtsType.Vrouw) ? "Meisjes" : "" %>
                        </td>
			            <td><%=ai.GeboorteJaarVan %></td>
			            <td><%=ai.GeboorteJaarTot %></td>
			            <td>
                            <div class="groep_bewerkAfdeling ui-icon ui-icon-pencil" title="Bewerken" style="cursor:pointer"></div>
                            <div class="afdelingjaarVerwijderen ui-icon ui-icon-circle-minus" title="Verwijderen uit huidig werkjaar" style="cursor: pointer"></div>
			            </td>
		            </tr>
		            <% } %>
	            </table>
                
                <% if (Model.NonActieveAfdelingen.Any()) { %>
                    <h3>Overige afdelingen (niet geactiveerd in dit werkjaar)</h3>
	                <table>
		                <tr>
			                <th>Afdeling</th>
			                <th>Afkorting</th>
			                <th class="center">Actie</th>
                            <th></th>
		                </tr>
		                <% foreach (var ai in Model.NonActieveAfdelingen) { %>
		                <tr>
			                <td><%=ai.Naam %><input value="<%=ai.ID %>" readonly hidden/></td>
			                <td><%=ai.Afkorting %></td>
                            <td><button id="activeer<%=ai.ID%>" class="afdActiveren">Activeren</button></td>
			                <td><div class="afdelingVerwijderen ui-icon ui-icon-circle-minus" title="Verwijderen" style="cursor: pointer"></div></td>
		                </tr>
		                <% } %>
	                </table>
                <%} %>
                <button id="groep_afdelingen_nieuw">Nieuwe Afdeling</button>
	    </fieldset>
    <% } %>
</asp:Content>
