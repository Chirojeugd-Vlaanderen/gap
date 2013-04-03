<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Nieuw adres
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuw adres toevoegen</h2>
	<p>
		Een persoon kan meerdere adressen hebben. Eén daarvan is het voorkeursadres.
		Op de persoonsfiche kun je dat zien: het is vetgedrukt. Dat is ook het adres
		dat in de lijst staat die je downloadt, en dat doorgestuurd wordt naar het nationaal
		secretariaat wanneer je iemand aansluit.</p>
	<p>
		Stappen in de procedure:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
		<li>Klik daar op de naam van degene voor wie je een adres wilt toevoegen. Je komt
			dan op de persoonsfiche.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_nieuwadreslink.png") %>"
		alt="Link 'nieuw adres' op Persoonsfiche" />
	<ul>
		<li>Klik op de link 'adres toevoegen'. Je komt dan op een formuliertje.</li>
	</ul>
	<img src="<%= ResolveUrl("~/Content/Screenshots/Formulier_Nieuw_adres.png") %>"
		alt="Staatnaamsuggestie" />
	<ul>
		<li>Vul de nodige gegevens in.
			<ul>
				<li>Het type adres: thuis, kot, enz.</li>
				<li>Het adres zelf: zodra je de straatnaam begint te typen, krijg je een suggestielijstje
					op basis van de postcode</li>
				<li>Je kunt aangeven of het als voorkeursadres opgeslagen moet worden</li>
			</ul>
		</li>
		<li>Klik op 'Bewaren'.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je de nodige aanpassingen nog kunt doen.</li>
				<li class="goed">Als er geen problemen meer zijn, krijg je een melding dat de gegevens
					opgeslagen zijn.</li>
			</ul>
		</li>
	</ul>
	<p>
		<strong>Opgelet:</strong> het programma controleert of je een geldige combinatie
		van straat en gemeente invult. Is dat niet het geval, dan heb je twee mogelijkheden.
		Ofwel zit de straat anders in onze databank, ofwel zit ze er niet in. We gebruiken
		alleen de officiële schrijfwijze. Vraag bij de inschrijvingen dus dat het adres
		correct ingevuld wordt. Het is bijvoorbeeld mogelijk dat de Hendrickxstraat
		die iedereen kent officieel Burgemeester Hendrickxstraat heet. Zit je met een
		straatnaam die je echt niet terugvindt, dan kun je
		<%= Html.ActionLink("vragen aan het nationaal secretariaat dat ze de straatnamenlijst aanvullen", 
			"ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweStraatnaam" })%>.</p>
</asp:Content>
