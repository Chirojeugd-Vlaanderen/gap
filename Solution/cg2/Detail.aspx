<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="cg2.Detail" MasterPageFile="~/CG.Master" %>

<asp:Content ID="overzichtPersonen" ContentPlaceHolderID="PaginaInhoud" runat="server">
    <h1><asp:Label ID="Titel" runat="server" Text="Label"></asp:Label>
        
    </h1>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        style="margin-right: 2px" AllowPaging="True" AllowSorting="True" 
    CssClass="tabel" GridLines="None">
        <RowStyle CssClass="normal" />
        <Columns>
            <asp:BoundField DataField="Naam" HeaderText="Naam" />
            <asp:BoundField DataField="VoorNaam" HeaderText="voornaam" />
            <asp:HyperLinkField DataNavigateUrlFields="persoonID" 
                DataNavigateUrlFormatString="PersoonPage.aspx?id={0}" DataTextField="persoonID" 
                DataTextFormatString="Toon persoon" HeaderText="link" />
        </Columns>
        <HeaderStyle CssClass="header" />
        <AlternatingRowStyle CssClass="alternating" />
        </asp:GridView>
</asp:Content>

