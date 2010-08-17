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
		<li>Klik op de link 'Nieuwe afdeling'</li>
		<li>Vul de naam en een afkorting in, en klik op de knop 'Bewaren'
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je nog zaken kunt aanpassen. Het kan bv. zijn dat er al een afdeling met die
					naam of die afkorting bestaat.</li>
				<li class="goed">Als er geen problemen meer zijn, kom je op het scherm met de afdelingsverdeling.
					Je nieuwe afdeling staat onderaan bij de niet-actieve.</li>
			</ul>
		</li>
		<li>Klik naast je nieuwe afdeling op de link 'Activeren in huidig werkjaar'</li>
		<li>Vul de geboortejaren en het geslacht in, geef aan met welke offici&euml;le afdeling
			ze overeenkomt en klik op de knop 'Bewaren'
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je nog zaken kunt aanpassen. Het kan bv. zijn dat je geboortejaren ingevuld
					hebt die ver afwijken van die van de offici&euml;le afdeling waar ze mee moet
					overeenkomen, of dat je geen rekening hield met <a href="http://www.chiro.be/minimumleeftijd"
						alt="Uitleg over de minimumleeftijd voor Chiroleden">de minimumleeftijd</a>.</li>
				<li class="goed">Als er geen problemen meer zijn, kom je op het scherm met de afdelingsverdeling.
					Je nieuwe afdeling staat bovenaan bij de actieve.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
