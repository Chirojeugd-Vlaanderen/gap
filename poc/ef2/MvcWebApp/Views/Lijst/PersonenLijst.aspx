<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="PersonenLijst.aspx.cs" Inherits="MvcWebApp.Views.Lijst.PersonenLijst" %>
<%@ Register src="PersonenLijstUserControl.ascx" tagname="PersonenLijstUserControl" tagprefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server">
    <uc1:PersonenLijstUserControl ID="PersonenLijstUserControl1" runat="server" />
    </form>
</asp:Content>
