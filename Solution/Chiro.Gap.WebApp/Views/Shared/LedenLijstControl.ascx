<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LidInfoModel>" %>

<div class="pager">
Pagina: <%= Html.WerkJaarLinks(
                ViewData.Model.IDGetoondGroepsWerkJaar, 
                ViewData.Model.WerkJaarInfos,
        				//TODO momentele wordt er altijd terug naar de volledige lijst gegaan, dit kan nog aangepast worden door een huidigeafdeling en een huidigefunctie bij te houden.
				wj => Url.Action("AfdelingsLijst", new { Controller = "Leden", groepsWerkJaarID = wj.ID/*, afdID = Model.HuidigeAfdeling*/ }))%>
</div>

<table class="overzicht">
<tr>
<th>Ad-nr.</th><th>Type</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Betaald</th><th>Acties</th><th>Afdeling</th><th>Functie</th>
</tr>

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
        
        <%=Html.ActionLink("Afdelingen", "AfdelingBewerken", new { Controller = "Leden", lidID = pl.LidInfo.LidID })%>
    </td>
    <td><% foreach (var a in pl.LidInfo.AfdelingIdLijst) 
           { %>
               <%=Html.ActionLink(Html.Encode(ViewData.Model.AfdelingsInfoDictionary[a].AfdelingAfkorting), "AfdelingsLijst", new { Controller = "Leden", afdID = a, groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar }, new { title = ViewData.Model.AfdelingsInfoDictionary[a].AfdelingNaam })%>
        <% } %>
    </td>
    <td><% foreach (var a in pl.LidInfo.Functies) 
           { %>
               <%=Html.ActionLink(Html.Encode(ViewData.Model.FunctieInfoDictionary[a.ID].Code), "FunctieLijst", new { Controller = "Leden", funcID = a.ID, groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar }, new { title = ViewData.Model.FunctieInfoDictionary[a.ID].Naam })%>
        <% } %>
    </td>
</tr>
<% } %>

</table>