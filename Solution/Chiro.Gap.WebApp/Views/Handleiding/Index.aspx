<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% Html.RenderPartial("AlleHelpLinks"); %>
	<div id="help">
		<p>
			De GAV's van jouw groep moeten de volgende zaken uitvoeren om je aansluiting
			in orde te brengen:</p>
		<ul>
			<li>De jaarovergang initiëren</li>
			<li>Leden en leiding van vorig indelen in hun nieuwe afdelingen</li>
			<li>Nieuwe leden toevoegen</li>
			<li>Al die mensen lid maken</li>
		</ul>
		<p>
			Onderaan rechts op je scherm zie je waarschuwingen voor zaken die je zo snel
			mogelijk in orde moet brengen.</p>
	</div>
</asp:Content>
