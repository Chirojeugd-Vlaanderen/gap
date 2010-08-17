<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Nieuwe persoon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Een nieuwe persoon toevoegen</h2>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen'.</li>
		<li>Klik rechts bovenaan naast de tabel op de link 'Nieuwe persoon'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Nieuwe_persoon.png") %>"
		alt="Een nieuwe persoon toevoegen vanop het tabblad 'Iedereen'" />
	<ul>
		<li>Je komt nu op een formuliertje om een nieuwe persoon toe te voegen.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Nieuwe_persoon.png") %>" alt="Formuliertje voor een nieuwe persoon" />
	<ul>
		<li>Vul de gevraagde gegevens in en klik op de knop 'Bewaren'
			<ul>
				<li>Als er iets foutliep, krijg je daar een foutmelding voor zodat je nog zaken
					kunt aanpassen. Het kan bv. zijn dat je iemand dubbel probeert toe te voegen
					of dat je te weinig invulde.</li>
				<li>Als er geen problemen meer zijn, kom je op een ander scherm, waar je bijkomende
					informatie kunt toevoegen.</li>
			</ul>
		</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_nieuwe_persoon.png") %>"
		alt="Bijkomende gegevens voor een nieuwe persoon" />
	<ul>
		<li>Bijkomende gegevens die je kunt toevoegen:
			<ul>
				<li>Adres(sen)</li>
				<li>Communicatievormen: telefoonnummer(s), mailadres(sen), enz.</li>
				<li>Categorieën</li>
			</ul>
		</li>
	</ul>
	<p>
		Vanaf nu kun je die nieuwe persoon eventueel inschrijven als lid.</p>
</asp:Content>
