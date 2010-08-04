<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.HandleidingModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<% Html.RenderPartial("AlleHelpLinks"); %>
	<div id="help">
		<%= ViewData.Model.HelpBestand %>
	</div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
