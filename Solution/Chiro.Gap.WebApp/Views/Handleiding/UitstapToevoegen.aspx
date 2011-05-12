<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Uitstap/bivak toevoegen</h2>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Ga naar het tabblad 'Uitstappen/bivak'.</li>
		<li>Klik op 'Uitstap/bivak toevoegen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Tabblad_bivak.png") %>" alt="Tabblad 'Uitstappen/bivak'" />
	<ul>
		<li>Vul op de nieuwe pagina de nodige gegevens in. Gaat het over je groepsbivak,
			het 'klein kamp' voor je jongste afdeling of een buitenlands bivak, vink dan
			aan dat het om een jaarlijks bivak gaat <em>(ook als je het niet elk jaar organiseert!)</em>.
			Voor een bivak moet je namelijk een bivakaangifte
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Bivakaangifte", new { helpBestand = "Trefwoorden" }, new { title = "Bivakaangifte" } ) %>
			doen. Dat vinkje zorgt ervoor dat de gegevens automatisch doorgegeven worden
			aan het nationaal secretariaat.</li>
		<li>Klik op de knop Bewaren.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Nieuwe_uitstap.png") %>" alt="Nieuwe uitstap registreren" />
	<ul>
		<li>Nu zie je een detailfiche van je nieuwe uitstap. Daar staan nog niet veel details
			in, dus klik je op de link 'Bivakplaats ingeven'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Formulier_bivakdetails.png") %>"
		alt="Details van de uitstap aanvullen" />
	<ul>
		<li>Vul het adres in. Het formulier gebruikt dezelfde stratenlijst als wanneer je
			personen toevoegt.</li>
		<li>Klik op 'Bewaren'. Je keert dan terug naar de detailfiche.</li>
	</ul>
	<p>
	</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
	Handleiding: Uitstap/bivak toevoegen
</asp:Content>
