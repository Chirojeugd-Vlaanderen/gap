<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Veelgestelde vragen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Veelgestelde vragen</h2>
	<ul>
		<li><a href="#JaarovergangFout">De jaarovergang lukt niet.</a></li>
		<li><a href="#WieVerzekerd">Hoe kan ik zien wie er allemaal verzekerd is?</a></li>
		<li><a href="#WieAangesloten">Hoe kan ik zien wie er allemaal aangesloten is?</a></li>
		<li><a href="#Uitstel">Kunnen we uitstel krijgen voor de jaarovergang/inschrijving/aansluiting?</a></li>
		<li><a href="#LateInschrijving">Onze inschrijvingsdag is kort voor/na de deadline
			van 15 oktober. Wat nu?</a></li>
		<li><a href="#Bestandsprobleem">Ik kan wel lijsten downloaden, maar ik krijg ze
			niet open.</a></li>
		<li><a href="#ExcelOpmaak">Kunnen er andere/meer kolommen in het gedownloade bestand?
			Kan de opmaak anders?</a></li>
		<li><a href="#Doorsturen">Ik vind geen knop om de gegevens door te sturen naar het
			nationaal secretariaat.</a></li>
	</ul>
	<h3>
		<a class="anchor" id="JaarovergangFout">De jaarovergang lukt niet.</a></h3>
	<p>
		In de meeste gevallen heeft dat te maken met de leeftijdsgrenzen van je afdelingen.
		Je mag geen afdeling hebben voor leden die jonger zijn dan 6 jaar (zie <a href="http://www.chiro.be/minimumleeftijd"
			title="Uitleg over de minimumleeftijd voor Chiroleden">de Chirosite</a>).
		Als het daar niet aan ligt, heb je waarschijnlijk leden die te jong zijn. Schrijf
		hen uit en probeer de jaarovergang opnieuw uit te voeren. Lukt het nog niet?
		Neem dan contact op met <a href="http://www.chiro.be/eloket/feedback-gap">de helpdesk</a>.</p>
	<h3>
		<a class="anchor" id="WieVerzekerd">Hoe kan ik zien wie er allemaal verzekerd is?</a></h3>
	<p>
		Iedereen die op het tabblad 'Ingeschreven' staat, is verzekerd. Ook al is hun
		instapperiode nog niet voorbij en zijn de gegevens dus nog niet doorgestuurd
		naar het nationaal secretariaat.</p>
	<h3>
		<a class="anchor" id="WieAangesloten">Hoe kan ik zien wie er allemaal aangesloten
			is?</a></h3>
	<p>
		Iedereen die op het tabblad 'Ingeschreven' staat en niet meer in hun instapperiode
		zit, is aangesloten. De aansluiting geldt wel voor een heel werkjaar. Mensen
		die je uitschrijft, staan niet meer op het tabblad 'Ingeschreven', maar ze blijven
		wel aangesloten. Als je hen achteraf dus opnieuw inschrijft, krijg je geen nieuwe
		factuur.</p>
	<h3>
		<a class="anchor" id="Uitstel">Kunnen we uitstel krijgen voor de jaarovergang/inschrijving/aansluiting?</a></h3>
	<p>
		Op 15 oktober moet je de jaarovergang uitgevoerd hebben. Niemand krijgt uitstel.
		Die datum is al jaren een deadline: voor de verzekeringen, én voor de werking
		van het nationaal secretariaat. Dat zijn zaken waar we geen risico mee willen
		nemen.</p>
	<p>
		Dat betekent niet dat je ledenlijst al volledig in orde moet zijn. Twijfelgevallen
		schrijf je uit (bij voorkeur nog voor de jaarovergang), nieuwe leden kun je
		nog het hele jaar toevoegen.</p>
	<h3>
		<a class="anchor" id="LateInschrijving">Onze inschrijvingsdag is kort voor/na de
			deadline van 15 oktober. Wat nu?</a></h3>
	<p>
		Op 15 oktober moet de jaarovergang uitgevoerd zijn. Dat betekent niet dat je
		leden allemaal aangesloten worden op die dag. Doe je de jaarovergang op 14 oktober,
		dan krijgen je automatisch ingeschreven leden een instapperiode
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een instapperiode?" } ) %>
		tot 4 november. Je hebt dus tot 4 november de tijd om diegenen uit te schrijven
		die niet meer komen. Nieuwe leden kun je het hele jaar door toevoegen en inschrijven.</p>
	<h3>
		<a class="anchor" id="Bestandsprobleem">Ik kan het bestand wel downloaden, maar
			ik krijg het niet open.</a></h3>
	<p>
		De lijsten die je kunt downloaden, zijn opgemaakt in het formaat van Office
		2007. Als je Office 2003 hebt en je krijgt het bestand niet open, dan moet je
		eerst een <a href="http://www.microsoft.com/downloads/details.aspx?familyid=941b3470-3ae9-4aee-8f43-c6bb74cd1466&displaylang=nl"
			title="Hulpprogramma om Office 2007-bestanden te kunnen openen in Office 2003">
			extra programma</a> installeren. Zorg ook dat je <a href="http://update.microsoft.com">
				de recentste updates van Office</a> geïnstalleerd hebt.</p>
	<p>
		Heb je geen programma waarmee je het bestand kunt openen? Calc is gratis, zowel
		in de versie van <a href="http://nl.openoffice.org/downloaden.html">OpenOffice.org</a>
		of als in die van <a href="http://www.libreoffice.org/download/">LibreOffice</a>.</p>
	<p>
		Microsoft Office voor Mac kan blijkbaar problemen hebben met de geboortedatums,
		daar moet je dus mee opletten.</p>
	<h3>
		<a class="anchor" id="ExcelOpmaak">Kunnen er andere/meer kolommen in het gedownloade
			bestand? Kan de opmaak anders?</a></h3>
	<p>
		In een volgende versie willen we de download flexibeler maken, zodat er inderdaad
		extra kolommen in kunnen komen. De opmaak kunnen we niet veranderen: die is
		verschillend in verschillende programma's en voor verschillende printers. Het
		is dus de bedoeling dat je dat bestand zelf nog aanpast. Voor gevorderden: met
		macro's kun je dat wel wat automatiseren.</p>
	<h3>
		<a class="anchor" id="Doorsturen">Ik vind geen knop om de gegevens door te sturen
			naar het nationaal secretariaat.</a></h3>
	<p>
		Die knop zul je ook niet vinden. Iemand die ingeschreven is, krijgt een instapperiode.
		Na die instapperiode worden de gegevens automatisch doorgestuurd, wordt de persoon
		aangesloten en verzekerd bij Chirojeugd Vlaanderen, en zul je een factuur krijgen
		voor het lidgeld. De aanduiding 'lidgeld betaald' heeft daar niets mee te maken,
		dat is informatie voor de groep.</p>
	<h3>
		Kan ik extra logins aanvragen?</h3>
	<p>
		Ja. Vraag er één aan via <a href="http://www.chiro.be/eloket/gap">het e-loket</a>.
		<strong>Vermeld je stamnummer</strong>, zodat we weten over welke groep het
		gaat.</p>
</asp:Content>
