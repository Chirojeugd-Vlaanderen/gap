<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersoonInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #713) %>
	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>"></script>
	<script type="text/javascript">
		$(document).ready(function() {
			$('#kiesCategorie').hide();
			$("#GekozenCategorieID").change(function() {
				$('#kiesCategorie').click();
            });

            $('#kiesActie').hide();
            $("#GekozenActie").change(function () {
                $('#kiesActie').click();
            });
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<p><em>Hou je muispijl boven een link in de tabel - zonder te klikken - voor iets meer uitleg over wat de link doet.</em></p>
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
        <h1>Selectie</h1>

    	<select id="GekozenActie" name="GekozenActie">
		    <option value="0">kies een actie</option>
		    <option value="1">Inschrijven</option>
		    <option value="3">In dezelfde categorie stoppen</option>
            <option value="4">Inschrijven voor bivak/uitstap</option>
	    </select>
	    <input id="kiesActie" type="submit" value="Uitvoeren" />

		<h1>
			Filteren</h1>
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
	
	<% Html.RenderPartial("PersonenLijstControl", Model); %>

    <%} %>
</asp:Content>
