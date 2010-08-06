<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Controllers" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<div class="pager">
	Pagina:
	<%= Html.WerkJaarLinks(
                ViewData.Model.IDGetoondGroepsWerkJaar, 
                ViewData.Model.WerkJaarInfos,
        				// TODO momenteel wordt er altijd terug naar de volledige lijst gegaan, dit kan nog aangepast worden door een huidigeafdeling en een huidigefunctie bij te houden.
                        // (overigens is het niet zeker dat de huidig geselecteerde afdeling ook in het andere werkjaar
                        // beschikbaar is.
				wj => Url.Action("Lijst", new { Controller = "Leden", groepsWerkJaarID = wj.ID, sortering = Model.GekozenSortering, lijst = LijstEnum.Alles/*, afdID = Model.HuidigeAfdeling*/ }))%>
</div>
<table class="overzicht">
	<tr>
		<th>Ad-nr. </th>
		<th>Type </th>
		<th>
			<%= Html.ActionLink("Naam", "Lijst", new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, sortering = LedenSorteringsEnum.Naam, lijst = Model.GekozenLijst, ID = Model.GekozenID })%>
		</th>
		<th>
			<%= Html.ActionLink("Geboortedatum", "Lijst", new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, sortering = LedenSorteringsEnum.Leeftijd, lijst = Model.GekozenLijst, ID = Model.GekozenID  })%>
		</th>
		<th>
			<%=Html.Geslacht(GeslachtsType.Man) %>
			<%=Html.Geslacht(GeslachtsType.Vrouw) %>
		</th>
		<th>Betaald </th>
		<th>
			<%= Html.ActionLink("Afd.", "Lijst", new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, sortering = LedenSorteringsEnum.Afdeling, lijst = Model.GekozenLijst, ID = Model.GekozenID })%>
		</th>
		<th>Func. </th>
		<%=Model.KanLedenBewerken ? "<th>Acties</th>" : String.Empty %>
	</tr>
	<% foreach (Chiro.Gap.ServiceContracts.DataContracts.PersoonLidInfo pl in ViewData.Model.LidInfoLijst)
	{  %>
	<tr>
		<td>
			<%= pl.PersoonDetail.AdNummer %>
		</td>
		<td>
			<%= pl.LidInfo.Type == LidType.Kind ? "Lid" : "Leiding" %>
		</td>
		<td>
			<% Html.RenderPartial("LedenLinkControl", pl); %>
		</td>
		<td class="right">
			<%=pl.PersoonDetail.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)pl.PersoonDetail.GeboorteDatum).ToString("d")%>
		</td>
		<td class="center">
			<%= Html.Geslacht(pl.PersoonDetail.Geslacht)%>
		</td>
		<td>
			<%= pl.LidInfo.LidgeldBetaald?"Ja":"Nee"%>
		</td>
		<td>
			<% foreach (var a in pl.LidInfo.AfdelingIdLijst)
	  { %>
			<%=Html.ActionLink(Html.Encode(ViewData.Model.AfdelingsInfoDictionary[a].AfdelingAfkorting), "Lijst", new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, sortering = Model.GekozenSortering, lijst = LijstEnum.Afdeling, ID = a }, new { title = ViewData.Model.AfdelingsInfoDictionary[a].AfdelingNaam })%>
			<% } %>
		</td>
		<td>
			<% foreach (var a in pl.LidInfo.Functies)
	  { %>
			<%=Html.ActionLink(Html.Encode(ViewData.Model.FunctieInfoDictionary[a.ID].Code), "Lijst", new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, sortering = Model.GekozenSortering, lijst = LijstEnum.Functie, a.ID }, new { title = ViewData.Model.FunctieInfoDictionary[a.ID].Naam })%>
			<% } %>
		</td>
		<%if (Model.KanLedenBewerken)
	{%>
		<td>
			<%=Html.ActionLink("uitschrijven", "DeActiveren", new { Controller = "Leden", id = pl.PersoonDetail.GelieerdePersoonID })%>
			<%=Html.ActionLink("afd.", "AfdelingBewerken", new { Controller = "Leden", lidID = pl.LidInfo.LidID })%>
		</td>
		<%
			}%>
	</tr>
	<% } %>
</table>
