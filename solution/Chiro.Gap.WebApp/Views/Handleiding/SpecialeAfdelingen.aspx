<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Speciale afdelingen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Speciale afdelingen</h2>
	<p>
		Elke afdeling in je groep moet gekoppeld zijn aan een officiële. Dat is nodig
		voor de website, omdat GAP iedereen die je inschrijft automatisch in de juiste
		afdeling stopt. En het is nodig voor het nationaal secretariaat: zo weten wij
		welke ledenuitgave
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Ledenuitgave", new { helpBestand = "Trefwoorden" }, new{ title="Wat is een ledenuitgave?"}) %>
		we moeten opsturen voor die leden.</p>
	<p>
		Sommige groepen hebben een aparte afdeling voor mensen met een (mentale) handicap.
		Dat is één mogelijke uitwerking van het <a href="http://http://www.chiro.be/info-voor-leiding/diversiteit/chirovisie">
			streven naar inclusie</a> van de Chiro:
	</p>
	<blockquote>
		We willen dat elke groep inspanningen doet om toegankelijk te zijn voor alle
		kinderen en jongeren die in de buurt, de parochie, het dorp,... leven, wonen
		en spelen. Zo kunnen we als groep ernaar streven een doorsnede van de kinderen
		en jongeren uit de buurt te vormen.
	</blockquote>
	<p>
		Dat soort afdelingen heeft heel andere leeftijdsgrenzen: van 6 tot 20 jaar,
		bijvoorbeeld. We noemen dat 'speciaal' omdat die afdelingen niet meegerekend
		worden wanneer het programma nagaat in welke afdeling een nieuw lid moet terechtkomen.
		Dat dient dus niet om aan te geven dat bijvoorbeeld jouw speelclubafdeling andere
		leeftijdsgrenzen heeft dan de officiële.</p>
</asp:Content>
