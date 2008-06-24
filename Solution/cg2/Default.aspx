<%@ Page Language="C#" MasterPageFile="~/ChiroGroep.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cg2._Default" %>

<asp:Content ID="overzichtPersonen" ContentPlaceHolderID="PaginaInhoud" 
    runat="server">
<h2>Personen CHIRO
    <asp:Label ID="groepsLabel" runat="server" Text="Label"></asp:Label>
    </h2>

<p>
Dit is de geweldige testapplicatie.
    <asp:GridView ID="lijst" runat="server">
    </asp:GridView>
</p>

    <asp:Label ID="domLabel" runat="server" Text="Label"></asp:Label>
    <br />


</asp:Content>