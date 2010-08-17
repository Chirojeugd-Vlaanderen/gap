<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Gegevens filteren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Gegevens in lijsten filteren</h2>
	<p>
		Er zijn twee plaatsen waar je lijsten te zien krijgt: op het tabblad 'Ingeschreven'
		en op het tabblad 'Iedereen'. Je kunt die gegevens filteren met het selectielijstje
		rechts ervan.</p>
	<p>
		Op die pagina's staat ook een link waarmee je de gegevens kunt downloaden als
		Excel-bestand. Als je een filter gebruikte, staan alleen de mensen uit je selectie
		in dat bestand. Zo kun je dus een lijstje van een afdeling
		<%=Html.ActionLink("downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" })%>,
		of van de kookploeg, enz.</p>
	<p>
		Afdelingen en functies kun je alleen toekennen aan ingeschreven leden en leiding,
		en alleen voor het huidige werkjaar. Categorieën
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Categorie", new { helpBestand = "Trefwoorden" }, null)%>
		kun je voor iedereen gebruiken, dus zowel voor leden en leiding als voor 'buitenstaanders'.
		Categorieën zijn niet werkjaargebonden, dus eens iemand erin zit, blijft dat
		zo tot iemand dat ongedaan maakt.</p>
	<a class="anchor" id="Afdeling" />
	<h3>
		Ingeschreven: filteren op afdeling</h3>
	<p>
		Klik op het tabblad 'Ingeschreven'. Rechts van de tabel zie je onder het titeltje
		'Filteren' een selectielijstje met de tekst 'Op afdeling'. Wanneer je een keuze
		maakt (als er een knop staat: en op de knop klikt), zie je de leden en de leiding
		in die afdeling, voor het gekozen werkjaar.
	</p>
	<a class="anchor" id="Functie" />
	<h3>
		Ingeschreven: filteren op functie</h3>
	<p>
		Klik op het tabblad 'Ingeschreven'. Rechts van de tabel zie je onder het titeltje
		'Filteren' een selectielijstje met de tekst 'Op functie'. Wanneer je een keuze
		maakt (als er een knop staat: en op de knop klikt), zie je de leden en de leiding
		met die functie, voor het gekozen werkjaar.
	</p>
	<a class="anchor" id="Categorie" />
	<h3>
		Iedereen: filteren op categorie</h3>
	<p>
		Klik op het tabblad 'Iedereen'. Rechts van de tabel zie je onder het titeltje
		'Filteren' een selectielijstje met de tekst 'Op categorie'. Wanneer je een keuze
		maakt (als er een knop staat: en op de knop klikt), zie je alle personen in
		die categorie.
	</p>
</asp:Content>
