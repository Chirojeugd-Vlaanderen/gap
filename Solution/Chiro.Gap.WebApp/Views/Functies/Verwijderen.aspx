<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LedenLinksModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<p class="validation-summary-errors">
		Er zijn nog leden of leid(st)ers die in dit werkjaar de functie hebben die je wilt verwijderen.
		Als je de functie verwijdert, dan verliezen deze personen de functie voor dit werkjaar.
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
	<h3>Personen met deze functie</h3>
	<%
		Html.RenderPartial("LedenLinksControl", Model); 
		// Ik denk dat die hidden hieronder niet nodig is,
		// dat kan even goed opnieuw uit de url gehaald worden.
	     %>
	<%=Html.HiddenFor(mdl => mdl.FunctieID) %>
	<%} %>
</asp:Content>
