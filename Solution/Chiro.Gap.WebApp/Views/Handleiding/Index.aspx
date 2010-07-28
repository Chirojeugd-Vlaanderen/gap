<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<% Html.RenderPartial("AlleHelpLinks"); %>
	<div id="help">
		<p>De volgende administratieve taken liggen op jou  te wachten.</p>
    <ul>
		<li>De jaarovergang initiëren</li>
		<li>Leden en leiding van vorig indelen in hun nieuwe afdelingen</li>
		<li>Nieuwe leden toevoegen</li>
		<li>Al die mensen lid maken</li>
    </ul>
	</div>
	<br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
