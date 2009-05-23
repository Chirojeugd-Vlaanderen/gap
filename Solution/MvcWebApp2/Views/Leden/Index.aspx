<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LidInfoModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="Cg2.ServiceContracts" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Ledenoverzicht</h2>
    
    <% Html.RenderPartial("LedenLijstControl"); %>

</asp:Content>
