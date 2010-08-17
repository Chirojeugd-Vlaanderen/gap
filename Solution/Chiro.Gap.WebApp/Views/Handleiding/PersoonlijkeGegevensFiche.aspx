<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Persoonlijkegegevensfiche
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Persoonlijkegegevensfiche</h2>
	<!-- EditGegevens -->
	<h3>
		Hoe kom je hier?</h3>
	<p>
		Je komt hier als je op de tab 'Iedereen' klikt op de link 'Nieuwe persoon',
		of als je een naam aanklikt en dan op de persoonsfiche klikt op de link 'persoonlijke
		gegevens aanpassen'.</p>
	<h3>
		Wat zie je hier?</h3>
	<p>
		Hier vind je alle 'identificerende' gegevens van een persoon: eventueel een
		AD-nummer
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "AD-nummer", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een AD-nummer?" })%>,
		voornaam en naam, geboortedatum en Chiroleeftijd
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?"})%>.</p>
	<h3>
		Wat kun je hier doen?</h3>
	<p>
		Het AD-nummer kun je niet veranderen, dat wordt toegekend door het nationaal
		secretariaat. Alle andere gegevens kun je wél aanpassen.</p>
</asp:Content>
