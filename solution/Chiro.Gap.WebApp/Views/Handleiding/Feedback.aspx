<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Feedback over het programma
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Feedback geven</h2>
	<p>
		De bedoeling van deze website is dat het een handig instrument is voor lokale
		leiding. Heb je opmerkingen over hoe bepaalde zaken nu uitgewerkt zijn, of heb
		je suggesties voor verbeteringen of uitbreiding? <a href="http://www.chiro.be/eloket/feedback-gap"
			title="Stel vragen, of geef opmerkingen en suggesties door">Laat het dan zeker
			weten!</a></p>
	<p>
		Je kunt discussiëren over hoe handig het systeem wel of niet is, en je
		kunt elkaar helpen om het te leren gebruiken: op het <a href="http://forum.chiro.be/forum/144">
			Chiroforum</a>.</p>
	<p>
		En zit je heel erg in de knoop en heb je dringend hulp nodig? Dan kun je altijd
		nog bellen naar het nationaal secretariaat: 03-231&nbsp;07&nbsp;95.</p>
</asp:Content>
