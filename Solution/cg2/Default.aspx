<%@ Page Language="C#" MasterPageFile="~/CG.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cg2.Default" %>

<asp:Content ID="overzichtPersonen" ContentPlaceHolderID="PaginaInhoud" runat="server">
    <h1>Lijst met groepen</h1>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
    CssClass="tabel" GridLines="None" 
    onselectedindexchanged="GridView1_SelectedIndexChanged">
        <RowStyle CssClass="normal" />
        <Columns>
            <asp:BoundField DataField="Code" HeaderText="StamNr"></asp:BoundField>
            <asp:HyperLinkField DataNavigateUrlFields="Code" 
                DataNavigateUrlFormatString="Detail.aspx?stamnr={0}" DataTextField="Naam" 
                DataTextFormatString="{0}" HeaderText="Naam" />
        </Columns>
        <HeaderStyle CssClass="header" />
        <AlternatingRowStyle CssClass="alternating" />
    </asp:GridView>
</asp:Content>



