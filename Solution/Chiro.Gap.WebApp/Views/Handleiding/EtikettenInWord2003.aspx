<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
%>
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
			extra programma</a> installeren. Zorg ook dat je <a href="http://update.microsoft.com">
				de recentste updates van Office</a> geïnstalleerd hebt.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Open het Excel-bestand.</li>
		<li>Klik dan op Bestand > Opslaan als. Kies in het venstertje de indeling "Microsoft
			Office Excel-werkmap" in plaats van "Excel 2007-werkmap" en sla het bestand
			op.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/excel2003_Opslaan_als.png") %>"
		alt="$2" />
	<ul>
		<li>Sluit Excel af en open Word.</li>
		<li>Klik op Extra > Brieven en verzendlijsten > Afdruk samenvoegen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten01.png") %>"
		alt="Afdruk samenvoegen" />
	<ul>
		<li>Er verschijnt rechts een paneel aan de rechterkant van je scherm. Kies 'Etiketten',
			en klik onderaan op 'Volgende: Begindocument'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten02.png") %>"
		alt="Etikettenwizard in het paneel" />
	<ul>
		<li>Als je dan op 'Opties' klikt, kan je een etiketformaat kiezen. (De meest gebruikte
			etiketten zijn er van 21 op een blad (3 bij 7): Standaard Avery A4/A5, formaat
			L7160.) Klik op 'OK' om verder te gaan.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten03.png") %>"
		alt="Begindocument etiketten" />
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten04.png") %>"
		alt="Etikettype kiezen" />
	<ul>
		<li>Klik op OK voor de instellingen van de etiketten, en dan in het paneel onderaan
			op 'Volgende: Adressen selecteren'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten05.png") %>"
		alt="Aan het Excel-bestand koppelen" />
	<ul>
		<li>Klik op 'Bladeren&#8230;'. Selecteer in het nieuwe venstertje je Excel-bestand
			en klik op de knop 'Openen'. Dan krijg je normaal gezien een venstertje waarin
			je moet aangeven op welk werkblad de gegevens staan (er is er maar één) en één
			waarin je een voorbeeld kunt zien van je gegevens. Klik op 'OK' om verder te
			gaan.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten06.png") %>"
		alt="Het Excel-bestand opzoeken" />
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten07.png") %>"
		alt="Het juiste werkblad selecteren" />
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten08.png") %>"
		alt="Een voorbeeld van de gegevens" />
	<ul>
		<li>Nu staat er een tabel op je blad, met in elke cel de tekst &lt;&lt;Volgende
			record&gt;&gt;. Om gegevens op je etiket te zetten, klik je rechts in het paneel
			op 'Meer items'. Je krijgt een lijstje met de beschikbare velden in je gegevensbestand.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten09.png") %>"
		alt="Klik op 'Meer items' om gegevens op te halen uit het Excel-bestand" />
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten10.png") %>"
		alt="Selecteer telkens het veld dat je nodig hebt en klik op 'Invoegen'" /><% // TODO (#649): screenshot maken en toevoegen %>
	<ul>
		<li>Klik één voor een alle velden aan die je wil gebruiken aan, en klik 'Invoegen'. Vergeet het busnummer niet!
			Vervolgens sluit je het venster met velden door op 'Annuleren' te klikken.</li>
		<li>Voeg de nodige spaties en regeleinden toe om het er als een adresetiket te laten
			uitzien.</li>
		<li>Als je tevreden bent over je etiket, klik je in het paneel op de knop 'Alle
			etiketten bijwerken'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten12.png") %>"
		alt="Klik op 'Etiketten bijwerken' om het blad af te werken" />
	<ul>
		<li>Klik op 'Volgende: Labelvoorbeeld', en je krijgt je eerste etikettenblad te
			zien.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/word2003_etiketten13.png") %>"
		alt="Een voorbeeldblad met de gegevens van de eerste pagina" />
	<ul>
		<li>Ten slotte klik je op 'Volgende: Samenvoeging voltooien' om je etiketten af
			te drukken.</li>
	</ul>
</asp:Content>
