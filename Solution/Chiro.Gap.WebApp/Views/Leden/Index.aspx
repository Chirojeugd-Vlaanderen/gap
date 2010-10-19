<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Controllers" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #713) %>
	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>
	<script type="text/javascript">
		$(document).ready(function() {
			$('#kiesAfdeling').hide();
			$("#AfdelingID").change(function() {
				$('#kiesAfdeling').click();
			});
			$('#kiesFunctie').hide();
			$("#FunctieID").change(function() {
				$('#kiesFunctie').click();
			});
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<p><em>Hou je muispijl boven een link in de tabel - zonder te klikken - voor iets meer uitleg over wat de link doet.</em></p>
	<div id="actiesmini">
		<span><%= Html.ActionLink(
		      	"Lijst downloaden", 
		      	"Download", 
		      	new { id = Model.IDGetoondGroepsWerkJaar, afdelingID = Model.AfdelingID, functieID = Model.FunctieID, sortering = Model.GekozenSortering }, 
		      	new { title = "Download de geselecteerde gegevens in een Excel-bestand" })%></span>
		<span><%=Html.ActionLink(
		      	"In instapperiode", 
		      	"ProbeerLeden",
		      	new { groepID = Model.GroepID, sortering = Model.GekozenSortering }, 
		      	new {title = "Leden en leiding tonen van wie de instapperiode nog niet verlopen is"})%></span>

		<span>
				<%using (Html.BeginForm("AfdelingsLijst", "Leden"))
				{ 
					var dummyItems = new SelectListItem[] {new SelectListItem {Text = "Filter op afdeling", Value = "0"}};%>
  				<%=Html.DropDownListFor(
					mdl=>mdl.AfdelingID,
  						dummyItems.Union(
					Model.AfdelingsInfoDictionary.Select(src => new SelectListItem {Text = src.Value.AfdelingNaam, Value = src.Value.AfdelingID.ToString()}))) %>

				<input id="kiesAfdeling" type="submit" value="Afdeling bekijken" />
				<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
				<%=Html.HiddenFor(s => s.GekozenSortering)%>
				<%} %>
		</span>
		<span>
			    <%using (Html.BeginForm("FunctieLijst", "Leden"))
			      {
				var dummyItems = new SelectListItem[] {new SelectListItem {Text = "Filter op functie", Value = "0"}};%>
				
				<%=Html.DropDownListFor(
					mdl=>mdl.FunctieID, 
						dummyItems.Union(
					Model.FunctieInfoDictionary.Select(src => new SelectListItem {Text = src.Value.Naam, Value = src.Value.ID.ToString()}))) %>
				<input id="kiesFunctie" type="submit" value="Functie bekijken" />
				<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
				<%=Html.HiddenFor(s => s.GekozenSortering)%>
			    <%} %>
		</span>
		<span><%= Html.ActionLink("Sorteer op verjaardag", "Lijst", new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Verjaardag, afdelingID = Model.AfdelingID, functieID = Model.FunctieID }, new { title = "Sorteren op verjaardag" })%></span>
		<span><%=Html.ActionLink(
		      	"Filters opheffen",
		      	"Lijst", 
		      	new { groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, afdelingID = 0, functieID = 0, sortering = Model.GekozenSortering },
			new { title ="Opnieuw alle ingeschreven leden en leiding van het gevraagde werkjaar tonen"}	)%></span>
		<span><%= Html.ActionLink("Wat betekent 'uitschrijven'?", "ViewTonen", "Handleiding", null, null, "Uitschrijven", new { helpBestand = "Trefwoorden" }, new { title = "Lees in de handleiding wat de gevolgen zijn wanneer je iemand uitschrijft" })%></span>
	</div>
	
	<div class="pager">
	Pagina:
	<%= Html.WerkJaarLinks(
                ViewData.Model.IDGetoondGroepsWerkJaar, 
                ViewData.Model.WerkJaarInfos,
				wj => Url.Action("Lijst", new { Controller = "Leden", id = wj.ID, sortering = Model.GekozenSortering, afdelingID = Model.AfdelingID, functieID = Model.FunctieID }))%>
	</div>
	
	<% Html.RenderPartial("LedenLijstControl"); %>
</asp:Content>
