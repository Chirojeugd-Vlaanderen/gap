<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LidInfoModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="Cg2.ServiceContracts" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<table>
<tr>
<th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th>
</tr>

<!-- TODO: Momenteel is dit slechts een kopie van de PersonenLijst.
Uiteraard willen we voor leden andere info zien als voor personen,
maar dat is nu nog niet geimplementeerd :-) -->

<% foreach (LidInfo l in ViewData.Model.LidInfoLijst) {  %>
<tr>
    <td><%=l.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", l); %></td>
    <td class="right"><%=l.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)l.GeboorteDatum).ToString("d")%></td>
    <td><%=l.Geslacht.ToString()%></td>
    <td>
        <%=Html.ActionLink("Bewerken", "Edit", new { Controller = "Personen", id = l.GelieerdePersoonID })%>
    </td>
</tr>
<% } %>

</table>