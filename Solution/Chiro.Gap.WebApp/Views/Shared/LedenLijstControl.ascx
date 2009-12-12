<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<div class="pager">
Pagina: <%= Html.PagerLinks(ViewData.Model.PageHuidig, ViewData.Model.PageTotaal, i => Url.Action("List", new { page = i, groepsWerkJaarId = ViewData.Model.GroepsWerkJaarIdZichtbaar }))%>
</div>

<table>
<tr>
<th>Ad-nr.</th><th>Type</th><th>Afdeling</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th>
</tr>

<!-- TODO: Momenteel is dit slechts een kopie van de PersonenLijst.
Uiteraard willen we voor leden andere info zien als voor personen,
maar dat is nu nog niet geimplementeerd :-) -->

<!-- TODO: terug naar vorige lijst heeft andere argumenten nodig, dus zou niet dezelfde standaard mogen zijn als bij personen -->

<% foreach (LidInfo l in ViewData.Model.LidInfoLijst) {  %>
<tr>
    <td><%=l.PersoonInfo.AdNummer %></td>
    <td><%=l.Type.ToString() %></td>
    <td><%=l.AfdelingsNamen %></td>
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