<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Afdeling activeren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een afdeling activeren</h2>
	<p>
		Elke afdeling die je ooit gebruikt, blijft bestaan voor jouw groep. Je kunt
		ze dus terugvinden, en je kunt in voorbije werkjaren nagaan wie er allemaal
		lid van was. Zo kan het zijn dat je één jaar een gemengde rakwi-afdeling had,
		maar het jaar erop twee niet-gemengde afdelingen voor diezelfde leeftijd: rakkers
		en kwiks.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Groep'</li>
		<li>Klik op de link 'afdelingsverdeling aanpassen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_afdelingsverdeling_aanpassen.png") %>"
		alt="De link om de afdelingsverdeling aan te passen, vind je op het tabblad 'Groep'" />
	<ul>
		<li>Je krijgt nu een overzichtje van de afdelingen die je dit werkjaar hebt, en
			onderaan van de afdelingen die je ooit toevoegde maar die je nog niet activeerde
			dit werkjaar.</li></ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Overzicht_afdelingen.png") %>"
		alt="De afdelingsverdeling" />
	<ul>
		<li>Klik naast zo'n niet-actieve afdeling op de link 'Activeren in huidig werkjaar'.
			Je gaat dan naar een formuliertje waar je bijkomende gegevens moet invullen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Afdeling_activeren.png") %>" alt="Een afdeling activeren" />
	<ul>
		<li>Onderaan vind je de 'offici&euml;le' afdelingen en de geboortejaren die daarmee
			overeenkomen. Je hoeft je daar niet aan te houden, ze staan er gewoon als hulp.
			Eén van de keuzes die je moet maken, is wel met welke offici&euml;le afdeling
			de jouwe overeenkomt. Zo weet het nationaal secretariaat welke ledenuitgave
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Ledenuitgave", new { helpBestand = "Trefwoorden" }, new{ title="Wat is een ledenuitgave?"}) %>
			we moeten opsturen voor die leden.</li>
		<li>Wanneer je de nodige gegevens ingevuld hebt, klik je op de knop 'Bewaren'.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je de nodige aanpassingen nog kunt doen. Het kan bijvoorbeeld zijn dat je geboortejaren
					invulde die niet overeenkomen met die van de corresponderende offici&euml;le
					afdeling. Je mag bijvoorbeeld wel drie jaar speelclub hebben, maar minstens
					één van die drie geboortejaren moet bij de offici&euml;le speelclub staan.</li>
				<li class="goed">Als er geen problemen meer zijn, keer je automatisch terug naar
					het overzicht van je afdelingen. De geactiveerde afdeling staat nu ook bovenaan.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
