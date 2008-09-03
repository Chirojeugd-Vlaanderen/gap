<%@ Page Language="C#" MasterPageFile="~/CG.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cg2.Default" %>

<asp:Content ID="overzichtPersonen" ContentPlaceHolderID="PaginaInhoud" runat="server">
<h1>Lijst met personen</h1>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Voornaam" HeaderText="Voornaam"></asp:BoundField>
        </Columns>
    </asp:GridView>
&nbsp;</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="InlogInfo" runat="server">
<div class="tekst">Ingelogd als Mattias Michaux <a href="#">[uitloggen]</a></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="werkjaar" runat="server">
<div class="huidig">Werkjaar: 2008 - 2009</div> <a href="#">[ander werkjaar]</a>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="titel" runat="server">
<div class="tekst">Chirogroep</a></div>
</asp:Content>



