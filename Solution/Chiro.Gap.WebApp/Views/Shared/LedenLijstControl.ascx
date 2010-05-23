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

<% foreach (Chiro.Gap.ServiceContracts.DataContracts.PersoonLidInfo pl in ViewData.Model.LidInfoLijst) {  %>
<tr>
    <td><%= pl.PersoonDetail.AdNummer %></td>
    <td><%= pl.LidInfo.Type.ToString() %></td>
    <td><% Html.RenderPartial("LedenLinkControl", pl); %></td>
    <td class="right"><%=pl.PersoonDetail.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)pl.PersoonDetail.GeboorteDatum).ToString("d")%></td>
    <td><%= pl.PersoonDetail.Geslacht.ToString()%></td>
    <td><%= pl.LidInfo.LidgeldBetaald?"Ja":"Nee"%></td>
    <td>
		<%if (pl.LidInfo.NonActief)%>
		<%{%>
			<%=Html.ActionLink("Activeren", "Activeren", new { Controller = "Leden", id = pl.LidInfo.LidID })%>
		<%}else{%>
			<%=Html.ActionLink("Non-Actief maken", "DeActiveren", new { Controller = "Leden", id = pl.LidInfo.LidID })%>
		<%} %>
        
        <%=Html.ActionLink("Afdelingen", "AfdelingBewerken", new { Controller = "Leden", id = pl.LidInfo.LidID })%>
    </td>
    <td><% foreach (int a in pl.LidInfo.AfdelingIdLijst) 
           { %>
               <%=Html.ActionLink(Html.Encode(ViewData.Model.AfdelingsInfoDictionary[a].AfdelingAfkorting), "List", new { Controller = "Leden", afdID = a, id = Model.GroepsWerkJaarIdZichtbaar }, new { title = ViewData.Model.AfdelingsInfoDictionary[a].AfdelingNaam } )%>
        <% } %>
    </td>

</tr>
<% } %>

</table>