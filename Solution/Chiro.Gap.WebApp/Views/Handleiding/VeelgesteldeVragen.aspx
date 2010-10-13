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
		<li><a href="#Uitstel">Kunnen we uitstel krijgen voor de jaarovergang/inschrijving/aansluiting?</a></li>
		<li><a href="#LateInschrijving">Onze inschrijvingsdag is kort voor/na de deadline
			van 15 oktober. Wat nu?</a></li>
		<li><a href="#Downloadfout">De download lukt niet.</a></li>
		<li><a href="#Bestandsprobleem">Ik kan het bestand wel downloaden, maar ik krijg
			het niet open.</a></li>
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
			title="Uitleg over de minimumleeftijd voor Chiroleden">de Chirosite</a>). Als het daar niet
			aan ligt, heb je waarschijnlijk leden die te jong zijn. Schrijf hen uit en probeer de jaarovergang opnieuw uit te voeren. Lukt het nog niet? Neem dan contact op met <a href="http://www.chiro.be/eloket/feedback-gap">de helpdesk</a>.</p>
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
		<a class="anchor" id="Downloadfout">De download lukt niet.</a></h3>
	<p>
		Internet Explorer ondervindt inderdaad moeilijkheden met de download. Je krijgt
		dan een apart venstertje met een foutmelding in de trant van "Kan bestand '208'
		niet downloaden". We weten nog niet wat het probleem veroorzaakt, maar als je
		<a href="http://www.mozilla.com/firefox">Firefox</a> of <a href="http://www.google.be/chrome">
			Chrome</a> gebruikt in plaats van Internet Explorer lukt de download wél.</p>
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
