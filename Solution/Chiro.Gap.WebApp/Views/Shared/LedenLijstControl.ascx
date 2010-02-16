<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<div class="pager">
Pagina: <%= Html.WerkJaarLinks(ViewData.Model.GroepsWerkJaarIdZichtbaar, ViewData.Model.GroepsWerkJaarLijst, i => Url.Action("List", new { groepsWerkJaarId = Model.GroepsWerkJaarLijst[i].ID, afdID = Model.HuidigeAfdeling }))%>
</div>

<table class="overzicht">
<tr>
<th>Ad-nr.</th><th>Type</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Betaald</th><th>Acties</th><th>Afdeling</th>
</tr>

<!-- TODO: terug naar vorige lijst heeft andere argumenten nodig, dus zou niet dezelfde standaard mogen zijn als bij personen -->

<% foreach (LidInfo l in ViewData.Model.LidInfoLijst) {  %>
<tr>
    <td><%=l.PersoonInfo.AdNummer %></td>
    <td><%=l.Type.ToString() %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", l.PersoonInfo); %></td>
    <td class="right"><%=l.PersoonInfo.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)l.PersoonInfo.GeboorteDatum).ToString("d")%></td>
    <td><%=l.PersoonInfo.Geslacht.ToString()%></td>
    <td><%= l.LidgeldBetaald?"Ja":"Nee"%></td>
    <td>
        <%=Html.ActionLink("Verwijderen", "Verwijderen", new { Controller = "Leden", id = l.LidID })%>
        <%=Html.ActionLink("Afdelingen", "AfdelingBewerken", new { Controller = "Leden", lidID = l.LidID, groepsWerkJaarID = Model.GroepsWerkJaarIdZichtbaar })%>
    </td>
    <td><% foreach (int a in l.AfdelingIdLijst) 
           { %>
               <%=Html.ActionLink(Html.Encode(ViewData.Model.AfdelingsInfoDictionary[a].Afkorting), "List", new { Controller = "Leden", afdID = a, groepsWerkJaarId = Model.GroepsWerkJaarIdZichtbaar }, new { title = ViewData.Model.AfdelingsInfoDictionary[a].Naam } )%>
        <% } %>
    </td>

</tr>
<% } %>

</table>