<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Etiketten in Word 2007/2010
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Etiketten maken in Word 2007/2010</h2>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Open Word.</li>
		<li>Klik in de ribbon op het tabblad 'Verzendlijsten'.</li>
		<li>Klik op de knop 'Afdruk samenvoegen starten' en kies 'Etiketten'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap01.png") %>" alt="Afdruk samenvoegen starten" />
	<ul>
		<li>Selecteer in het venstertje het merk en formaat van je etiketten. (De meest
			gebruikte etiketten zijn er van 21 op een blad (3 bij 7): Standaard Avery A4/A5,
			formaat L7160.)</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap02.png") %>" alt="Etikettenformaat instellen" />
	<ul>
		<li>Klik in de ribbon op de knop 'Adressen selecteren' en kies 'Bestaande lijst
			selecteren'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap03.png") %>" alt="Etikettenformaat instellen" />
	<ul>
		<li>Je krijgt dan een venstertje waarmee je de gedownloade lijst opzoekt.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap04.png") %>" alt="Het Excel-bestand koppelen" />
	<ul>
		<li>Heb je het bestand geselecteerd en op de knop 'Openen' geklikt, dan krijg je
			normaal gezien een venstertje waarin je moet aangeven op welk werkblad de gegevens
			staan (er is er maar één). Klik op 'OK' om verder te gaan.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap05.png") %>" alt="Werkblad selecteren in het Excel-bestand" />
	<ul>
		<li>Nu staat er een tabel op je blad, met in elke cel de tekst &lt;&lt;Volgende
			record&gt;&gt;. Om gegevens op je etiket te zetten, klik je in de ribbon op
			de knop 'Samenvoegvelden invoegen'. Je krijgt dan een lijstje met de beschikbare
			kolommen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap06.png") %>" alt="Gegevens op het etiket zetten" />
	<ul>
		<li>Klik op de opties om de gegevens in je etiket te zetten en typ er de nodige
			spaties en nieuwe regels bij.</li>
		<li>Een kleine truc: selecteer de hele tabel, klik in de ribbon op het tabblad Indeling,
			onder 'Hulpmiddelen voor tabellen', en klik daar op de knop om linksmidden uit
			te lijnen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap07.png") %>" alt="De etiketten uitlijnen" />
	<ul>
		<li>Als de schikking van je etiket klaar is, klik je op de knop 'Etiketten bijwerken'.
			De andere etiketten op het blad krijgen nu dezelfde schikking.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap08.png") %>" alt="Gegevens op het etiket zetten" />
	<ul>
		<li>Om de etiketten te maken, klik je op de knop 'Voltooien en samenvoegen'. Kies
			'Afzonderlijke documenten bewerken', dan krijg je eerst alle gegevens te zien.
			Dan kun je eventueel nog dingen aanpassen voor je begint te printen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap09.png") %>" alt="Voltooien en samenvoegen" />
	<ul>
		<li>Klik in het kleine venstertje gewoon op OK om door te gaan.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/wordribbon_stap10.png") %>" alt="Kies 'Alles' om alle etiketten te maken" />
</asp:Content>
