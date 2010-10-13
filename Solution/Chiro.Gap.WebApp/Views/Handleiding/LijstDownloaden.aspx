<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Lijst downloaden
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een lijst downloaden</h2>
	<p>
		De gegevens van je groep worden natuurlijk pas echt bruikbaar als je er meer
		mee kunt doen dan ze alleen online bekijken. Daarom kun je allerlei lijstjes
		downloaden als Excel-bestand. Je kunt ze dan openen in Excel van Microsoft,
		Calc van OpenOffice.org, en misschien ook nog in andere spreadsheetprogramma's.
		Heb je geen programma waarmee je het bestand kunt openen? Calc kun je <a href="http://nl.openoffice.org/downloaden.html">
			gratis downloaden</a>.</p>
	<p>
		De lijsten die je kunt downloaden, zijn opgemaakt in het formaat van Office
		2007. Als je Office 2003 hebt en je krijgt het bestand niet open, dan moet je
		eerst een <a href="http://www.microsoft.com/downloads/details.aspx?familyid=941b3470-3ae9-4aee-8f43-c6bb74cd1466&displaylang=nl"
			title="Hulpprogramma om Office 2007-bestanden te kunnen openen in Office 2003">
			extra programma</a> installeren. Zorg ook dat je <a href="http://update.microsoft.com">
				de recentste updates van Office</a> geïnstalleerd hebt.</p>
	<p>
		<em>Internet Explorer geeft soms problemen. Je krijgt dan een venstertje met de
			foutmelding "Kan bestand '208' niet downloaden". We weten nog niet wat het probleem
			veroorzaakt, maar als je <a href="http://www.mozilla.com/firefox">Firefox</a>
			of <a href="http://www.google.be/chrome">Chrome</a> gebruikt in plaats van Internet
			Explorer lukt de download wél.</em></p>
	<p>
		Lijsten die je kunt downloaden:</p>
	<ul>
		<li>Alle personen
			<ul>
				<li>Klik op het tabblad 'Iedereen'</li>
				<li>Klik op de link 'Lijst downloaden'</li>
			</ul>
		</li>
		<li>Alle ingeschreven leden
			<ul>
				<li>Klik op het tabblad 'Ingeschreven'</li>
				<li>Klik op de link 'Lijst downloaden'</li>
			</ul>
		</li>
		<li>De leden en leiding van één bepaalde afdeling
			<ul>
				<li>Klik op het tabblad 'Ingeschreven'</li>
				<li>Selecteer rechts van de tabel de afdeling die je nodig hebt (en klik eventueel
					nog op de knop 'Afdeling bekijken' - als die er niet staat, wordt de lijst automatisch
					gefilterd)</li>
				<li>Even wachten tot de lijst gefilterd is...</li>
				<li>Klik op de link 'Lijst downloaden'</li>
			</ul>
		</li>
		<li>De personen in een bepaalde categorie (bv. de kookploeg)
			<ul>
				<li>Klik op het tabblad 'Iedereen'</li>
				<li>Selecteer rechts van de tabel de categorie die je nodig hebt (en klik eventueel
					nog op de knop 'Categorie bekijken' - als die er niet staat, wordt de lijst
					automatisch gefilterd)</li>
				<li>Even wachten tot de lijst gefilterd is...</li>
				<li>Klik op de link 'Lijst downloaden'</li>
			</ul>
		</li>
	</ul>
</asp:Content>
