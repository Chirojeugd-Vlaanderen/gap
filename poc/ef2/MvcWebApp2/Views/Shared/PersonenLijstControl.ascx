<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<GelieerdePersoon>>" %>
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

<table>
<tr>
<th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th>
</tr>

<% foreach (GelieerdePersoon p in ViewData.Model) {  %>
<tr>
    <td><%=p.Persoon.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", p); %></td>
    <td><%=p.Persoon.GeboorteDatum == null ? "?" : ((DateTime)p.Persoon.GeboorteDatum).ToString("d") %></td>
    <td><%=p.Persoon.Geslacht.ToString() %></td>
    <td>
        <%=Html.ActionLink("Bewerken", "Edit", new { Controller = "Personen", id = p.ID }) %>
        <% if (p.Lid.Count == 0)
           { %>
        <%=Html.ActionLink("Lid maken", "LidMaken", new { Controller = "Personen", id = p.ID })%>
        <% } %>
    </td>
</tr>
<% } %>

</table>