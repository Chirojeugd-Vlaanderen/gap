<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Verzekering loonverlies
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Verzekering loonverlies</h2>
	<p>
		Aangesloten leden en leiding zijn verzekerd voor burgelijke aansprakelijkheid,
		dood en invaliditeit. Meer uitleg over de Chiroverzekering vind je op <a href="http://www.chiro.be/verzekeringen"
			title="Uitleg over de Chiroverzekering">www.chiro.be/verzekeringen</a>.</p>
	<p>
		Leiding die al gaat werken, kun je bijkomend verzekeren voor loonverlies. Die
		optie vult voor één jaar het bedrag aan dat de ziekteverzekering uitbetaalt
		als je door een ongeval niet meer kunt gaan werken.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' op de naam van de leider of leidster die
			je bijkomend wilt verzekeren.</li>
		<li>Je komt nu op de persoonsfiche. Klik links onderaan, bij de lidgegevens, op
			de link ''.</li></ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Loonverlies.png") %>" alt="Iemand verzekeren voor loonverlies vanop de persoonsfiche" />
	<ul>
		<li>Dan krijg je eerst nog een vraag om bevestiging. Klik op de knop 'Bevestigen'
			om af te ronden.</li></ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Bevestiging_Loonverlies.png") %>"
		alt="Bevestiging verzekering voor loonverlies" />
</asp:Content>
