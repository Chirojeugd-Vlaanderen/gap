<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Groep
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Groep</h2>
	<h3>
		Wat zie je hier?</h3>
	<p>
		Op dit tabblad zie je alle groepsgebonden instellingen: jullie afdelingsverdeling,
		en de categorieën en functies die je groep gebruikt.</p>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Groepsinstellingen_overzicht.png") %>" 
		alt="Wat is er te zien op het tabblad Groep?" />
	<h3>
		Wat kun je hier doen?</h3>
	<ul>
		<li>
			<%=Html.ActionLink("De afdelingsverdeling aanpassen", "ViewTonen", new { controller = "Handleiding", helpBestand = "AfdelingsverdelingAanpassen" })%></li>
		<li>
			<%=Html.ActionLink("Een nieuwe categorie toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweCategorie" })%></li>
		<li>
			<%=Html.ActionLink("Een nieuwe functie toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweFunctie" })%></li>
	</ul>
</asp:Content>
