<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Iedereen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iedereen</h2>
	<h3>
		Wat zie je hier?</h3>
	<p>
		Op dit tabblad zie je een overzicht van alle personen die je ooit toevoegde.
		Dat zijn in de eerste plaats je leden en je leiding, maar ook kandidaat-leden
		die afhaakten, de mensen van je kookploeg, contactpersonen in de jeugdraad en
		het parochieteam, enzovoort. Je kiest zelf wie je allemaal toevoegt. Alleen
		van de leden en de leiding die je aansluit, worden de gegevens verstuurd naar
		het nationaal secretariaat.</p>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Tabblad_Iedereen.png") %>" alt="Tabblad 'Iedereen'" />
	<p>
		Wat kun je hier doen?</p>
	<ul>
		<li>
			<%=Html.ActionLink("Een nieuwe persoon toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuwePersoon" })%></li>
		<li>
			<%=Html.ActionLink("Een zus/broer toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "ZusBroer" }, new { title = "Iemands adres en gezinsgebonden communicatievormen kopiëren voor een broer of zus" })%>
			van een geregistreerde persoon</li>
		<li>
			<%=Html.ActionLink("De persoonsfiche bekijken", "ViewTonen", new { controller = "Handleiding", helpBestand = "Persoonsfiche" })%></li>
		<li>
			<%=Html.ActionLink("Iemand inschrijven als lid/leiding", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuwLid" })%>
			(enkel als alle nodige info ingevuld is)</li>
		<li>
			<%= Html.ActionLink("Verschillende mensen tegelijk inschrijven", "ViewTonen", "Handleiding", null, null, "MeerdereMensen", new { helpBestand = "NieuwLid" }, null)%></li>
		<li>
			<%= Html.ActionLink("Verschillende mensen tegelijk in een categorie stoppen", "ViewTonen", "Handleiding", null, null, "MeerdereMensen", new { helpBestand = "Categoriseren" }, null)%></li>
		<li>
			<%= Html.ActionLink("Filteren op categorie", "ViewTonen", "Handleiding", null, null, "Categorie", new { helpBestand = "Filteren" }, null)%></li>
		<li>
			<%=Html.ActionLink("Een lijst downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" })%>
			(bv. om
			<%=Html.ActionLink("etiketten te maken", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenMaken" })%>)</li>
	</ul>
</asp:Content>
