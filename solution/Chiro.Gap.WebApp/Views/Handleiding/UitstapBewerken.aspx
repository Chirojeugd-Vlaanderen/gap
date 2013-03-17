<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Uitstap/bivak bewerken</h2>
	<h3>
		Stappen in het proces:</h3>
	<ul>
		<li>Ga naar het tabblad 'Uitstappen/bivak'.</li>
		<li>Klik in het overzichtje op de naam die je aan die uitstap of het bivak gegeven
			hebt. Zo kom je op de detailfiche.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Fiche_bivakdetails_met_leden.png") %>"
		alt="Details van de uitstap" />
	<ul>
		<li>Je hebt een link om de data aan te passen en één om een bivakplaats toe te voegen
			of te bewerken (al naargelang).</li>
		<li>Om deelnemers uit te schrijven, klik je naast hun naam op de link 'uitschrijven'.</li>
	</ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
	Handleiding: Uitstap/bivak bewerken
</asp:Content>
