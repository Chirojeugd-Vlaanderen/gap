<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: De jaarovergang uitvoeren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		De jaarovergang initiëren</h2>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Jaarovergang' (dat is alleen zichtbaar tussen 1 september
			en het moment dat een GAV van je groep het proces doorlopen heeft).</li>
		<li>Het eerste wat je moet doen, is aanvinken welke afdelingen je hebt in het nieuwe
			werkjaar. In het lijstje staan alle afdelingen die je het jaar ervoor had, dus
			meestal mag je ze gewoon allemaal aanvinken. Eventueel kun je afdelingen aanpassen
			of bijmaken. Als je klaar bent, klik je op de knop 'Volgende'.</li>
	</ul>
	<img src="<%= ResolveUrl("~/Content/Screenshots/Jaarovergang01_afdelingen_aanmaken.png") %>"
		alt="Afdelingen selecteren in jaarovergang" />
	<ul>
		<li>In het volgende scherm zie je de afdelingen die je gekozen hebt, met de officiële
			afdelingen die ermee overeenkomen, de geboortejaren en het 'geslacht'. Maak
			eventueel nog de nodige aanpassingen en klik op de knop 'Verdeling bewaren en
			vorige leden herinschrijven'.</li>
	</ul>
	<img src="<%= ResolveUrl("~/Content/Screenshots/Jaarovergang02_afdelingen_verdelen.png") %>"
		alt="Afdelingen verdelen in jaarovergang" />
	<ul>
		<li>Al wie vorig jaar lid was, is nu ingeschreven. Ze hebben wel nog allemaal een
			instapperiode. Pas na die instapperiode
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is die instapperiode?" } ) %>
			krijg je een factuur voor de aansluitingen, dus je hebt nog tijd om mensen eventueel
			uit te schrijven.</li>
	</ul>
</asp:Content>
