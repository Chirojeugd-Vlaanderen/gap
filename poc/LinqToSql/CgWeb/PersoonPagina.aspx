<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PersoonPagina.aspx.cs" Inherits="CgWeb.PersoonPagina" %>

<%@ Register src="PersoonUserControl.ascx" tagname="PersoonUserControl" tagprefix="uc1" %>

<asp:Content ID="inhoud" ContentPlaceHolderID="mainContent" runat="server">
    <uc1:PersoonUserControl ID="PersoonUserControl1" runat="server" />
    <asp:Button ID="bewarenButton" runat="server" onclick="bewarenButton_Click" 
        Text="Bewaren" />
</asp:Content>