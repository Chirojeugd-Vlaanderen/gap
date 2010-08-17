<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Nieuwe straatnaam
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuwe straatnaam toevoegen</h2>
	<p>
		In onze databank zit een lijst met de officiële straatnamen van België. Bij
		elk nieuw adres dat toegevoegd wordt, controleert het programma of de straatnaam
		bestaat en of ze voorkomt in de gemeente die je invulde. Zo willen we een hoop
		fouten en misverstanden vermijden.</p>
	<p>
		Zit je nu met een nieuwe straat, die nog niet in onze lijst voorkomt, dan moet
		je vragen aan het nationaal secretariaat dat ze ze toevoegen. Daar wordt dan
		eerst nagekeken of het toch niet om een bestaande straat gaat, en of de straat
		echt bestaat. Pas dan voegen we de nieuwe straatnaam toe. Het kan dus wel een
		tijdje duren voor je ze effectief kunt gebruiken.</p>
	<p>
		Welke gegevens moet je zeker doorgeven?</p>
	<ul>
		<li>De <strong>volledige</strong> straatnaam, volledig <strong>juist gespeld</strong></li>
		<li>Het postnummer en de naam van de gemeente</li>
	</ul>
	<p>
		Stuur die gegevens door via het <a href="http://drupal.chiro.be/eloket/gap-straatnaam">
			e-loket</a>.</p>
	<p>
		Het secretariaat brengt je op de hoogte zodra ze je verzoek behandeld hebben.</p>
</asp:Content>
