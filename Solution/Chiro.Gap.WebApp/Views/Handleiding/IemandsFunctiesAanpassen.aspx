<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Iemands functies aanpassen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iemands functies aanpassen</h2>
	<p>
		Je kunt alleen functies toekennen aan ingeschreven leden en leiding. Op het
		tabblad 'Ingeschreven' zie je in de kolom 'Func.' de afkortingen van de functies
		die iemand heeft.</p>
	<p>
		Je kunt iemand een functie toekennen of ze weer afnemen. Als je iemand
		<%=Html.ActionLink("uitschrijft", "ViewTonen", new { controller = "Handleiding", helpBestand = "Uitschrijven" })%>,
		worden de toegekende functies automatisch afgenomen. Bij de jaarovergang worden
		<em>alle</em> functies afgenomen.</p>
	<p>
		Er zijn twee functies die elke groep <em>moet</em> invullen:</p>
	<ul>
		<li>Contactpersoon: dat is degene die de post zal ontvangen van het nationaal secretariaat.</li>
		<li>Financieel verantwoordelijke: dat is degene naar wie de facturen opgestuurd worden</li>
	</ul>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
		<li>Klik daar op de naam van degene voor wie je de functies wilt aanpassen. Je komt
			dan op de persoonsfiche.</li>
		<li>Klik links onderaan, bij de lidgegevens op de link 'functies aanpassen'.</li>
		<li>Je komt nu op het formulier met lidgegevens van de persoon in kwestie. Je vindt
			er onder andere een lijstje met alle mogelijke functies: zowel de officiële
			als degene die jouw groep zelf toevoegde. Vink aan welke functies de persoon
			moet krijgen, en vink wanneer nodig de andere af.</li>
		<li>Klik om af te ronden op de knop 'Gegevens wijzigen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Inschrijvingsgegevens.png") %>"
		alt="Inschrijvingsgegevens aanpassen" />
</asp:Content>
