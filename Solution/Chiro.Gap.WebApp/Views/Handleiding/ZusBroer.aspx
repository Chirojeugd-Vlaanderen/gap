<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Zus/broer toevoegen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Gegevens van zus of broer toevoegen</h2>
	<p>
		Als je verschillende kinderen uit hetzelfde gezin in je groep hebt, is het vervelend
		als je alle adres- en telefoongegevens dubbel moet invullen. Goed nieuws: dankzij
		de link 'zus/broer maken' is dat niet meer nodig!</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen'</li>
		<li>Zoek de persoon voor wie je een broer of zus wilt toevoegen</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Broer_of_zus.png") %>" alt="Gegevens voor een zus of broer toevoegen" />
	<ul>
		<li>Klik op de link 'zus/broer maken'. Je gaat dan naar het scherm waar je een nieuwe
			persoon aanmaakt, en de familienaam is al ingevuld.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Formulier_Broer_zus.png") %>" alt="Gegevens voor een zus of broer toevoegen" />
	<ul>
		<li>Vul de ontbrekende gegevens in (en pas eventueel de familienaam aan als het
			over een nieuwsamengesteld gezin gaat). Klik op de knop 'Bewaren'.</li>
		<li>Nu zit je op de persoonsfiche. Daar kun je zien dat het thuisadres en alle gezinsgebonden
			communicatievormen overgenomen zijn van de oorspronkelijke persoon.</li>
	</ul>
</asp:Content>
