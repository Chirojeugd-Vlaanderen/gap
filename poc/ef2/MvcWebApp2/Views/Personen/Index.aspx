<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IList<GelieerdePersoon>>" %>
<%@ Import Namespace="Cg2.Orm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Personenoverzicht</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Personenoverzicht</h2>

<% Html.RenderPartial("PersonenLijstControl"); %>

</asp:Content>
