<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Categoriseren
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Werken met categorie&euml;n</h2>
	<p>
		Categorieën dienen om mensen in groepen te stoppen, zodat je ze makkelijk terugvindt.
		Er zijn twee plaatsen waar je dat kunt doen.</p>
	<p>
		Voor je mensen in categorie&euml;n kunt stoppen, moet je die categorie&euml;n
		natuurlijk wel al
		<%=Html.ActionLink("toegevoegd hebben", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweCategorie" })%>.</p>
	<a class="anchor" id="MeerdereMensen" />
	<h3>
		Op het tabblad 'Iedereen'</h3>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Vink het hokje aan voor al de personen die tot de categorie moeten behoren.</li>
		<li>Selecteer boven de tabel de actie 'In dezelfde categorie stoppen'.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Categoriseren_MeerdereMensen.png") %>"
		alt="Meerdere mensen tegelijk in een categorie stoppen" />
	<ul>
		<li>Vink aan in welke categorie die mensen moeten terechtkomen.</li>
	</ul>
	<a class="anchor" id="Individueel" />
	<h3>
		Op de persoonsfiche</h3>
	<p>
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen' of 'Ingeschreven'.</li>
		<li>Klik op de naam van degene die je in een categorie wilt stoppen. Zo ga je naar
			de persoonsfiche.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_categorielink.png") %>"
		alt="Iemands categorieën aanpassen" />
	<ul>
		<li>Klik op de link 'toevoegen aan categorie'</li>
		<% // TODO (#649): screenshot maken en toevoegen %>
		<li>$$$ aan te vullen na bugfix $$$</li>
	</ul>
</asp:Content>
