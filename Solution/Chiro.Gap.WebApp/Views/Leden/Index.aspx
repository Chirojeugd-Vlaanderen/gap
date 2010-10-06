<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Controllers" %>
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
	<div id="acties">
		<h1>
			Acties</h1>
		<ul>
			<li>
				<%= Html.ActionLink("Lijst downloaden", "Download", new { id = Model.IDGetoondGroepsWerkJaar, afdelingID = Model.AfdelingID, functieID = Model.FunctieID, sortering = Model.GekozenSortering }, new { title = "Download de geselecteerde gegevens in een Excel-bestand" })%></li></ul>
		<h1>
			Filteren</h1>
		<ul>
			<li>
				<%=Html.ActionLink("Alle leden bekijken", "Lijst", new { groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, afdelingID = 0, functieID = 0, sortering = Model.GekozenSortering })%>
			</li>
			<li>
				<%using (Html.BeginForm("AfdelingsLijst", "Leden"))
				{ 
					var dummyItems = new SelectListItem[] {new SelectListItem {Text = "Op afdeling", Value = "0"}};%>
  				<%=Html.DropDownListFor(
					mdl=>mdl.AfdelingID,
  						dummyItems.Union(
					Model.AfdelingsInfoDictionary.Select(src => new SelectListItem {Text = src.Value.AfdelingNaam, Value = src.Value.AfdelingID.ToString()}))) %>

				<input id="kiesAfdeling" type="submit" value="Afdeling bekijken" />
				<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
				<%=Html.HiddenFor(s => s.GekozenSortering)%>
				<%} %>
			</li>
			<li>
			    <%using (Html.BeginForm("FunctieLijst", "Leden"))
			      {
				var dummyItems = new SelectListItem[] {new SelectListItem {Text = "Op functie", Value = "0"}};%>
				
				<%=Html.DropDownListFor(
					mdl=>mdl.FunctieID, 
						dummyItems.Union(
					Model.FunctieInfoDictionary.Select(src => new SelectListItem {Text = src.Value.Naam, Value = src.Value.ID.ToString()}))) %>
				<input id="kiesFunctie" type="submit" value="Functie bekijken" />
				<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
				<%=Html.HiddenFor(s => s.GekozenSortering)%>
			    <%} %>
			</li>
		</ul>
		<h1>
			Uitleg</h1>
		<ul>
			<li>
				<%= Html.ActionLink("Wat betekent 'uitschrijven'?", "ViewTonen", "Handleiding", null, null, "Uitschrijven", new { helpBestand = "Trefwoorden" }, new { title = "Lees in de handleiding wat de gevolgen zijn wanneer je iemand uitschrijft" })%></li>
		</ul>
	</div>
	<% Html.RenderPartial("LedenLijstControl"); %>
</asp:Content>
