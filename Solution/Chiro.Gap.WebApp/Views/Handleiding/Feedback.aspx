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
		je suggesties voor verbeteringen of uitbreiding? <a href="http://drupal.chiro.be/eloket/feedback-gap"
			title="Stel vragen, of geef opmerkingen en suggesties door">Laat het dan zeker
			weten!</a></p>
</asp:Content>
