<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IList<Lid>>" %>
<%@ Import Namespace="Cg2.Orm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Ledenoverzicht</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Ledenoverzicht</h2>
    
    <% Html.RenderPartial("LedenLijstControl"); %>

</asp:Content>
