<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonInfoModel>" %>
<%@ Import Namespace="MvcWebApp2.Models" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="Cg2.ServiceContracts" %>

<table>
<tr>
<th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th>
</tr>

<% foreach (PersoonInfo p in ViewData.Model.PersoonInfoLijst) {  %>
<tr>
    <td><%=p.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", p); %></td>
    <td class="right"><%=p.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)p.GeboorteDatum).ToString("d") %></td>
    <td><%=p.Geslacht.ToString() %></td>
    <td>
        <% if (!p.IsLid)
           { %>
        <%=Html.ActionLink("Lid maken", "LidMaken", new { Controller = "Personen", id = p.GelieerdePersoonID })%>
        <% } %>
    </td>
</tr>
<% } %>

</table>