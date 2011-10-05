<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Ingeschreven
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Ingeschreven</h2>
	<h3>
		Wat zie je hier?</h3>
	<p>
		Op dit tabblad vind je een overzicht van al wie ingeschreven is, per werkjaar.
		Klik je op iemands naam, dan kun je zijn of haar persoonsgegevens aanpassen.
		Andere links hebben met lidgegevens te maken. Die kun je alleen aanpassen in
		het huidige werkjaar. Gegevens over het verleden staan vast, daar kun je niets
		meer aan veranderen.</p>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Tabblad_Ingeschreven.png") %>"
		alt="Het tabblad 'Ingeschreven'" />
    <p>Je ledenlijst wordt doorgegeven aan Chirojeugd Vlaanderen, voor de aansluiting&nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Aansluiting", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een aansluiting?" })%> en de verzekering. Dat gebeurt per lid, na het einde van hun instapperiode&nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een instapperiode?" })%>.</p>
	<h3>
		Wat kun je hier doen?</h3>
	<ul>
		<li>
			<%= Html.ActionLink("Leiding verzekeren tegen loonverlies", "ViewTonen", new { controller = "Handleiding", helpBestand = "VerzekeringLoonverlies" }, new { title = "Werkende leiding extra verzekeren"})%></li>
		<li>
			<%= Html.ActionLink("Iemand bij een (andere) afdeling zetten", "ViewTonen", new { controller = "Handleiding", helpBestand = "VeranderenVanAfdeling" }, new { title = "Leden in of leiding bij een andere afdeling zetten" })%></li>
		<li>
			<%= Html.ActionLink("Filteren op afdeling of functie", "ViewTonen", new { controller = "Handleiding", helpBestand = "Filteren" }, new { title = "De ledenlijst filteren op afdeling of functie" })%></li>
		<li>
			<%=Html.ActionLink("Een lijst downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" }, new { title = "Gegevens downloaden als Excel-bestand" })%>
			(bv. om
			<%=Html.ActionLink("etiketten te maken", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenMaken" }, new { title = "Etiketten maken met gegevens uit een Excel-bestand" })%>)</li>
	</ul>
</asp:Content>
