<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Colofon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Colofon</h2>
	<p>
		Heel wat mensen hebben ertoe bijgedragen dat deze website tot stand kwam.</p>
	<ul>
		<li>De <strong>werkgroep GAP</strong>: Johan Vervloet (hoofdprogrammeur en projectleider),
			Bart Boone, Peter Bertels, Broes Decat, Tim Mallezie, Koen Meersman, Tom Haepers,
            Mathias Keustermans</li>
		<li>De <strong>testers</strong>: Merijn Gouweloose, Maarten Perpet, Roel Vercammen, Ben Bridts en heel wat
			anderen</li>
		<li>En verder: alle mensen die ooit feedback gaven over het vroegere Chirogroepprogramma,
			de medewerk(st)ers van het nationaal secretariaat</li>
	</ul>
	<p>
		De bedoeling van deze website is dat het een handig instrument is voor lokale
		leiding. Heb je opmerkingen over hoe bepaalde zaken nu uitgewerkt zijn, of heb
		je suggesties voor verbeteringen of uitbreiding? <a href="http://www.chiro.be/eloket/feedback-gap"
			title="Stel vragen, of geef opmerkingen en suggesties door">Laat het dan zeker
			weten!</a></p>
</asp:Content>
