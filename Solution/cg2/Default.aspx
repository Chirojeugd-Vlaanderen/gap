<%@ Page Language="C#" MasterPageFile="~/ChiroGroep.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cg2._Default" %>

<asp:Content ID="overzichtPersonen" ContentPlaceHolderID="PaginaInhoud" 
    runat="server">
<h2>Dit is een testpagina</h2>

<p>
Dit is de geweldige testapplicatie.
    <asp:Label ID="groepsNaamLabel" runat="server" Text="Label"></asp:Label>
</p>

    <asp:ListBox ID="lijst" runat="server"></asp:ListBox>
    <br />


</asp:Content>