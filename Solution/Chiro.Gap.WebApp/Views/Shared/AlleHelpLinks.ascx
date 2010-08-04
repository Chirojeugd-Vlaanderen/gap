<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="helpmenu">
	<h2>
		Inhoud</h2>
	<p>
		Algemeen:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Trefwoorden", "BestandTonen", new { Controller = "Handleiding", helpBestand = "Trefwoorden" }) %></li>
	</ul>
	<p>
		Info per tabblad:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Jaarovergang", "BestandTonen", new { Controller = "Handleiding", helpBestand = "Jaarovergang" }) %></li>
		<li>
			<%=Html.ActionLink("Ingeschreven", "BestandTonen", new { Controller = "Handleiding", helpBestand = "Ingeschreven" }) %></li>
		<li>
			<%=Html.ActionLink("Iedereen", "BestandTonen", new { Controller = "Handleiding", helpBestand = "Iedereen" }) %></li>
		<li>
			<%=Html.ActionLink("Groep", "BestandTonen", new { Controller = "Handleiding", helpBestand = "Groep" }) %></li>
	</ul>
	<p>
		Nieuwe gegevens toevoegen:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Afdeling", "BestandTonen", new { Controller = "Handleiding", helpBestand = "NieuweAfdeling" }) %></li>
		<li>
			<%=Html.ActionLink("Categorie", "BestandTonen", new { Controller = "Handleiding", helpBestand = "NieuweCategorie" }) %></li>
		<li>
			<%=Html.ActionLink("Functie", "BestandTonen", new { Controller = "Handleiding", helpBestand = "NieuweFunctie" }) %></li>
		<li>
			<%=Html.ActionLink("Lid/leiding", "BestandTonen", new { Controller = "Handleiding", helpBestand = "NieuwLid" }) %></li>
		<li>
			<%=Html.ActionLink("Persoon", "BestandTonen", new { Controller = "Handleiding", helpBestand = "NieuwePersoon" }) %></li>
	</ul>
	<p>
		Gegevens aanpassen:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Persoonlijke identificatiegegevens", "BestandTonen", new { Controller = "Handleiding", helpBestand = "PersoonlijkeGegevensfiche" })%></li>
		<li>
			<%=Html.ActionLink("Andere persoonsgegevens", "BestandTonen", new { Controller = "Handleiding", helpBestand = "Persoonsfiche" })%></li>
	</ul>
</div>
