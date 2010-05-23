<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.LedenLinksModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<p class="validation-summary-errors">
		Er zijn nog leden of leid(st)ers die de functie hebben die je wilt verwijderen.
		Ben je er zeker van dat je de functie wilt verwijderen?
	</p>
	<% using (Html.BeginForm())
	{ %>
	<ul id="acties">
		<li>
			<input type="submit" value="Ja" /></li>
		<li>
			<%=Html.ActionLink("Nee", "Index")%></li>
	</ul>
	<p>
		Het gaat in totaal over
		<%=Model.TotaalAantal %>
		personen.</p>
	<%=Html.HiddenFor(mdl => mdl.FunctieID) %>
	<%} %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
