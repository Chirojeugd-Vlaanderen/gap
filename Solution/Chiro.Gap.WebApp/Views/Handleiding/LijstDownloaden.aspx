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
		Calc van OpenOffice.org of LibreOffice, en misschien ook nog in andere spreadsheetprogramma's.
		Heb je geen programma waarmee je het bestand kunt openen? Calc is gratis,
		zowel in de versie van <a href="http://nl.openoffice.org/downloaden.html">OpenOffice.org</a>
		of als in die van <a href="http://www.libreoffice.org/download/">LibreOffice</a>.</p>
	<p>
		Microsoft Office voor Mac kan blijkbaar problemen hebben met de geboortedatums, daar moet je dus mee opletten.</p>
	<p>
		De lijsten die je kunt downloaden, zijn opgemaakt in het formaat van Office
		2007. Als je Office 2003 hebt en je krijgt het bestand niet open, dan moet je
		eerst een <a href="http://www.microsoft.com/downloads/details.aspx?familyid=941b3470-3ae9-4aee-8f43-c6bb74cd1466&displaylang=nl"
			title="Hulpprogramma om Office 2007-bestanden te kunnen openen in Office 2003">
			extra programma</a> installeren. Zorg ook dat je <a href="http://update.microsoft.com">
				de recentste updates van Office</a> geïnstalleerd hebt.</p>
	<p>
		Als je in de kolom met geboortedata alleen ##### ziet staan, dan betekent 
		dat gewoon dat de kolom te smal is. Er is dus niets foutgelopen. Maak de 
		kolom wat breder om de data te zien.</p>
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
