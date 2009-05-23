<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GavModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>GAV</h2>
    <% foreach (var item in ViewData.Model.GroepenLijst)
       {
           %>
           
           
           <%=Html.TextBox(item.Groepsnaam) %>
           <br />
           <%
           
       } %>

</asp:Content>