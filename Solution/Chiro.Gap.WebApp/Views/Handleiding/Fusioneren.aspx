﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Fusioneren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Fusioneren</h2>
	<p>
		Fusioneert je groep met een andere? Laat dat dan weten aan het nationaal secretariaat,
		want zij moeten één en ander in orde brengen.</p>
	<p>
		Informatie die je moet doorgeven:</p>
	<ul>
		<li>De naam van de 'nieuwe' groep</li>
		<li>Mogen de huidige GAV's hun rechten behouden? Moeten er nieuwe/extra logins aangemaakt
			worden?</li>
		<li>Hoe is de afdelingsverdeling in de fusiegroep?</li>
		<li>Wie is contactpersoon? Wie is financieel verantwoordelijke?</li>
	</ul>
	<p>
		Taken voor het nationaal secetariaat:</p>
	<ul>
		<li>Een nieuwe groep aanmaken, met een nieuw stamnummer</li>
		<li>De nodige afdelingen aanmaken in die nieuwe groep</li>
		<li>De leden van de fusiegroepen overzetten en in de juiste afdelingen stoppen</li>
	</ul>
	<p>
		Je wordt op de hoogte gebracht wanneer die hele procedure doorlopen is. Als
		je daarna inlogt, zul je de gegevens van de fusiegroep zien.</p>
</asp:Content>