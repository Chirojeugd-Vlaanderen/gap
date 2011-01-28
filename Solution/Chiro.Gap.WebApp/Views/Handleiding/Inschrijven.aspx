<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Inschrijven</h2>
	<p>
		Iemand inschrijven kan op twee manieren:</p>
	<ul>
		<li>Automatisch bij de jaarovergang: iedereen die vorig werkjaar ingeschreven was,
			wordt automatisch opnieuw ingeschreven. Ze krijgen allemaal een instapperiode,
			zodat je nog tijd hebt om degenen uit te schrijven die niet meer komen. Kort
			voor het einde van die instapperiode krijgen de mensen met een login voor jouw
			groep een mailtje om daar even op te wijzen.</li>
		<li>In de loop van het jaar: je kunt het hele jaar door mensen toevoegen en inschrijven.</li>
	</ul>
	<p>
		Mensen die ingeschreven zijn en die niet meer in hun instapperiode zitten, zijn
		aangesloten bij Chirojeugd Vlaanderen. Daarvoor krijgt je financieel verantwoordelijke
		een factuur.</p>
		
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
	Handleiding: inschrijven
</asp:Content>
