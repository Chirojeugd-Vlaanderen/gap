<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersoonUserControl.ascx.cs" Inherits="CgWeb.PersoonUserControl" %>
<p>
    Naam:</p>
<asp:TextBox ID="naamTextBox" runat="server"></asp:TextBox>
<p>
    Voornaam:</p>
<asp:TextBox ID="voorNaamTextBox" runat="server"></asp:TextBox>
<p>
    Geboortedatum:</p>
<asp:Calendar ID="geboorteDatumCalendar" runat="server"></asp:Calendar>
<p>
    Telefoonnummers:</p>
<asp:GridView ID="telefoonNrGrid" runat="server">
</asp:GridView>
