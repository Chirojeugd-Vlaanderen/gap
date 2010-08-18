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
	<h3>
		Wat kun je hier doen?</h3>
	<ul>
		<li>
			<%= Html.ActionLink("Leiding verzekeren tegen loonverlies", "ViewTonen", "Handleiding", null, null, "Categorie", new { helpBestand = "VerzekeringLoonverlies" }, null)%></li>
		<li>
			<%= Html.ActionLink("Iemand bij een (andere) afdeling zetten", "ViewTonen", "Handleiding", null, null, "Categorie", new { helpBestand = "VeranderenVanAfdeling" }, null)%></li>
		<li>
			<%= Html.ActionLink("Filteren op afdeling of functie", "ViewTonen", "Handleiding", null, null, "Categorie", new { helpBestand = "Filteren" }, null)%></li>
		<li>
			<%=Html.ActionLink("Een lijst downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" })%>
			(bv. om
			<%=Html.ActionLink("etiketten te maken", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenMaken" })%>)</li>
	</ul>
</asp:Content>
