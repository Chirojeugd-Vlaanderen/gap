<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
	Handleiding: Jaarovergang
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Jaarovergang</h2>
	<h3>
		Wat zie je hier?</h3>
	<p>
		Het tabblad voor de jaarovergang wordt zichtbaar in de periode dat het nationaal
		secretariaat aan het nieuwe werkjaar begint, dus rond 1 september. Het blijft
		zichtbaar tot een GAV
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "GAV", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een GAV?"})%> van
		je groep het proces doorlopen heeft. Dat moet <b>voor 15 oktober</b> gebeuren.
		Neem daar wel je tijd voor! Er moet op dat moment veel gebeuren, en het is het
		beste dat je het allemaal ineens doet - anders kun je een factuur
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Factuur", new { helpBestand = "Trefwoorden" }, new { title = "Uitleg over facturatie"})%>
		krijgen voor de aansluiting&nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Factuur", new { helpBestand = "Trefwoorden" }, new { title = "Uitleg over facturatie"})%>van mensen die je eigenlijk niet wilt inschrijven.</p>
	<h3>
		Wat kun je hier doen?</h3>
	<ul>
		<li>
			<%=Html.ActionLink("De jaarovergang uitvoeren", "ViewTonen", new { controller = "Handleiding", helpBestand = "JaarovergangUitvoeren" })%></li>
	</ul>
</asp:Content>
