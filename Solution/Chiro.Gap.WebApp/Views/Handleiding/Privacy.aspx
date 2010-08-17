<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Privacy
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Privacy</h2>
	<p>
		De website is beveiligd, dus alleen mensen met een login kunnen gegevens bekijken
		en bewerken. Je kunt ook alleen aan gegevens van een groep waar je groepsadministratieverantwoordelijke
		(GAV) voor bent – in de meeste gevallen is dat er maar één.</p>
	<p>
		Van mensen die je aansluit, worden de volgende persoonsgegevens doorgestuurd
		naar Chirojeugd Vlaanderen:</p>
	<ul>
		<li>Naam en voornaam</li>
		<li>Geboortedatum</li>
		<li>Geslacht</li>
		<li>Het voorkeursadres</li>
		<li>Het voorkeursmailadres</li>
		<li>Het voorkeurstelefoonnummer</li>
	</ul>
	<p>
		Van leden houden we bij in welke afdeling ze zitten, van leiders en leidsters
		bij welke afdeling(en) ze staan en welke officiële functies ze hebben. Officiële
		functies zijn bijvoorbeeld ‘contactpersoon’, ‘groepsleiding’ en ‘financieel
		verantwoordelijke’.</p>
	<p>
		Alle andere gegevens die je invult, zijn van je groep en zijn alleen zichtbaar
		voor de GAV’s van je groep. Chirojeugd Vlaanderen gebruikt die op geen enkele
		manier, ze worden alleen centraal opgeslagen en geback-upt.</p>
	<p>
		Als iemand uit je groep aan het nationaal secretariaat laat weten dat hij of
		zij verhuisd is, passen wij dat aan in onze administratie. Die aanpassing wordt
		dan ook doorgestuurd naar de groepsadministratiewebsite, zodat jullie ook ineens
		het juiste adres hebben.</p>
	<p>
		Chirojeugd Vlaanderen gebruikt persoonlijke gegevens enkel voor haar interne
		werking (vnl. post opsturen), voor de verzekering en voor statistische verwerking.
		Adres- en andere gegevens worden nooit doorgegeven aan derden. Als bijvoorbeeld
		studenten een enquête willen afnemen, is het het nationaal secretariaat dat
		de adresetiketten afprint en de formulieren verstuurt. Als organisaties of bedrijven
		reclame willen maken bij Chiroleden, kan dat alleen via de Chiropublicaties.</p>
</asp:Content>
