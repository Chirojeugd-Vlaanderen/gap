<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Bekijken.aspx.cs" Inherits="WebApp.Views.Groepen.Bekijken" %>

<%@ Register src="GroepenLijstControl.ascx" tagname="GroepenLijstControl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<form id="form1" runat="server">
<uc1:GroepenLijstControl ID="GroepenLijstControl1" runat="server" />
</form>
</asp:Content>
