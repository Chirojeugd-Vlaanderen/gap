<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Iemands Chiroleeftijd aanpassen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Iemands Chiroleeftijd aanpassen</h2>
	<p>
		Je Chiroleeftijd speelt een rol zolang je lid bent. Hij bepaalt met welk geboortejaar
		je meegaat. Een Chiroleeftijd van 0 betekent dat je gewoon bij je leeftijdsgenoten
		zit. -1 laat je meegaan met de leden die een jaar jonger zijn, 1 met degenen
		die een jaar ouder zijn.</p>
	<p>
		Een typisch geval waarbij je de Chiroleeftijd aanpast, is bij iemand die in
		de lagere school 'is blijven zitten'. Zo iemand zit meestal bij zijn of haar
		klasgenoten in de afdeling, in plaats van bij zijn of haar leeftijdsgenoten.</p>
	<p>
		Bij nieuwe leden kun je de Chiroleeftijd al instellen op het moment dat je hen
		als nieuwe persoon toevoegt.</p>
	<img src="<%=ResolveUrl("~/Content/Screenshots/Nieuwe_persoon.png") %>" alt="De Chiroleeftijd instellen voor een nieuwe persoon" />
	<p>
		Voor iemand die al lid was en pas daarna blijft zitten, kun je dat nog aanpassen.
		Stappen in het proces:</p>
	<ul>
		<li>Klik op het tabblad 'Iedereen' of 'Ingeschreven' op de naam van die persoon.
			Je krijgt dan de persoonsfiche te zien.</li>
	</ul>
	<img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_aanpassenlink.png") %>"
		alt="Chiroleeftijd aanpassen" />
	<ul>
		<li>Klik bovenaan op de link 'persoonlijke gegevens aanpassen'. Je krijgt dan hetzelfde
			formulier te zien als bij een nieuwe persoon, maar wel al ingevuld.</li>
	</ul>
</asp:Content>
