﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Trefwoorden
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Trefwoorden</h2>
	<dl class="Woordenlijst">
		<dt><strong><a class="anchor" id="Aansluiting">Aansluiting</a></strong>:</dt>
		<dd>Je bent aangesloten bij Chirojeugd Vlaanderen wanneer een groep, gewest, verbond
			of nationale vrijwilligersploeg je inschrijft en je instapperiode <a href="#Instapperiode">
				[?]</a> is verstreken. Een inschrijving kan ongedaan gemaakt worden in de
			loop van het werkjaar, een aansluiting niet</dd>
		<dt><strong><a class="anchor" id="AD-nummer">AD-nummer</a></strong>:</dt>
		<dd>Iedereen die in de administratie van het nationaal Chirosecretariaat terechtkomt,
			krijgt een administratief identificatienummer. Je hebt dat nodig als je bepaalde
			zaken wilt op-of aanvragen, en Chirojeugd Vlaanderen gebruikt het om je makkelijk
			terug te vinden.</dd>
		<dt><strong><a class="anchor" id="Categorie">Categorie</a></strong>:</dt>
		<dd>Een categorie is een label dat je zelf kunt kiezen. Geef ze een naam (bv. 'kookploeg')
			en een afkorting (bv. 'KP' of 'kok'). Mensen die je aan een categorie toevoegt,
			kun je makkelijk terugvinden door in het personenoverzicht (klik bovenaan rechts
			op 'Iedereen') te selecteren op categorie.</dd>
		<dt><strong><a class="anchor" id="Chiroleeftijd">Chiroleeftijd</a></strong>:</dt>
		<dd>Bij de jaarovergang worden leden automatisch in de afdeling gestopt die overeenkomt
			met hun geboortejaar. Voor leden die een jaar zijn blijven zitten of om een
			andere reden niet bij degenen van hun leeftijd zitten, wil je dat niet. Pas
			voor hen de Chiroleeftijd aan: bv. -1 voor iemand die een jaar is blijven zitten,
			dan gaat hij of zij altijd mee met degenen die een jaar jonger zijn.
			<%=Html.ActionLink("Hoe doe je dat?", "ViewTonen", new { controller = "Handleiding", helpBestand = "ChiroleeftijdAanpassen" })%>
		</dd>
		<dt><strong><a class="anchor" id="Factuur">Factuur</a></strong>:</dt>
		<dd>Op geregelde tijdstippen verzamelt het nationaal secretariaat alle bewerkingen
			die groepen deden en waarvoor er betaald moet worden. De eerste keer gebeurt
			dat na 15 oktober, de uiterste datum waarop je de jaarovergang moet uitvoeren.
			Daarna is dat om de zoveel weken - het kan dus wel een tijdje duren voor je
			financieel verantwoordelijke zo'n factuur effectief in de bus krijgt.<br />
			Je kunt facturen krijgen voor aansluitingen, Dubbelpuntabonnementen en bijkomende
			verzekeringen. De prijs van abonnementen en verzekeringen staat altijd ergens
			vermeld - zo kom je niet voor verrassingen te staan.</dd>
		<dt><strong><a class="anchor" id="GAV">GAV</a></strong>:</dt>
		<dd>Groepsadministratieverantwoordelijke. Aangezien jij deze website gebruikt, ben
			je een GAV van jouw groep. Een groep kan zoveel GAV's hebben als je zelf wilt.
			Ze moeten wel allemaal een eigen login hebben voor deze website.</dd>
		<dt><strong><a class="anchor" id="GezinsgebondenCommunicatievorm">Gezinsgebonden communicatievormen</a></strong>:</dt>
		<dd>Een communicatievorm is bijvoorbeeld een telefoonnummer of een mailadres. Je
			kunt die allemaal met hetzelfde formulier toevoegen. Zo'n communicatievorm is
			gezinsgebonden als je er alle gezinsleden mee kunt bereiken - en dus ook de
			ouders van je leden, bijvoorbeeld. Een typisch voorbeeld is een telefoonnummer
			van een vaste lijn, maar je kunt ook het gsm-nummer van vader of moeder opgeven
			als gezinsgebonden voor je leden.</dd>
		<dt><strong><a class="anchor" id="Inschrijven">Inschrijven</a></strong>:</dt>
		<dd>Mensen die je inschrijft, maak je lid van je groep. Je vindt hen op het tabblad
			'Ingeschreven', en ze zijn verzekerd bij Chiroactiviteiten.<br />
			Iedereen krijgt bij de inschrijving een instapperiode <a href="#Instapperiode">[?]</a>.
			Pas nadat die verlopen is, word je effectief aangesloten bij Chirojeugd Vlaanderen,
			en kan het nationaal secretariaat daarvoor een factuur <a href="#Factuur">[?]</a>
			opmaken.</dd>
		<dt><strong><a class="anchor" id="Inschrijvingsvoorwaarden">Inschrijvingsvoorwaarden</a></strong>:</dt>
		<dd>Je kunt enkel mensen inschrijven die aan alle voorwaarden voldoen, dus zie je
			in het personenoverzicht (tabblad 'Iedereen') alleen bij die mensen een link
			waarmee dat kan. Voorwaarden zijn: nog niet ingeschreven zijn in het lopende
			werkjaar (evident), de geboortedatum moet ingevuld zijn en de persoon moet de
			minimumleeftijd hebben om lid te mogen worden. De Chiro sluit namelijk geen
			kleuters aan - zie <a href="http://www.chiro.be/minimumleeftijd" title="Uitleg over de minimumleeftijd voor Chiroleden">
				de Chirosite</a>.</dd>
		<dt><strong><a class="anchor" id="Instapperiode">Instapperiode</a></strong>:</dt>
		<dd>Iedereen die je inschrijft in je groep krijgt een instapperiode. De einddatum
			vind je op de persoonsfiche. De GAV's krijgen een mailtje wanneer er zo'n einddatum
			nadert. Is de einddatum verstreken, dan geldt die persoon als definitief ingeschreven.
			Op dat moment wordt hij of zij aangesloten bij Chirojeugd Vlaanderen. Dat betekent
			dat je groep een factuur zal krijgen voor zijn of haar lidgeld.</dd>
		<dt><strong><a class="anchor" id="Ledenuitgave">Ledenuitgave</a></strong>:</dt>
		<dd>Chirojeugd Vlaanderen maakt verschillende publicaties voor leden. Voor de jongste
			afdelingen sturen we een boekje of een spel naar de afdelingsleiding, keti's
			en aspi's krijgen Kramp thuisbezorgd.<br />
			Wanneer je een afdeling
			<%=Html.ActionLink("activeert voor het huidige werkjaar", "ViewTonen", new { controller = "Handleiding", helpBestand = "AfdelingActiveren" })%>
			moet je aangeven met welke offici&euml;le afdeling ze overeenkomt. Zo weet het
			nationaal secretariaat welke uitgave we moeten opsturen, en zo krijgt je hele
			afdeling dezelfde publicatie.</dd>
		<dt><strong><a class="anchor" id="Lid">Lid</a></strong>:</dt>
		<dd>Een persoon die ingeschreven is in de groep en (na de instapperiode <a href="#Instapperiode">
			[?]</a>) dus aangesloten <a href="#Aansluiting">[?]</a> bij Chirojeugd Vlaanderen</dd>
		<dt><strong><a class="anchor" id="Lidgeld">Lidgeld</a></strong>:</dt>
		<dd>Op de persoonsfiche van ingeschreven leden kun je aanduiden of zij hun lidgeld
			al betaald hebben. Dat is informatie voor je groep, niet voor het nationaal
			secretariaat. Lidgeld is wat je leden aan je groep betalen om aan de activiteiten
			te mogen deelnemen. Je groep betaalt aan Chirojeugd Vlaanderen voor de aansluiting
			en dus de verzekering. Iedereen die ingeschreven is, is verzekerd. Iedereen
			die ingeschreven is én voor wie de instapperiode verstreken is, is aangesloten.</dd>
		<dt><strong><a class="anchor" id="Persoon">Persoon</a></strong>:</dt>
		<dd>In deze handleiding bedoelen we met een persoon om het even wie van wie je gegevens
			wilt bijhouden in je groepsadministratie. Dat zijn in de eerste plaats de mensen
			die ingeschreven moeten worden in je groep: je leden en je leiding. Maar je
			kunt ook andere (contact)gegevens bijhouden: van je kookploeg, oud-leiding,
			contactpersonen uit de jeugdraad of het parochieteam, noem maar op. Die informatie
			is dan ineens beschikbaar voor alle <a href="#GAV">GAV</a>'s van je groep. En
			wil je al die mensen makkelijk terugvinden tussen die honderden personen? Stop
			ze dan in duidelijke <a href="#Categorie">categorieën</a>.</dd>
		<dt><strong><a class="anchor" id="Snelleberichtenlijsten">Snelleberichtenlijsten</a></strong>:</dt>
		<dd>Chirojeugd Vlaanderen heeft een aantal mailinglijsten. We gebruiken die niet
			om je te overstelpen met mails, wel om je snel te kunnen bereiken als we dringend
			nieuws hebben. Zo zijn er lijsten voor leiding, kaderleiding, groepsleiding,
			VB's en jeugdraadvertegenwoordigers. Meer info vind je <a href="http://www.chiro.be/Snelleberichtenlijst">
				op de Chirosite</a>.</dd>
		<dt><strong><a class="anchor" id="Voorkeursadres">Voorkeursadres</a></strong>:</dt>
		<dd>Elke persoon kan verschillende adressen hebben, van verschillende types (thuis,
			kot, enz.). Het voorkeursadres is waar de post moet aankomen. Dat is dus het
			adres dat in het Excel-bestand staat dat je kunt downloaden, en dat doorgestuurd
			wordt naar het nationaal secretariaat op het moment dat je aangesloten wordt.</dd>
	</dl>
</asp:Content>