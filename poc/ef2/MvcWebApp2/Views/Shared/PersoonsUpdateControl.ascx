<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GelieerdePersoon>" %>
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
<%@ Import Namespace="Cg2.Orm" %>

    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) {%>

<h3>Algemeen</h3>

    <table>
    <tr><td>Ad-nummer:</td><td><%=ViewData.Model.Persoon.AdNummer %></td></tr>    
    <tr><td>Familienaam</td><td><%=Html.TextBox("Persoon.Naam") %></td></tr>
    <tr><td>Voornaam</td><td><%=Html.TextBox("Persoon.VoorNaam") %></td></tr>
    <tr><td>Geboortedatum</td><td><%=Html.TextBox("Persoon.GeboorteDatum") %></td></tr>
    <tr><td>Geslacht</td><td><%=Html.TextBox("Persoon.Geslacht") %></td></tr>
    <tr><td>Chiroleeftijd</td><td><%=Html.TextBox("ChiroLeeftijd") %></td></tr>
    </table>

    <p>
        <%=Html.Hidden("ID") %>
        <%=Html.Hidden("VersieString") %>
        <%=Html.Hidden("BusinessKey") %>
        <%=Html.Hidden("Persoon.ID") %>
        <%=Html.Hidden("Persoon.VersieString") %>
        <%=Html.Hidden("Persoon.BusinessKey") %>
        <%=Html.Hidden("Persoon.AdNummer") %>
        <%=Html.Hidden("EntityKey") %>
    
        <input type="submit" value="Save" />
    </p>

    <% } %>
