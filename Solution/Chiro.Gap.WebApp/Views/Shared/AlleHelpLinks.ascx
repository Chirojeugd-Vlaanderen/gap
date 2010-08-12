<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HandleidingModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%
	// Als er een groepID in het model zit, kunnen we de default masterpage tonen. Anders hebben we een aangepaste nodig.
	// De action method bepaalt welke masterpage we gebruiken.
	var actie = Model.GroepID > 0 ? "BestandTonen" : "ByPassIndex";
%>
<div id="helpmenu">
	<h2>
		Handleiding</h2>
	<p>
		Algemeen:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Trefwoorden", actie, new { Controller = "Handleiding", helpBestand = "Trefwoorden" }) %></li>
	</ul>
	<p>
		Info per tabblad:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Jaarovergang", actie, new { Controller = "Handleiding", helpBestand = "Jaarovergang" }) %></li>
		<li>
			<%=Html.ActionLink("Ingeschreven", actie, new { Controller = "Handleiding", helpBestand = "Ingeschreven" }) %></li>
		<li>
			<%=Html.ActionLink("Iedereen", actie, new { Controller = "Handleiding", helpBestand = "Iedereen" }) %></li>
		<li>
			<%=Html.ActionLink("Groep", actie, new { Controller = "Handleiding", helpBestand = "Groep" }) %></li>
	</ul>
	<p>
		Nieuwe gegevens toevoegen:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Afdeling", actie, new { Controller = "Handleiding", helpBestand = "NieuweAfdeling" }) %></li>
		<li>
			<%=Html.ActionLink("Categorie", actie, new { Controller = "Handleiding", helpBestand = "NieuweCategorie" }) %></li>
		<li>
			<%=Html.ActionLink("Functie", actie, new { Controller = "Handleiding", helpBestand = "NieuweFunctie" }) %></li>
		<li>
			<%=Html.ActionLink("Lid/leiding", actie, new { Controller = "Handleiding", helpBestand = "NieuwLid" }) %></li>
		<li>
			<%=Html.ActionLink("Persoon", actie, new { Controller = "Handleiding", helpBestand = "NieuwePersoon" }) %></li>
	</ul>
	<p>
		Gegevens aanpassen:</p>
	<ul>
		<li>
			<%=Html.ActionLink("Persoonlijke identificatiegegevens", actie, new { Controller = "Handleiding", helpBestand = "PersoonlijkeGegevensfiche" })%></li>
		<li>
			<%=Html.ActionLink("Andere persoonsgegevens", actie, new { Controller = "Handleiding", helpBestand = "Persoonsfiche" })%></li>
	</ul>
</div>
