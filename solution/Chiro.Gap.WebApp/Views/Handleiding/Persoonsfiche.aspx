﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Persoonsfiche
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Persoonsfiche</h2>
	<!-- EditRest -->
	<h3>
		Hoe kom je hier?</h3>
	<p>
		Je komt hier terecht wanneer je in het personenoverzicht (tabblad 'Iedereen')
		of in het ledenoverzicht (tabblad 'Ingeschreven') op een naam klikt.</p>
	<img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_basis.png") %>" alt="Een persoonsfiche" />
	<h3>
		Wat kun je hier zien?</h3>
	<p>
		Op deze pagina zie je alle persoonsgebonden gegevens: adres, telefoonnummer(s),
		mailadres(sen), categorieën, enz.</p>
	<p>
		Gaat het om iemand die ingeschreven is dit werkjaar, dan zie je ook lidgegevens:
		heeft hij of zij al lidgeld betaald, wanneer verstrijkt de instapperiode
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is de instapperiode?"})%>,
		welke functies heeft hij of zij en in welke afdeling zit hij of zij.</p>
	<img src="<%=ResolveUrl("~/Content/Screenshots/lidgegevens_basis.png") %>" alt="Lidgegevens op de persoonsfiche" />
	<h3>
		Wat kun je hier doen?</h3>
	<ul>
		<li>
			<%=Html.ActionLink("Iemands persoonsgegevens aanpassen", "ViewTonen", new { controller = "Handleiding", helpBestand = "PersoonlijkeGegevensfiche" })%>:
			geslacht, geboortedatum, Chiroleeftijd
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?"})%></li>
		<li>
			<%=Html.ActionLink("Een nieuw adres toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuwAdres" })%></li>
		<li>
			<%=Html.ActionLink("Iemands adres aanpassen ('verhuizen')", "ViewTonen", new { controller = "Handleiding", helpBestand = "Verhuizen" })%></li>
		<li>
			<%=Html.ActionLink("Nieuwe telefoonnummers en/of mailadressen toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweCommunicatievorm" })%></li>
		<li>
			<%= Html.ActionLink("Nieuwe categorieën toekennen", "ViewTonen", "Handleiding", null, null, "MeerdereMensen", new { helpBestand = "Categoriseren" }, null)%></li>
		<li>
			<%=Html.ActionLink("Iemand abonneren op Dubbelpunt", "ViewTonen", new { controller = "Handleiding", helpBestand = "Dubbelpuntabonnement" })%></li>
		<li>Lidgegevens aanpassen: lidgeld betaald,
			<%= Html.ActionLink("functies", "ViewTonen", "Handleiding", null, null, "Individueel", new { helpBestand = "IemandsFunctiesAanpassen" }, null)%>
			en
			<%= Html.ActionLink("afdeling(en)", "ViewTonen", "Handleiding", null, null, "Individueel", new { helpBestand = "VeranderenVanAfdeling" }, null)%></li>
	</ul>
</asp:Content>