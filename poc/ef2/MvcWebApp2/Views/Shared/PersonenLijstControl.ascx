<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<GelieerdePersoon>>" %>
<%@ Import Namespace="Cg2.Orm" %>

<table>
<tr>
<th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Bewerken</th>
</tr>

<% foreach (GelieerdePersoon p in ViewData.Model) {  %>
<tr>
    <td><%=p.Persoon.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", p); %></td>
    <td><%=p.Persoon.GeboorteDatum == null ? "?" : ((DateTime)p.Persoon.GeboorteDatum).ToString("d") %></td>
    <td><%=p.Persoon.Geslacht.ToString() %></td>
    <td><%=Html.ActionLink("Bewerken", "Edit", new { Controller = "Personen", id = p.ID }) %></td>
</tr>
<% } %>

</table>