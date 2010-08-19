<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Etiketten in Writer
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Etiketten maken in OpenOffice.org Writer</h2>
	<p>
		In Writer kun je enkel etiketten maken op basis van een database ('gegevensbron').
		Je kunt het Excel-bestand wel omzetten in een database, maar daar heb je minstens
		versie 3.2 van OpenOffice.org voor nodig. Gelukkig kun je die gratis <a href="http://nl.openoffice.org/downloaden.html">
			downloaden</a>.</p>
	<p>
		Stap 1 in het proces: het database-bestand maken</p>
	<ul>
		<li>Open Base, het databaseprogramma van OpenOffice.org. Je krijgt dan eerst een
			klein venstertje te zien dat je een procedure doet doorlopen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/assistent_database_01.png") %>"
		alt="Database-assistent OpenOffice.org" />
	<ul>
		<li>Kies 'Met een bestaande database verbinden', met als bestandstype 'Werkblad'
			(in plaats van 'JDBC'). Klik op Volgende.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/assistent_database_02.png") %>"
		alt="Database-assistent OpenOffice.org" />
	<ul>
		<li>Selecteer het bestand dat je gedownload hebt.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/assistent_database_03.png") %>"
		alt="Database-assistent OpenOffice.org" />
	<ul>
		<li>Klik op Volgende en daarna op Voltooien. Nu moet je de database nog een naam
			geven - standaard staat er 'Nieuwe Database.odb' maar dat is natuurlijk geen
			goede keuze.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/assistent_database_04_Opslaan.png") %>"
		alt="Database-assistent OpenOffice.org" />
	<ul>
		<li>Je krijgt nu het database-bestand te zien. Dat mag je gewoon sluiten.</li>
	</ul>
	<p>
		Stap 2 in het proces: de etiketten maken</p>
	<ul>
		<li>Open Writer.</li>
		<li>Klik op Bestand > Nieuw > Etiketten.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/writer_Bestand_Nieuw.png") %>"
		alt="Een nieuw etikettenbestand maken" />
	<ul>
		<li>Je krijgt nu een klein venstertje te zien. Selecteer rechtsboven, onder 'Database',
			de naam die je aan het database-bestand gegeven hebt.</li>
		<li>Selecteer daaronder de naam van het werkblad. Normaal heb je maar één keuze,
			dus dat is de goede.</li>
		<li>Selecteer in het derde selectielijstje een veld dat op je etiket moet komen
			en klik op het pijltje links ervan. Vergeet het busnummer niet!<br />
			In het kadertje ernaast zie je hoe je etiket geordend is. Zo plaats je alle
			nodige velden op het etiket. Voeg de nodige spaties en regeleinden toe.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/writer_Etikettenwizard.png") %>"
		alt="Het etiket opmaken met de etiketten-assistent" />
	<ul>
		<li>Selecteer onderaan het formaat van de etiketten. (De meest gebruikte etiketten
			zijn er van 21 op een blad (3 bij 7): Standaard Avery A4/A5, formaat L7160.)</li>
		<li>Klik op de knop 'Nieuw document' om je etikettenblad te maken. Daar zie je aanvankelijk
			alleen de codes staan die naar de velden in de database verwijzen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/writer_Etikettenblad_met_codes.png") %>"
		alt="Het voorlopige etikettenblad" />
	<ul>
		<li>Klik op Bestand > Afdrukken. Je krijgt dan een klein venstertje dat je eraan
			herinnert dat er database-velden in je bestand staan, en het vraagt of je een
			standaardbrief wilt afdrukken. De vraag is wat ongelukkig, maar ja, dat is wel
			degelijk wat je wilt doen. Klik dus op Ja.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/writer_Samenvoegen.png") %>" alt="Afdruk samenvoegen" />
	<ul>
		<li>In een volgend venstertje krijg je een voorproefje van de gegevens die op je
			etiketten zullen komen, maar nog gewoon in een tabel. Hier kun je - onder 'Uitgave'
			- kiezen om je etiketten af te printen of om er een apart bestand van te maken.
			Die tweede optie laat je toe om eerst nog eens de etiketten na te kijken voor
			je ze print.</li>
	</ul>
</asp:Content>
