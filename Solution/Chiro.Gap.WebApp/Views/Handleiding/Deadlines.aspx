<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Deadlines</h2>
		<p>Er zijn een aantal deadlines die je als <abbr title="groepsadministratieverantwoordelijke">GAV</abbr> in de gaten moet houden.</p>
		<ol>
			<li>De jaarovergang: tussen eind augustus en 15 oktober <%=Html.ActionLink("[handleiding]", "ViewTonen", new { controller = "Handleiding", helpBestand = "JaarovergangUitvoeren" }, new { title = "Handleiding: jaarovergang uitvoeren" })%></li>
			<li>Tentenaanvraag (niet via <abbr title="het Groepsadministratieprogramma/deze website">GAP</abbr>): vanaf 1 oktober <a href="http://www.chiro.be/actie/bivak/bivakmateriaal#uldk" title="Handleiding tentenaanvraag">[handleiding]</a></li>
			<li>De bivakaangifte: voor 1 juni <%=Html.ActionLink("[handleiding]", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" }, new { title = "Handleiding: uitstap toevoegen" })%></li>
		</ol>
		<br />
		<p>We herinneren je op de volgende manieren aan die deadlines:</p>
		<ul>
			<li>Vermeldingen in De Mix, onze tweemaandelijkse nieuwsbrief voor groepsleiding en VB's</li>
			<li>De startpagina van de GAP-website (en dus ook van de handleiding) vermeldt bovenaan welke deadline nadert</li>
			<li>Er staat een melding in het bruine kadertje rechts onderaan</li>
		</ul>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
	Handleiding: deadlines
</asp:Content>
