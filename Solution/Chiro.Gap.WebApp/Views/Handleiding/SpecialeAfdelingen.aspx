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
		Elke afdeling in je groep moet gekoppeld zijn aan een officiële. Zo weet het
		nationaal secretariaat welke ledenuitgave
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Ledenuitgave", new { helpBestand = "Trefwoorden" }, new{ title="Wat is een ledenuitgave?"}) %>
		we moeten opsturen voor die leden.</p>
	<p>
		Sommige groepen hebben een aparte afdeling voor mensen met een (mentale) handicap.
		Dat is één mogelijke uitwerking van het <a href="http://www.chiro.be/toegankelijkheidchirovisie">
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
		bijvoorbeeld. Daarvoor dient 'Speciaal geval'. Niet om </p>
</asp:Content>
