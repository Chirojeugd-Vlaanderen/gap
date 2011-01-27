<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Uitschrijven
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iemand uitschrijven</h2>
	<p>
		Iemand uitschrijven is een drastische maatregel. In principe kan het ook niet
		- toch niet voor het nationaal secretariaat. Wanneer je iemand aansluit, is
		dat voor een heel werkjaar.</p>
	<p>
		In je groep kun je wel de beslissing nemen om iemand uit te schrijven. In het
		programma heet dat: 'op non-actief zetten'. De persoon blijft wel in je gegevensbestand
		zitten, maar hij of zij verschijnt niet meer bij de ingeschreven leden. Van
		mensen die automatisch ingeschreven werden bij de jaarovergang en die geen lid
		meer zijn, kun je zo vermijden dat je toch nog voor hun aansluiting betaalt.
	</p>
	<p>
		<em>Terzijde: het administratieprogramma waarschuwt je wanneer de probeerperiode
			van je leden verloopt, zodat je nog degenen kunt uitschrijven die geen lid willen
			worden of van wie je het nog niet zeker bent. Als je na de probeerperiode ontdekt
			dat je toch leden foutief aangesloten hebt, dan kunnen we dat nog rechtzetten,
			maar dat kost wel wat werk. Je krijgt dan wel hun inschrijvingsgeld terug, maar
			min 15 euro administratiekosten.</em>
	</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven'. In de laatste kolom zie je een link 'Uitschrijven'
			staan bij al wie ingeschreven is. </li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Uitschrijven.png") %>" alt="Iemand uitschrijven" />
	<ul>
		<li>Als je op die link klikt, krijg je de melding dat het lid op non-actief gezet
			is. De naam is dan ook verdwenen uit de tabel.</li>
	</ul>
	<p>
		Foutje gemaakt? De verkeerde persoon uitgeschreven? Geen probleem: zoek hem
		of haar op het tabblad 'Iedereen' en klik weer op 'Inschrijven'. Je zult niet
		opnieuw een factuur krijgen voor de aansluiting.</p>
</asp:Content>
