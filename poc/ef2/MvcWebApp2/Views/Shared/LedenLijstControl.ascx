<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Lid>>" %>
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

<!-- TODO: Momenteel is dit slechts een kopie van de PersonenLijst.
Uiteraard willen we voor leden andere info zien als voor personen,
maar dat is nu nog niet geimplementeerd :-) -->

<% foreach (Lid l in ViewData.Model) {  %>
<tr>
    <td><%=l.GelieerdePersoon.Persoon.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", l.GelieerdePersoon); %></td>
    <td><%=l.GelieerdePersoon.Persoon.GeboorteDatum == null ? "?" : ((DateTime)l.GelieerdePersoon.Persoon.GeboorteDatum).ToString("d")%></td>
    <td><%=l.GelieerdePersoon.Persoon.Geslacht.ToString()%></td>
    <td>
        <%=Html.ActionLink("Bewerken", "Edit", new { Controller = "Personen", id = l.GelieerdePersoon.ID })%>
    </td>
</tr>
<% } %>

</table>