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
		<li>Jaarovergang</li>
		<li>Ingeschreven</li>
		<li>Iedereen</li>
		<li>Groep</li>
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
</div>
