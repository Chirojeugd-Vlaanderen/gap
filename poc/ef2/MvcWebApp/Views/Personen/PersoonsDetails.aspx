<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="PersoonsDetails.aspx.cs" Inherits="MvcWebApp.Views.PersoonsDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<%Html.RenderPartial("PersoonsDetailsUserControl"); %>

</asp:Content>
