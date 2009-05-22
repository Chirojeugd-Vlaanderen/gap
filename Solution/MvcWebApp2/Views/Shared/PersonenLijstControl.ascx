<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<GelieerdePersoon>>" %>
<%@ Import Namespace="Cg2.Orm" %>

<table>
<tr>
<th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th>
</tr>

<% foreach (GelieerdePersoon p in ViewData.Model) {  %>
<tr>
    <td><%=p.Persoon.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", p); %></td>
    <td class="right"><%=p.Persoon.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)p.Persoon.GeboorteDatum).ToString("d") %></td>
    <td><%=p.Persoon.Geslacht.ToString() %></td>
    <td>
        <% if (p.Lid.Count == 0)
           { %>
        <%=Html.ActionLink("Lid maken", "LidMaken", new { Controller = "Personen", id = p.ID })%>
        <% } %>
    </td>
</tr>
<% } %>

</table>