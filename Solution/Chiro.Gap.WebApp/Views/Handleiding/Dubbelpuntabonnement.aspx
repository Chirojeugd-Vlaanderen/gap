<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Dubbelpuntabonnement
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iemand abonneren op Dubbelpunt</h2>
	<p>
		Dubbelpuntabonnementen moet je individueel aanvragen. Dat kan op de persoonsfiche.
		Daar staat ook bij hoeveel dat kost. Na de eerstvolgende facturatieronde
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Factuur", new { helpBestand = "Trefwoorden" }, new { title = "Uitleg over facturen"})%>
		krijgt je financieel verantwoordelijke een factuur voor de aangevraagde abonnementen.</p>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
		<li>Klik daar op de naam van degene voor wie je een abonnement wilt aanvragen. Je
			komt dan op de persoonsfiche.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_abonnerenlink.png") %>"
		alt="Een Dubbelpuntabonnement aanvragen" />
	<ul>
		<li>Klik daar op de link 'abonneren'.</li>
		<li>Dan krijg je eerst nog een vraag om bevestiging. Klik op de knop 'Bevestigen'
			om af te ronden.</li>
	</ul>
		<img src="<%=ResolveUrl("~/Content/Screenshots/Bevestiging_Dubbelpuntabonnement.png") %>"
		alt="Bevestiging Dubbelpuntabonnement" />
</asp:Content>
