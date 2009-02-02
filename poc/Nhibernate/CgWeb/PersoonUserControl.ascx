<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersoonUserControl.ascx.cs" Inherits="CgWeb.PersoonUserControl" %>
<p>
    PersoonID:
    <asp:Label ID="persoonIDLabel" runat="server" Text="Label"></asp:Label>
</p>
<p>
    Naam:</p>
<asp:TextBox ID="naamTextBox" runat="server"></asp:TextBox>
<p>
    Voornaam:</p>
<asp:TextBox ID="voorNaamTextBox" runat="server"></asp:TextBox>
<p>
    Geboortedatum:</p>
<asp:TextBox ID="geboorteDatumTextBox" runat="server"></asp:TextBox>
<p>
    Telefoonnummers:</p>
<asp:GridView ID="telefoonNrGrid" runat="server" AutoGenerateColumns="False">
    <Columns>
        <asp:BoundField DataField="Nummer" HeaderText="Nummer" />
        <asp:BoundField DataField="Voorkeur" HeaderText="Voorkeur" />
    </Columns>
</asp:GridView>
<asp:TextBox ID="nieuwNrTextBox" runat="server"></asp:TextBox>
<asp:Button ID="toevoegenButton" runat="server" onclick="toevoegenButton_Click" 
    Text="Nr. Toevoegen" />

