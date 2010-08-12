<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<% Html.RenderPartial("AlleHelpLinks"); %>
	<div id="help">
		<%=Model.HelpBestand %>
	</div>
</asp:Content>