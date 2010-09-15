<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding GAP
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="helpContent" runat="server">
	<% 
		// Van augustus tot en met november tonen we bovenaan welke stappen er nodig zijn voor het nieuwe werkjaar
		if (DateTime.Now.Month > 7 || DateTime.Now.Month <= 11)
		{ %>
	<p>
		De GAV's van jouw groep moeten de volgende zaken uitvoeren om je aansluiting
		in orde te brengen:</p>
	<ul>
		<li>De jaarovergang initiëren</li>
		<li>Controleren of je leden van vorig werkjaar in de juiste afdeling terechtgekomen
			zijn</li>
		<li>De leiding bij de juiste afdeling zetten</li>
		<li>Nieuwe leden toevoegen en inschrijven</li>
	</ul>
	<%
		}%>
	<p>
		Als er zaken zijn die je zo snel mogelijk in orde moet brengen, dan krijg je
		daar onderaan rechts op je scherm een kadertje met een waarschuwing voor.</p>
	<p>
		Als je in de handleiding een link ziet staan die uit één of meerdere woorden
		bestaat, dan verwijst die naar een ander onderdeel van de handleiding of naar
		een externe website. Als er [?] staat, dan verwijst die link naar een trefwoord
		dat uitgelegd wordt in de handleiding.</p>
	<p>
		Vragen of feedback? <a href="http://www.chiro.be/eloket/feedback-gap">Laat maar
			komen!</a></p>
	<p>
		Je kunt discussiëren over hoe handig het systeem wel of niet is, en je
		kunt elkaar helpen om het te leren gebruiken: op het <a href="http://forum.chiro.be/forum/144">
			Chiroforum</a>.</p>
	<p>
		Of zit je heel erg in de knoop en heb je dringend hulp nodig? Dan kun je altijd
		nog bellen naar het nationaal secretariaat: 03-231&nbsp;07&nbsp;95.</p>
</asp:Content>
