<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Verhuizen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Verhuizen</h2>
	<p>
		Zit je met een adreswijziging, dan kun je in principe het oude adres verwijderen
		en een nieuw toevoegen. Het programma kijkt na of er nog andere mensen op dat
		adres wonen, en je krijgt de mogelijkheid om ook voor hen dat oude adres te
		verwijderen. Dat betekent dan wel dat je voor verschillende mensen een nieuw
		adres moet toevoegen.</p>
	<p>
		Gelukkig is er een eenvoudiger procedure: verhuizen. Ook dan kijkt het programma
		na of er nog andere mensen op het oude adres wonen, én je kunt ze allemaal ineens
		verhuizen.</p>
	<p>
		Stappen in de procedure:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
		<li>Klik daar op de naam van iemand die je wilt verhuizen. Je komt dan op de persoonsfiche.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Verhuizen.png") %>" alt="Iemand verhuizen vanop de persoonsfiche" />
	<ul>
		<li>Klik achter het oude adres op de link 'verhuizen'. Verhuist iemand van kot,
			dan klik je op de link achter het kotadres, enz. Je komt dan op het verhuisformulier.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Formulier_Verhuizen.png") %>"
		alt="Verhuisformulier" />
	<ul>
		<li>Bovenaan zie je iedereen die op dat adres woont. Vink aan wie er allemaal mee
			verhuist. Vul onderaan het nieuwe adres in en klik op de knop 'Bewaren'.
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
