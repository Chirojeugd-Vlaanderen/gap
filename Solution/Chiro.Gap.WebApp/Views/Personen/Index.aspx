<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersoonInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #713) %>
	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>
	<script type="text/javascript">
		$(document).ready(function() {
			$('#kiesCategorie').hide();
			$("#GekozenCategorieID").change(function() {
				$('#kiesCategorie').click();
			});
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<%using (Html.BeginForm("List", "Personen"))
   { %>
	<div id="acties">
		<h1>
			Acties</h1>
		<ul>
			<li>
				<strong><%= Html.ActionLink("Nieuwe persoon", "Nieuw", new{ title="Voeg een nieuwe persoon toe in je gegevensbestand"}) %></strong></li>
			<li>
				<%= Html.ActionLink("Lijst downloaden", "Download", new { id = Model.GekozenCategorieID }, new { title = "Download de geselecteerde gegevens in een Excel-bestand"}) %></li>
		</ul>
		<h1>
			Filteren</h1>
		<ul>
			<li>
				<select id="GekozenCategorieID" name="GekozenCategorieID">
					<option value="">Op categorie</option>
					<% foreach (var s in Model.GroepsCategorieen)
		{%>
					<option value="<%=s.ID%>">
						<%=s.Naam%></option>
					<%}%>
				</select>
				<input id="kiesCategorie" type="submit" value="Uitvoeren" />
				<%=Html.HiddenFor(e => e.Sortering) %>
			</li>
		</ul>
		<h1>
			Uitleg</h1>
		<ul>
			<li>
				<%=Html.ActionLink("Wat betekent 'zus/broer maken'?", "ViewTonen", new { Controller = "Handleiding", helpBestand = "ZusBroer" }) %>
			</li>
			<li>
				<%= Html.ActionLink("Wat betekent 'inschrijven'?", "ViewTonen", "Handleiding", null, null, "Inschrijven", new { helpBestand = "Trefwoorden" }, new { title = "Lees in de handleiding wat de gevolgen zijn wanneer je iemand inschrijft" })%></li>
		</ul>
	</div>
	<%} %>
	<% Html.RenderPartial("PersonenLijstControl", Model); %>
</asp:Content>
