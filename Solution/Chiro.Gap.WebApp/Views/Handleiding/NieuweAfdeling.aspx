<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Nieuwe afdeling
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuwe afdeling toevoegen</h2>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Groep'</li>
		<li>Klik op de link 'afdelingsverdeling aanpassen'</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_afdelingsverdeling_aanpassen.png") %>"
		alt="De link om de afdelingsverdeling aan te passen, vind je op het tabblad 'Groep'" />
	<ul>
		<li>Klik op de link 'Nieuwe afdeling'. Je komt dan op het formuliertje.</li></ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Nieuwe_afdeling.png") %>"
		alt="Een nieuwe afdeling toevoegen" />
	<ul>
		<li>Vul de naam en een afkorting in, en klik op de knop 'Bewaren'
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je nog zaken kunt aanpassen. Het kan bv. zijn dat er al een afdeling met die
					naam of die afkorting bestaat.</li>
				<li class="goed">Als er geen problemen meer zijn, kom je op het scherm met de afdelingsverdeling.
					Je nieuwe afdeling staat onderaan bij de niet-actieve.</li>
			</ul>
		</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Overzicht_afdelingen.png") %>"
		alt="De afdelingsverdeling" />
	<ul>
		<li>Klik naast je nieuwe afdeling op de link 'Activeren in huidig werkjaar'. Je
			komt dan weer op een formuliertje.</li></ul>
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
					je nog zaken kunt aanpassen. Het kan bijvoorbeeld zijn dat je geboortejaren
					invulde die niet overeenkomen met die van de corresponderende offici&euml;le
					afdeling. Je mag bijvoorbeeld wel drie jaar speelclub hebben, maar minstens
					één van die drie geboortejaren moet bij de offici&euml;le speelclub staan. Of
					misschien hield je geen rekening met <a href="http://www.chiro.be/minimumleeftijd"
						title="Uitleg over de minimumleeftijd voor Chiroleden">de minimumleeftijd</a>:
					de Chiro sluit geen kleuters aan.</li>
				<li class="goed">Als er geen problemen meer zijn, kom je op het scherm met de afdelingsverdeling.
					Je nieuwe afdeling staat bovenaan bij de actieve.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
