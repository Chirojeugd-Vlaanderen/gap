<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Veranderen van afdeling
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Veranderen van afdeling</h2>
	<p>
		Leiding kan bij meer dan één afdeling staan, leden kunnen er maar in één zitten.</p>
	<p>
		Stappen in de procedure:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven'.</li>
		<li>Klik daar op de naam van degene die je in een andere afdeling wilt stoppen.
			Je krijgt dan de persoonsfiche te zien, met links onderaan de lidgegevens. Klik
			daar bij Afdelingen op de link 'aanpassen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Link_Afdelingen_aanpassen.png") %>" 
		alt="Iemands afdeling aanpassen vanop de persoonsfiche" />
	<ul>
		<li>Nu krijg je een lijstje te zien met alle actieve afdelingen in het huidige werkjaar.
			Vink aan in welke afdeling de persoon zit, vink de andere zo nodig af, en klik op 'Bewaren'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Iemand_bij_afdeling_zetten.png") %>" alt="Iemands afdelingen aanpassen" />
	<p>
		<strong>Opgelet:</strong> leiding kun je bij eender welke afdeling zetten, maar
		leden zijn aan hun leeftijd gebonden. Gaat het over een lid dat niet mee moet
		gaan met zijn of haar leeftijdsgenoten, pas dan de Chiroleeftijd
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?" } ) %>
		aan.
	</p>
</asp:Content>
