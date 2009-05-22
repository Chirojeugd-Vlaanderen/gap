<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Lid>>" %>
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
    <td class="right"><%=l.GelieerdePersoon.Persoon.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)l.GelieerdePersoon.Persoon.GeboorteDatum).ToString("d")%></td>
    <td><%=l.GelieerdePersoon.Persoon.Geslacht.ToString()%></td>
    <td>
        <%=Html.ActionLink("Bewerken", "Edit", new { Controller = "Personen", id = l.GelieerdePersoon.ID })%>
    </td>
</tr>
<% } %>

</table>