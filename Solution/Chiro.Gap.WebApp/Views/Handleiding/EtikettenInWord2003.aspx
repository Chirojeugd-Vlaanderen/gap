<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Etiketten in Word 2003
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Etiketten maken in Microsoft Word 2003</h2>
	<p>
		De lijsten die je kunt downloaden, zijn opgemaakt in het formaat van Office
		2007. Als je Office 2003 hebt en je krijgt het bestand niet open, dan moet je
		eerst een <a href="http://www.microsoft.com/downloads/details.aspx?familyid=941b3470-3ae9-4aee-8f43-c6bb74cd1466&displaylang=nl"
			title="Hulpprogramma om Office 2007-bestanden te kunnen openen in Office 2003">
			extra programma</a> installeren. Open dan eerst het Excel-bestand en sla 
		het op in de 2003-indeling - anders kan Word er niet mee overweg.</p>
		
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Open Word.</li>
		<li>Klik op Extra > Brieven en verzendlijsten > Afdruk samenvoegen.</li>
		<li>Er verschijnt rechts een paneel aan de rechterkant van je scherm. Kies 'Etiketten',
			en klik onderaan op 'Volgende: Begindocument'.</li>
		<li>Als je dan op 'Opties' klikt, kan je een etiketformaat kiezen. (De meest gebruikte
			etiketten zijn er van 21 op een blad (3 bij 7): Standaard Avery A4/A5, formaat
			L7160.) Klik op 'OK' om verder te gaan.</li>
		<li>Klik op 'Volgende: Adressen selecteren'.</li>
		<li>Klik op 'Bladeren&#8230;'. Selecteer in het nieuwe venstertje je Excel-bestand en
			klik op de knop 'Openen'. Dan krijg je normaal gezien een venstertje waarin
			je moet aangeven op welk werkblad de gegevens staan (er is er maar één) en één
			waarin je een voorbeeld kunt zien van je gegevens. Klik op 'OK' om verder te
			gaan.</li>
		<li>Nu staat er een tabel op je blad, met in elke cel de tekst &lt;&lt;Volgende
			record&gt;&gt;. Om gegevens op je etiket te zetten, klik je rechts in het paneel
			op 'Meer items'. Je krijgt een lijstje met de beschikbare velden in je gegevensbestand.
			Klik één voor een alle velden aan die je wil gebruiken aan, en klik 'Invoegen'.
			Vervolgens sluit je het venster met velden door op 'Annuleren' te klikken.</li>
		<li>Voeg de nodige spaties en regeleinden toe om het er als een adresetiket te laten
			uitzien.</li>
		<li>Als je tevreden bent over je etiket, klik je in het paneel op de knop 'Alle
			etiketten bijwerken'.</li>
		<li>Klik op 'Volgende: Labelvoorbeeld', en je krijgt je eerste etikettenblad te
			zien.</li>
		<li>Ten slotte klik je op 'Volgende: Samenvoeging voltooien' om je etiketten af
			te drukken.</li>
	</ul>
</asp:Content>
