<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Controllers" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<div class="pager">
	Pagina:
	<%= Html.WerkJaarLinks(
                ViewData.Model.IDGetoondGroepsWerkJaar, 
                ViewData.Model.WerkJaarInfos,
				wj => Url.Action("Lijst", new { Controller = "Leden", id = wj.ID, sortering = Model.GekozenSortering, afdelingID = Model.AfdelingID, functieID = Model.FunctieID }))%>
</div>
<table class="overzicht">
	<tr>
		<th>Ad-nr. </th>
		<th>Type </th>
		<th>
			<%= Html.ActionLink("Naam", "Lijst", new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Naam, afdelingID = Model.AfdelingID, functieID = Model.FunctieID }, new { title = "Sorteren op naam" })%>
		</th>
		<th>
			<%= Html.ActionLink("Geboortedatum", "Lijst", new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Leeftijd, afdelingID = Model.AfdelingID, functieID = Model.FunctieID }, new { title = "Sorteren op geboortedatum" })%>
		</th>
		<th>
			<%=Html.Geslacht(GeslachtsType.Man) %>
			<%=Html.Geslacht(GeslachtsType.Vrouw) %>
		</th>
		<th>Betaald </th>
		<th>
			<%= Html.ActionLink("Afd.", "Lijst", new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Afdeling, afdelingID = Model.AfdelingID, functieID = Model.FunctieID }, new { title = "Sorteren op afdeling" })%>
		</th>
		<th>Func. </th>
		<%=Model.KanLedenBewerken ? "<th>Acties</th>" : String.Empty %>
	</tr>
	<%
     foreach (LidOverzicht lidOverzicht in ViewData.Model.LidInfoLijst)
	{  %>
	<tr>
		<td>
			<%= lidOverzicht.AdNummer %>
		</td>
		<td>
			<%= lidOverzicht.Type == LidType.Kind ? "Lid" : "Leiding" %>
		</td>
		<td>
			<% Html.RenderPartial("LedenLinkControl", lidOverzicht); %>
		</td>
		<td class="right">
			<%=lidOverzicht.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)lidOverzicht.GeboorteDatum).ToString("d")%>
		</td>
		<td class="center">
			<%= Html.Geslacht(lidOverzicht.Geslacht)%>
		</td>
		<td>
			<%= lidOverzicht.LidgeldBetaald?"Ja":"Nee"%>
		</td>
		<td>
			<% foreach (var a in lidOverzicht.Afdelingen)
	  { %>
			<%=Html.ActionLink(Html.Encode(a.Afkorting), "Lijst", new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = Model.GekozenSortering, afdelingID = a.ID, functieID = 0 }, new { title = ViewData.Model.AfdelingsInfoDictionary[a.ID].AfdelingNaam })%>
			<% } %>
		</td>
		<td>
			<% foreach (var functieInfo in lidOverzicht.Functies)
	  { %>
			<%=Html.ActionLink(Html.Encode(functieInfo.Code), "Lijst", new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = Model.GekozenSortering, afdelingID = 0, functieID = functieInfo.ID }, new { title = ViewData.Model.FunctieInfoDictionary[functieInfo.ID].Naam })%>
			<% } %>
		</td>
		<%if (Model.KanLedenBewerken)
	{%>
		<td>
			<%=Html.ActionLink("uitschrijven", "DeActiveren", new { Controller = "Leden", id = lidOverzicht.GelieerdePersoonID }, new { title = "Op non-actief zetten, zodat de persoon niet meer in de ledenlijst verschijnt" })%>
			<%=Html.ActionLink("afd.", "AfdelingBewerken", new { Controller = "Leden", lidID = lidOverzicht.LidID }, new { title = "Bij een (andere) afdeling zetten" })%>
		</td>
		<%
			}%>
	</tr>
	<% } %>
</table>
