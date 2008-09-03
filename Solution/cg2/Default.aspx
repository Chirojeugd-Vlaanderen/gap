<%@ Page Language="C#" MasterPageFile="~/ChiroGroep.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="cg2._Default" %>

<asp:Content ID="overzichtPersonen" ContentPlaceHolderID="PaginaInhoud" 
    runat="server">
<h2>Personen CHIRO
    <asp:Label ID="groepsLabel" runat="server" Text="Label"></asp:Label>
    </h2>

<p>
Dit is de geweldige testapplicatie.</p>
    <p>

    <asp:Label ID="domLabel" runat="server" Text="Label"></asp:Label>
</p>
    <p>
&nbsp;<asp:GridView ID="lijst" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="VoorNaam" HeaderText="Voornaam" />
            <asp:HyperLinkField DataNavigateUrlFields="PersoonID" 
                DataNavigateUrlFormatString="Persoon.aspx?id={0}" DataTextField="Naam" 
                HeaderText="Naam" />
            <asp:BoundField DataField="GeboorteDatum" HeaderText="Geboortedatum" 
                DataFormatString="{0:d}" />
            <asp:BoundField DataField="ChiroLeefTijd" HeaderText="Chiroleeftijd" />
        </Columns>
    </asp:GridView>
</p>

    <br />


</asp:Content>