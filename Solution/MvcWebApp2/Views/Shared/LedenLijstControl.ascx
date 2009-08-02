<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LidInfoModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="Cg2.ServiceContracts" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<table>
<tr>
<th>Ad-nr.</th><th>Type</th><th>Afdeling</thQ></th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th>
</tr>

<!-- TODO: Momenteel is dit slechts een kopie van de PersonenLijst.
Uiteraard willen we voor leden andere info zien als voor personen,
maar dat is nu nog niet geimplementeerd :-) -->

<% foreach (LidInfo l in ViewData.Model.LidInfoLijst) {  %>
<tr>
    <td><%=l.PersoonInfo.AdNummer %></td>
    <td><%=l.Type.ToString() %></td>
    <td><%=l.AfdelingString %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", l.PersoonInfo); %></td>
    <td class="right"><%=l.PersoonInfo.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)l.PersoonInfo.GeboorteDatum).ToString("d")%></td>
    <td><%=l.PersoonInfo.Geslacht.ToString()%></td>
    <td>
        <%=Html.ActionLink("Verwijderen", "Verwijderen", new { Controller = "Leden", id = l.LidID })%>
        <%=Html.ActionLink("Afdelingen", "AfdelingenBewerken", new { Controller = "Leden", id = l.LidID })%>
    </td>
</tr>
<% } %>

</table>