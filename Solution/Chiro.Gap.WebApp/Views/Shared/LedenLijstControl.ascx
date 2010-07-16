<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<div class="pager">
Pagina: <%= Html.WerkJaarLinks(
                ViewData.Model.IDGetoondGroepsWerkJaar, 
                ViewData.Model.WerkJaarInfos,
        				//TODO momentele wordt er altijd terug naar de volledige lijst gegaan, dit kan nog aangepast worden door een huidigeafdeling en een huidigefunctie bij te houden.
                        // (overigens is het niet zeker dat de huidig geselecteerde afdeling ook in het andere werkjaar
                        // beschikbaar is.
				wj => Url.Action("AfdelingsLijst", new { Controller = "Leden", groepsWerkJaarID = wj.ID/*, afdID = Model.HuidigeAfdeling*/ }))%>
</div>

<table class="overzicht">
<tr>
    <th>Ad-nr.</th>
    <th>Type</th>
    <th>Naam</th>
    <th>Geboortedatum</th>
    <th><%=Html.Geslacht(GeslachtsType.Man) %> <%=Html.Geslacht(GeslachtsType.Vrouw) %></th>
    <th>Betaald</th>
    <th>Afd.</th>
    <th>Func.</th>
    <th>Acties</th>  
</tr>

<% foreach (Chiro.Gap.ServiceContracts.DataContracts.PersoonLidInfo pl in ViewData.Model.LidInfoLijst) {  %>
<tr>
    <td><%= pl.PersoonDetail.AdNummer %></td>
    <td><%= pl.LidInfo.Type.ToString() %></td>
    <td><% Html.RenderPartial("LedenLinkControl", pl); %></td>
    <td class="right"><%=pl.PersoonDetail.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)pl.PersoonDetail.GeboorteDatum).ToString("d")%></td>
    <td><%= Html.Geslacht(pl.PersoonDetail.Geslacht)%></td>
    <td><%= pl.LidInfo.LidgeldBetaald?"Ja":"Nee"%></td>
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
    <td>
		<%if (pl.LidInfo.NonActief)%>
		<%{%>
			<%=Html.ActionLink("inschrijven", "Activeren", new { Controller = "Leden", id = pl.LidInfo.LidID })%>
		<%}else{%>
			<%=Html.ActionLink("uitschrijven", "DeActiveren", new { Controller = "Leden", id = pl.LidInfo.LidID })%>
		<%} %>
        
        <%=Html.ActionLink("afd.", "AfdelingBewerken", new { Controller = "Leden", lidID = pl.LidInfo.LidID })%>
    </td>    
</tr>
<% } %>

</table>