<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<div class="pager">
Pagina: <%= Html.WerkJaarLinks(
                ViewData.Model.GroepsWerkJaarIdZichtbaar, 
                ViewData.Model.WerkJaarInfos, 
                wj => Url.Action("List", new { id = wj.ID, afdID = Model.HuidigeAfdeling }))%>
</div>

<table class="overzicht">
<tr>
<th>Ad-nr.</th><th>Type</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Betaald</th><th>Acties</th><th>Afdeling</th>
</tr>

<!-- TODO: terug naar vorige lijst heeft andere argumenten nodig, dus zou niet dezelfde standaard mogen zijn als bij personen -->

<% foreach (LidInfo lidInfo in ViewData.Model.LidInfoLijst) {  %>
<tr>
    <td><%= lidInfo.PersoonDetail.AdNummer %></td>
    <td><%= lidInfo.Type.ToString() %></td>
    <td><% Html.RenderPartial("LedenLinkControl", lidInfo); %></td>
    <td class="right"><%=lidInfo.PersoonDetail.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)lidInfo.PersoonDetail.GeboorteDatum).ToString("d")%></td>
    <td><%= lidInfo.PersoonDetail.Geslacht.ToString()%></td>
    <td><%= lidInfo.LidgeldBetaald?"Ja":"Nee"%></td>
    <td>
        <%=Html.ActionLink("Verwijderen", "Verwijderen", new { Controller = "Leden", id = lidInfo.LidID })%>
        <%=Html.ActionLink("Afdelingen", "AfdelingBewerken", new { Controller = "Leden", id = lidInfo.LidID })%>
    </td>
    <td><% foreach (int a in lidInfo.AfdelingIdLijst) 
           { %>
               <%=Html.ActionLink(Html.Encode(ViewData.Model.AfdelingsInfoDictionary[a].AfdelingAfkorting), "List", new { Controller = "Leden", afdID = a, groepsWerkJaarId = Model.GroepsWerkJaarIdZichtbaar }, new { title = ViewData.Model.AfdelingsInfoDictionary[a].AfdelingNaam } )%>
        <% } %>
    </td>

</tr>
<% } %>

</table>