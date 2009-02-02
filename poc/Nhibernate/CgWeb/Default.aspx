<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CgWeb._Default" %>

<asp:Content ID="inhoud" ContentPlaceHolderID="mainContent" runat="server">
<br />
<br />
    <asp:GridView ID="persoonsInfoGrid" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="PersoonID" 
                DataNavigateUrlFormatString="PersoonPagina.aspx?Id={0}" DataTextField="PersoonID" 
                HeaderText="PersoonID" />
            <asp:BoundField DataField="VoorNaam" HeaderText="Voornaam" />
            <asp:BoundField DataField="Naam" HeaderText="Naam" />
            <asp:BoundField DataField="Categorieen" HeaderText="Categorieën" />
            <asp:BoundField DataField="StraatNaam" HeaderText="Straat" />
            <asp:BoundField DataField="HuisNr" HeaderText="Nr." />
            <asp:BoundField DataField="Bus" HeaderText="Bus" />
            <asp:BoundField DataField="PostNr" HeaderText="Postnr." />
            <asp:BoundField DataField="SubGemeente" HeaderText="Gemeente" />
            <asp:BoundField DataField="GeboorteDatum" HeaderText="Geboortedatum" />
            <asp:BoundField DataField="TelefoonNummer" HeaderText="Telefoonnr." />
            <asp:TemplateField HeaderText="E-mail">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" 
                        NavigateUrl='<%# Eval("EMail", "mailto:{0}") %>' Text='<%# Eval("EMail") %>'></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
</asp:GridView>
&nbsp;
</asp:Content>