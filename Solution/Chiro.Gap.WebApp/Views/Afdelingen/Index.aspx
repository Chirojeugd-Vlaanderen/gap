<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AfdelingsOverzichtModel>" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<ul id="acties">
		<li>
			<%= Html.ActionLink("Nieuwe afdeling", "Nieuw") %></li>
	</ul>
	<h3>
		Afdelingen beschikbaar in het huidige werkjaar</h3>
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
		<% foreach (var ai in Model.Actief)
	 { %>
		<tr>
			<td>
				<%=ai.AfdelingNaam %>
			</td>
			<td>
				<%=ai.AfdelingAfkorting %>
			</td>
			<td>
				<%=ai.OfficieleAfdelingNaam %>
			</td>
            <td>
                <%=(ai.Geslacht == GeslachtsType.Gemengd) ? "Gemengd" : "" %>
                <%=(ai.Geslacht == GeslachtsType.Man) ? "Jongens" : "" %>
                <%=(ai.Geslacht == GeslachtsType.Vrouw) ? "Meisjes" : "" %>
            </td>
			<td>
				<%=ai.GeboorteJaarVan %>
			</td>
			<td>
				<%=ai.GeboorteJaarTot %>
			</td>
			<td>
				<%=Html.ActionLink("Bewerken", "AfdJaarBewerken", new { Controller = "Afdelingen", id = ai.AfdelingsJaarID } )%> -
				<%=Html.ActionLink("Verwijderen van huidig werkjaar", "VerwijderenVanWerkjaar", new { Controller = "Afdelingen", id = ai.AfdelingsJaarID })%>
			</td>
		</tr>
		<% } %>
	</table>
	<h3>
		Overige afdelingen (niet geactiveerd in dit werkjaar)</h3>
	<table>
		<tr>
			<th>Afdeling</th>
			<th>Afkorting</th>
			<th class="center">Actie</th>
		</tr>
		<% foreach (var ai in Model.NietActief)
	 { %>
		<tr>
			<td>
				<%=ai.Naam %>
			</td>
			<td>
				<%=ai.Afkorting %>
			</td>
			<td>
				<%=Html.ActionLink("Activeren in huidig werkjaar", "Activeren", new { Controller = "Afdelingen", id = ai.ID } )%> -
                <%=Html.ActionLink("Bewerken", "AfdBewerken", new { Controller = "Afdelingen", id = ai.ID } )%> -
                <%=Html.ActionLink("Verwijderen", "Verwijderen", new { Controller = "Afdelingen", id = ai.ID } )%>
			</td>
		</tr>
		<% } %>
        <% if (!Model.NietActief.Any())
           { %>
        <tr>
            <td colspan="3">
                Geen afdelingen gevonden.
            </td>
        </tr>
        <% } %>
	</table>
</asp:Content>
