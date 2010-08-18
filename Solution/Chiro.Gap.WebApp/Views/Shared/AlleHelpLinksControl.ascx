<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="helpmenu">
	<h2>
		Handleiding</h2>
	<h3>
		Algemeen:</h3>
	<ul>
		<li>
			<%=Html.ActionLink("Trefwoorden", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Trefwoorden" }) %></li>
	</ul>
	<h3>
		Info per tabblad:</h3>
	<ul>
		<li>
			<%=Html.ActionLink("Jaarovergang", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Jaarovergang" }) %></li>
		<li>
			<%=Html.ActionLink("Ingeschreven", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Ingeschreven" }) %></li>
		<li>
			<%=Html.ActionLink("Iedereen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Iedereen" }) %></li>
		<li>
			<%=Html.ActionLink("Groep", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Groep" }) %></li>
	</ul>
	<h3>
		Nieuwe gegevens toevoegen:</h3>
	<ul>
		<li>
			<%=Html.ActionLink("Afdeling", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweAfdeling" }) %></li>
		<li>
			<%=Html.ActionLink("Categorie", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweCategorie" }) %></li>
		<li>
			<%=Html.ActionLink("Functie", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweFunctie" }) %></li>
		<li>
			<%=Html.ActionLink("Lid/leiding", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwLid" }) %></li>
		<li>
			<%=Html.ActionLink("Persoon", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwePersoon" }) %></li>
		<li>
			<%=Html.ActionLink("Adres", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwAdres" }) %></li>
		<li>
			<%=Html.ActionLink("Straatnaam", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweStraatnaam" }) %></li>
	</ul>
	<h3>
		Gegevens aanpassen:</h3>
	<ul>
		<li>
			<%=Html.ActionLink("Persoonlijke identificatiegegevens", "ViewTonen", new { Controller = "Handleiding", helpBestand = "PersoonlijkeGegevensfiche" })%></li>
		<li>
			<%=Html.ActionLink("Andere persoonsgegevens", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Persoonsfiche" })%></li>
		<li>
			<%=Html.ActionLink("Adressen aanpassen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Verhuizen" })%></li>
		<li>
			<%=Html.ActionLink("Iemands functies aanpassen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "IemandsFunctiesAanpassen" })%></li>
		<li>
			<%=Html.ActionLink("Mensen in categorieën stoppen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Categoriseren" })%></li>
		<li>
			<%=Html.ActionLink("Iemand uitschrijven", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Uitschrijven" })%></li>
		<li>
			<%=Html.ActionLink("Groepen fusioneren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Fusioneren" })%></li>
	</ul>
	<h3>
		Aanvragen:</h3>
	<ul>
		<li>
			<%=Html.ActionLink("Verzekering loonverlies", "ViewTonen", new { Controller = "Handleiding", helpBestand = "VerzekeringLoonverlies" })%></li>
		<li>
			<%=Html.ActionLink("Dubbelpunt", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Dubbelpuntabonnement" })%></li>
	</ul>
	<h3>
		Gegevens opzoeken en gebruiken:</h3>
	<ul>
		<li>
			<%=Html.ActionLink("Overzichten filteren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Filteren" })%></li>
		<li>
			<%=Html.ActionLink("Lijsten downloaden", "ViewTonen", new { Controller = "Handleiding", helpBestand = "LijstDownloaden" })%></li>
		<li>
			<%=Html.ActionLink("Etiketten maken", "ViewTonen", new { Controller = "Handleiding", helpBestand = "EtikettenMaken" })%></li>
	</ul>
</div>
