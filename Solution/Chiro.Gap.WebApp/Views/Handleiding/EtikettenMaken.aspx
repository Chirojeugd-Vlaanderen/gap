<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Etiketten maken
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Etiketten maken</h2>
	<p>
		Je kunt adresetiketten afdrukken met de gegevens uit het GAP. Dat kan niet rechtstreeks
		vanuit het GAP: je moet eerst een
		<%=Html.ActionLink("lijst downloaden", "ViewTonen", new { controller = "Handleiding", helpBestand = "LijstDownloaden" })%>
		en dan een tekstverwerkingsprogramma als Word van Microsoft of Writer van OpenOffice.org
		gebruiken. Het gaat waarschijnlijk ook met andere programma's, maar de meeste
		mensen hebben Word of Writer op hun computer. Writer kun je overigens gratis
		<a href="http://nl.openoffice.org/downloaden.html">downloaden</a> - je hebt wel
		minstens versie 3.2 nodig.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Download de lijst die je nodig hebt: een ledenlijst, een lijst van personen
			in een bepaalde categorie, enz.</li>
		<li>Open je tekstverwerkingsprogramma en doorloop daar de procedure voor 'afdruk
			samenvoegen'.
			<ul>
				<li>
					<%=Html.ActionLink("Etiketten maken in Microsoft Word 2003", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenInWord2003" })%></li>
				<li>
					<%=Html.ActionLink("Etiketten maken in Microsoft Word 2007 of 2010", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenInWordMetRibbon" })%></li>
				<li>
					<%=Html.ActionLink("Etiketten maken in OpenOffice.org Writer 3.2", "ViewTonen", new { controller = "Handleiding", helpBestand = "EtikettenInWriter" })%></li>
			</ul>
		</li>
	</ul>
</asp:Content>
